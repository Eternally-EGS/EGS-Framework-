using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using EGS_Framework.Core.JSON;
using EGS_Framework.Core.SaveSystem;
using FlatRedBall;
using FlatRedBall.Screens;

namespace EGS_Framework.Core.Debug
{
    public static class DebugConsoleHelper {
        // Pars command
        public static void ExtractCommand(string Input) {
            
            string[] parts = Input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string one = parts[0];
            string two = parts.Length > 1 ? parts[1] : "";
            string three = parts.Length > 2 ? parts[2] : "";
            string four = parts.Length > 3 ? parts[3] : "";
            //string five = parts.Length > 4 ? parts[4] : ""; ...

            // Command list
            switch(one.ToUpper().Trim()) {
            case "HELP" : DebugConsole.Output.Clear(); DebugConsole.Output.AddRange(HELP());  break;
            case "CL" : DebugConsole.Output.Clear(); break;
            case "DOS" : DebugConsole.Output.AddRange(DOS(two)); break;
            case "GOTO" : DebugConsole.Output.Add(GOTO(two)); break;
            case "R" : DebugConsole.Output.Add(R()); break;
            case "RJS" : DebugConsole.Output.Add(RJS()); break;
            case "SAVE" : DebugConsole.Output.AddRange(SAVE()); break;
            case "SET" : DebugConsole.Output.Add(SET(two,three,four)); break;
            case "ANY" : DebugConsole.Output.Add(ANY()); break;
            default: DebugConsole.Output.Add(ERR()); break;
            }
            
        }

        // Show all command 
        public static string[] HELP() {
             return new string[] {
                "Available commands:",
                "DOS <name>     - Inspect object",
                "SET <field>    - Change value",
                "SAVE           - Save state to JSON",
                "R              - Full game restart",
                "GOTO <screen>  - Switch screen",
                "RJS            - Reload configs + restart screen",
                "CL             - Clear console"
            };
        }

        // Placeholder 
        public static string ERR() {
            return "Command not found";
        }

        // Restart room
        public static string R() {
            ScreenManager.CurrentScreen.RestartScreen();
            return "Command complete";
        }

        // Reset JSON
        public static string RJS() {
            ScreenManager.CurrentScreen.RestartScreen();
            ConfigManager.LoadAll();
            return "Command complete";
        }

        public static string ANY() {
            SaveManager.SwitchSlot(10);
            SaveManager.SaveAll();
            return "Command maybe complete";  
        }

        // Change screen
        public static string GOTO(string Screen) 
        {

            // Get screen path
            string projectNamespace = FlatRedBall.Screens.ScreenManager.CurrentScreen.GetType().Namespace;
            string fullScreenName = $"{projectNamespace}.{Screen.Trim()}";

            Type screenType = null;

            screenType = Type.GetType(fullScreenName);

            // Is null
            if (screenType == null)
                return $"Screen '{Screen}' not found";
            // Move
            ScreenManager.CurrentScreen.MoveToScreen(fullScreenName);

            return "Command complete";
        }

        // Stats
        public static string[] DOS(string Objname)
        {
            // Get obj name
            string OBJ = Objname.Trim();

            // All obj
            if (OBJ == "")
            {
                var managedObjects = FlatRedBall.SpriteManager.ManagedPositionedObjects;

                string[] result = new string[managedObjects.Count + 1];
                result[0] = "=== All objects ===";

                for (int i = 0; i < managedObjects.Count; i++)
                {
                    var o = managedObjects[i];
                    result[i + 1] = $"[{o.GetType().Name}] Name='{o.Name}'";
                }

                return result;
            }

            // Stats
            var obj = FlatRedBall.SpriteManager.ManagedPositionedObjects
                .FirstOrDefault(o =>
                    !string.IsNullOrEmpty(o.Name) &&
                    string.Equals(o.Name.Trim(), OBJ, StringComparison.OrdinalIgnoreCase));

            if (obj == null)
                return new string[] { $"Object '{OBJ}' not found" };

            // Filter
            var type = obj.GetType();

            var fieldInfos = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                    | BindingFlags.DeclaredOnly)
                .Where(f =>
                    !f.Name.StartsWith("m") &&
                    f.Name != "Texture" &&
                    f.Name != "SpriteInstance" &&
                    f.Name != "LayerProvidedByContainer" &&
                    f.Name != "Visible")
                .ToArray();

            var propInfos = type.GetProperties(
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                    | BindingFlags.DeclaredOnly)
                .Where(p =>
                    p.CanRead &&
                    p.GetIndexParameters().Length == 0 &&
                    !p.Name.StartsWith("m"))
                .ToArray();

            // View
            DebugWindow View = new DebugWindow(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine($"=== {obj.Name} ({type.Name}) ===");

                // base
                sb.AppendLine($"X: {obj.X:F2}  Y: {obj.Y:F2}  Z: {obj.Z:F2}");
                sb.AppendLine($"RotZ: {obj.RotationZ:F2}  RotX: {obj.RotationX:F2}  RotY: {obj.RotationY:F2}");
                sb.AppendLine($"Vel: ({obj.Velocity.X:F1},{obj.Velocity.Y:F1},{obj.Velocity.Z:F1})");
                sb.AppendLine($"Acc: ({obj.Acceleration.X:F1},{obj.Acceleration.Y:F1},{obj.Acceleration.Z:F1})");

                // FIELDS
                sb.AppendLine("--- FIELDS ---");
                if (fieldInfos.Length == 0)
                {
                    sb.AppendLine("(no own fields)");
                }
                else
                {
                    foreach (var f in fieldInfos)
                    {
                        object val = null;
                        try
                        {
                            val = f.IsStatic ? f.GetValue(null) : f.GetValue(obj);
                        }
                        catch
                        {
                            val = "<err>";
                        }

                        string tag = f.IsPublic ? "+" : "-";
                        string staticTag = f.IsStatic ? "static " : "";
                        sb.AppendLine($"{tag} {staticTag}{f.Name} = {val}");
                    }
                }

                // PROPERTIES
                sb.AppendLine("--- PROPERTIES ---");
                if (propInfos.Length == 0)
                {
                    sb.AppendLine("(no own properties)");
                }
                else
                {
                    foreach (var p in propInfos)
                    {
                        object val = null;
                        try
                        {
                            bool isStatic = p.GetMethod?.IsStatic == true;
                            val = p.GetValue(isStatic ? null : obj);
                        }
                        catch
                        {
                            val = "<err>";
                        }

                        string tag = p.GetMethod?.IsPublic == true ? "+" : "-";
                        string staticTag = p.GetMethod?.IsStatic == true ? "static " : "";
                        sb.AppendLine($"{tag} {staticTag}{p.Name} = {val}");
                    }
                }

                return sb.ToString();
            });

            // Update
            DebugUpdate.debugWindows.Add(View);
            return new string[] { "Command complete" };
        }

        // Save to json
        public static string[] SAVE() 
        {
            // list
            var allData = new List<Dictionary<string, object>>();

            // finde all obj            
            foreach (var obj in FlatRedBall.SpriteManager.ManagedPositionedObjects)
            {
                if (string.IsNullOrEmpty(obj.Name)) continue;

                var type = obj.GetType();
                
                // all info 
                var entry = new Dictionary<string, object>
                {
                    ["Name"] = obj.Name,
                    ["Type"] = type.Name,
                    ["X"] = SafeFloat(obj.X),
                    ["Y"] = SafeFloat(obj.Y),
                    ["Z"] = SafeFloat(obj.Z),
                    ["RotationZ"] = SafeFloat(obj.RotationZ),
                    ["RotationX"] = SafeFloat(obj.RotationX),
                    ["RotationY"] = SafeFloat(obj.RotationY),
                    ["VelocityX"] = SafeFloat(obj.Velocity.X),
                    ["VelocityY"] = SafeFloat(obj.Velocity.Y),
                    ["VelocityZ"] = SafeFloat(obj.Velocity.Z),
                    ["AccelerationX"] = SafeFloat(obj.Acceleration.X),
                    ["AccelerationY"] = SafeFloat(obj.Acceleration.Y),
                    ["AccelerationZ"] = SafeFloat(obj.Acceleration.Z),
                };

                var fieldInfos = type.GetFields(
                        BindingFlags.Public | BindingFlags.NonPublic 
                        | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(f => 
                        !f.Name.StartsWith("m") &&
                        f.Name != "Texture" &&
                        f.Name != "SpriteInstance" &&
                        f.Name != "LayerProvidedByContainer" &&
                        f.Name != "Visible")
                    .ToArray();

                // Make Serializable
                foreach (var f in fieldInfos)
                {
                    object val = null;
                    try { val = f.GetValue(obj); } 
                    catch { val = "<err>"; }

                    entry[$"field_{f.Name}"] = MakeSerializable(val);
                }

                // Add to list
                allData.Add(entry);
            }

            // json format
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            // Serializable
            string json = JsonSerializer.Serialize(allData, options);
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "core", "Log");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            // Date
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            
            string fileName = $"scene_dump_{timeStamp}.json";
            string path = Path.Combine(logDirectory, fileName);

            // Writing
            try 
            {
                File.WriteAllText(path, json);
            } 
            catch (Exception ex) 
            {
                return new string[] { $"Error saving: {ex.Message}" };
            }
            
            // Short path
            string relativePath = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, path);

            return new string[] 
            { 
                "=== SAVE SYSTEM ===",
                $"Status: Successfully saved {allData.Count} objects.",
                $"Path:{relativePath}"
            };
        }
        
        // Convert float var
        private static object SafeFloat(float v) 
        {
            if (float.IsNaN(v) || float.IsInfinity(v)) return v.ToString();
            return v;
        }

        // Convert to json format (int,string ...)
        private static object MakeSerializable(object val)
        {
            if (val == null) return null;

            var t = val.GetType();

            if (val is string || val is bool || val is int || val is long 
            || val is float || val is double || val is decimal)
            {
                if (val is float f && (float.IsNaN(f) || float.IsInfinity(f))) return f.ToString();
                if (val is double d && (double.IsNaN(d) || double.IsInfinity(d))) return d.ToString();
                return val;
            }

            if (t.IsEnum) return val.ToString();

            if (val is System.Collections.IEnumerable e && !(val is string))
            {
                int count = 0;
                foreach (var _ in e) count++;
                return $"[collection of {count}]";
            }

            return val.ToString();
        }

        // Set key value
        public static string SET(string Objname, string Fields, string Value) 
        {
            Objname = Objname?.Trim();
            Fields  = Fields?.Trim();
            Value   = Value?.Trim();

            if (string.IsNullOrEmpty(Objname) || string.IsNullOrEmpty(Fields))
                return "Usage: SET <obj> <field> <value>";

            // OBJ
            var obj = FlatRedBall.SpriteManager.ManagedPositionedObjects
                .FirstOrDefault(o => 
                    !string.IsNullOrEmpty(o.Name) &&
                    string.Equals(o.Name.Trim(), Objname, StringComparison.OrdinalIgnoreCase));

            if (obj == null)
                return $"Object '{Objname}' not found";

            var type = obj.GetType();

            // Base
            switch (Fields.ToLower())
            {
                case "x":
                    if (float.TryParse(Value, out var x)) { obj.X = x; return $"X = {x}"; }
                    return "Invalid value for X";

                case "y":
                    if (float.TryParse(Value, out var y)) { obj.Y = y; return $"Y = {y}"; }
                    return "Invalid value for Y";

                case "z":
                    if (float.TryParse(Value, out var z)) { obj.Z = z; return $"Z = {z}"; }
                    return "Invalid value for Z";

                case "rotationz":
                    if (float.TryParse(Value, out var rz)) { obj.RotationZ = rz; return $"RotationZ = {rz}"; }
                    return "Invalid value for RotationZ";

                case "rotationx":
                    if (float.TryParse(Value, out var rx)) { obj.RotationX = rx; return $"RotationX = {rx}"; }
                    return "Invalid value for RotationX";

                case "rotationy":
                    if (float.TryParse(Value, out var ry)) { obj.RotationY = ry; return $"RotationY = {ry}"; }
                    return "Invalid value for RotationY";

                case "velocity":
                    var v = ParseVector3(Value);
                    if (v == null) return "Usage: SET <obj> velocity <x,y,z>";
                    obj.Velocity = v.Value;
                    return $"Velocity = ({v.Value.X},{v.Value.Y},{v.Value.Z})";

                case "acceleration":
                    var a = ParseVector3(Value);
                    if (a == null) return "Usage: SET <obj> acceleration <x,y,z>";
                    obj.Acceleration = a.Value;
                    return $"Acceleration = ({a.Value.X},{a.Value.Y},{a.Value.Z})";
            }

            // ★ Поиск поля
            var field = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                    | BindingFlags.DeclaredOnly)
                .FirstOrDefault(f =>
                    !f.Name.StartsWith("m") &&
                    f.Name != "Texture" &&
                    f.Name != "SpriteInstance" &&
                    f.Name != "LayerProvidedByContainer" &&
                    f.Name != "Visible" &&
                    string.Equals(f.Name, Fields, StringComparison.OrdinalIgnoreCase));

            // ★ Если поля нет — ищем свойство
            if (field == null)
            {
                var prop = type.GetProperties(
                        BindingFlags.Public | BindingFlags.NonPublic
                        | BindingFlags.Instance | BindingFlags.Static
                        | BindingFlags.DeclaredOnly)
                    .FirstOrDefault(p =>
                        p.CanWrite &&
                        p.GetIndexParameters().Length == 0 &&
                        !p.Name.StartsWith("m") &&
                        string.Equals(p.Name, Fields, StringComparison.OrdinalIgnoreCase));

                if (prop == null)
                    return $"Field or property '{Fields}' not found on {type.Name}";

                try
                {
                    object converted = ConvertValue(Value, prop.PropertyType);
                    if (converted == null)
                        return $"Cannot convert '{Value}' to {prop.PropertyType.Name}";

                    if (prop.SetMethod?.IsStatic == true)
                        prop.SetValue(null, converted);
                    else
                        prop.SetValue(obj, converted);

                    return $"{prop.Name} = {converted}";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }

            try
            {
                object converted = ConvertValue(Value, field.FieldType);
                if (converted == null)
                    return $"Cannot convert '{Value}' to {field.FieldType.Name}";

                if (field.IsStatic)
                    field.SetValue(null, converted);
                else
                    field.SetValue(obj, converted);

                return $"{field.Name} = {converted}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // Conver Vector
        private static Microsoft.Xna.Framework.Vector3? ParseVector3(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) return null;

            if (float.TryParse(parts[0], out var x) &&
                float.TryParse(parts[1], out var y) &&
                float.TryParse(parts[2], out var z))
            {
                return new Microsoft.Xna.Framework.Vector3(x, y, z);
            }
            return null;
        }

        // End convert
        private static object ConvertValue(string value, Type targetType)
        {
            try
            {
                if (targetType == typeof(string))  return value;
                if (targetType == typeof(int))     return int.Parse(value);
                if (targetType == typeof(float))   return float.Parse(value);
                if (targetType == typeof(double))  return double.Parse(value);
                if (targetType == typeof(bool))    return bool.Parse(value);
                if (targetType == typeof(long))    return long.Parse(value);

                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, ignoreCase: true);

                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }

        // Log func
        public static void SendConsoleLog(string text) {
            DebugConsole.Output.Add(text);
        }

    }
}