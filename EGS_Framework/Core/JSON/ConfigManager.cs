using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using EGS_Framework.Core.Debug;
using EGS_Framework.Core.JSON.Сlasses;

namespace EGS_Framework.Core.JSON
{
    public static class ConfigManager
    {
        static readonly string ExportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Core", "Configs");
        public static GameConfig Game { get; private set; } = new();

        public static void LoadAll()
        {
            Directory.CreateDirectory(ExportPath);

            var type = typeof(GameConfig);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                string fileName = prop.Name + ".json";
                string path = Path.Combine(ExportPath, fileName);

                object instance = Activator.CreateInstance(prop.PropertyType);
                if (instance == null) continue;

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, Serializer.ToJson(instance));
                    prop.SetValue(Game, instance);
                    continue;
                }

                string json = File.ReadAllText(path);
                object loaded = LoadGeneric(prop.PropertyType, json, out bool success);

                if (!success)
                {
                    DebugConsole.Output.Add($"[CONFIG] {fileName} damaged.");
                    prop.SetValue(Game, instance);
                    continue;
                }

                MergeNewFields(path, loaded, prop.PropertyType);

                string updatedJson = File.ReadAllText(path);
                loaded = LoadGeneric(prop.PropertyType, updatedJson, out _);

                object existing = prop.GetValue(Game);
                if (existing == null)
                {
                    prop.SetValue(Game, loaded);
                    continue;
                }

                CopyFields(loaded, existing);
            }
        }

        private static void CopyFields(object source, object target)
        {
            var type = source.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;

                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
        }

        private static object LoadGeneric(Type type, string json, out bool success)
        {
            var method = typeof(Serializer).GetMethod("FromJson")!.MakeGenericMethod(type);
            object[] args = { json, false };
            var result = method.Invoke(null, args);
            success = (bool)args[1];
            return result!;
        }

        private static void MergeNewFields(string path, object loaded, Type type)
        {
            string json = File.ReadAllText(path);
            var node = JsonNode.Parse(json)?.AsObject() ?? new JsonObject();
            bool changed = false;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!node.ContainsKey(prop.Name))
                {
                    var val = prop.GetValue(loaded);
                    node[prop.Name] = JsonValue.Create(val);
                    changed = true;
                }
            }

            if (changed)
            {
                File.WriteAllText(path, node.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
                }));
            }
        }
    }
}