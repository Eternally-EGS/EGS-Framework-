using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EGS_Framework.Core.Debug;
using EGS_Framework.Core.JSON;

namespace EGS_Framework.Core.SaveSystem
{
    public static class SaveManager
    {
        public readonly static string SavePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EGS_Framework", "Saves");

        public readonly static string MainPath = Path.Combine(SavePath, "Main.json");

        // === CURRENT SLOT ===
        public static int CurrentSlot
        {
            get => GetMain<int>("CurrentSlot", 0);
            set => SetMain("CurrentSlot", value);
        }

        public static string SlotPath => Path.Combine(SavePath, $"slot{CurrentSlot}", "save.json");

        public static Dictionary<string, object> Main = new();
        public static Dictionary<string, object> Slot = new();

        // === MAIN ===
        public static void SetMain<T>(string key, T value) => Main[key] = value!;

        public static T GetMain<T>(string key, T defaultValue = default!)
        {
            if (!Main.TryGetValue(key, out var v) || v == null)
                return defaultValue;

            return Unwrap<T>(v, defaultValue);
        }

        // === SLOT ===
        public static void SetSlot<T>(string key, T value) => Slot[key] = value!;

        public static T GetSlot<T>(string key, T defaultValue = default!)
        {
            if (!Slot.TryGetValue(key, out var v) || v == null)
                return defaultValue;

            return Unwrap<T>(v, defaultValue);
        }

        // === UNWRAP HELPER ===
        private static T Unwrap<T>(object v, T defaultValue)
        {
            if (v is T t) return t;

            if (v is JsonElement je)
            {
                try
                {
                    if (typeof(T) == typeof(string)) return (T)(object)je.GetString()!;
                    if (typeof(T) == typeof(int))    return (T)(object)je.GetInt32();
                    if (typeof(T) == typeof(long))   return (T)(object)je.GetInt64();
                    if (typeof(T) == typeof(float))  return (T)(object)je.GetSingle();
                    if (typeof(T) == typeof(double)) return (T)(object)je.GetDouble();
                    if (typeof(T) == typeof(bool))   return (T)(object)je.GetBoolean();
                }
                catch { return defaultValue; }
            }

            try { return (T)Convert.ChangeType(v, typeof(T)); }
            catch { return defaultValue; }
        }

        // === SAVE / LOAD ===
        public static void LoadAll()
        {
            Directory.CreateDirectory(SavePath);

            if (File.Exists(MainPath))
                Main = Serializer.FromJson<Dictionary<string, object>>(File.ReadAllText(MainPath), out _) ?? new();

            if (File.Exists(SlotPath))
                Slot = Serializer.FromJson<Dictionary<string, object>>(File.ReadAllText(SlotPath), out _) ?? new();

            DebugConsoleHelper.SendConsoleLog($"Load complete (slot {CurrentSlot})");
        }

        public static void SaveAll()
        {
            SaveList.Collect();

            Directory.CreateDirectory(SavePath);
            File.WriteAllText(MainPath, Serializer.ToJson(Main));

            string slotDir = Path.Combine(SavePath, $"slot{CurrentSlot}");
            Directory.CreateDirectory(slotDir);
            File.WriteAllText(SlotPath, Serializer.ToJson(Slot));
        }

        public static void SwitchSlot(int slot)
        {
            SaveAll();

            CurrentSlot = slot;
            File.WriteAllText(MainPath, Serializer.ToJson(Main));

            Slot = new();
            if (File.Exists(SlotPath))
                Slot = Serializer.FromJson<Dictionary<string, object>>(File.ReadAllText(SlotPath), out _) ?? new();

            DebugConsoleHelper.SendConsoleLog($"Switched to slot {slot}");
        }
    }
}