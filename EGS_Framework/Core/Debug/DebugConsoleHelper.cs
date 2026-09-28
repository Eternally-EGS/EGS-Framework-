using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FlatRedBall.Screens;

namespace EGS_Framework.Core.Debug
{
    public static class DebugConsoleHelper {
        public static void ExtractCommand(string Input) {
            
            string[] parts = Input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string one = parts[0];
            string two = parts.Length > 1 ? parts[1] : "";
            string three = parts.Length > 2 ? parts[2] : "";
            string four = parts.Length > 3 ? parts[3] : "";
            string five = parts.Length > 4 ? parts[4] : "";

            // Command list
            switch(one.ToUpper().Trim()) {
            case "HELP" : DebugConsole.Output.Clear(); DebugConsole.Output.AddRange(HELP());  break;
            case "CL" : DebugConsole.Output.Clear(); break;
            case "DOS" : DebugConsole.Output.Add(DOS(two)); break;
            case "SET" : DebugConsole.Output.Add(SET(two,three,four)); break;
            default: DebugConsole.Output.Add(ERR()); break;
            }
            
        }

        // Show all command 
        public static string[] HELP() {
            return new string[] {
                "Available commands:",
                "DOS <name>     - Inspect object",
                "SET <field>    - Change value",
                "CO <name>      - Create object",
                "CALL <method>  - Call method",
                "DAD            - Drag & Drop mode",
                "SAVE           - Save state to JSON",
                "R              - Full game restart",
                "R -0           - Restart current screen",
                "GOTO <screen>  - Switch screen",
                "KILL <name>    - Delete object",
                "CL             - Clear console"
            };
        }

        // Placeholder 
        public static string ERR() {
            return "Command not found";
        }


        public static string DOS(string Objname) 
        {
            string OBJ = Objname.Trim();
            var sb = new StringBuilder();
            
            // All obj
            if (OBJ == "") 
            {
                sb.AppendLine("=== All objects ===");

                foreach (var o in FlatRedBall.SpriteManager.ManagedPositionedObjects)
                {
                    sb.AppendLine($"[{o.GetType().Name}] Name='{o.Name}'");
                }
                return sb.ToString();
            } 
            else 
            // Stats
            {
                var obj = FlatRedBall.SpriteManager.ManagedPositionedObjects
                    .FirstOrDefault(o => 
                        !string.IsNullOrEmpty(o.Name) &&
                        string.Equals(o.Name.Trim(), OBJ, StringComparison.OrdinalIgnoreCase));

                if (obj == null)
                    return $"Object '{OBJ}' not found";

                var type = obj.GetType();
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

                // Wiew
                DebugWindow View = new DebugWindow(() =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"=== {obj.Name} ({type.Name}) ===");

                    // base
                    sb.AppendLine($"X: {obj.X:F2}  Y: {obj.Y:F2}  Z: {obj.Z:F2}");
                    sb.AppendLine($"RotZ: {obj.RotationZ:F2}  RotX: {obj.RotationX:F2}  RotY: {obj.RotationY:F2}");
                    sb.AppendLine($"Vel: ({obj.Velocity.X:F1},{obj.Velocity.Y:F1},{obj.Velocity.Z:F1})");
                    sb.AppendLine($"Acc: ({obj.Acceleration.X:F1},{obj.Acceleration.Y:F1},{obj.Acceleration.Z:F1})");

                    // variable
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
                            try { val = f.GetValue(obj); } 
                            catch { val = "<err>"; }

                            string tag = f.IsPublic ? "+" : "-";
                            sb.AppendLine($"{tag} {f.Name} = {val}");
                        }
                    }

                    return sb.ToString();
                });

                // Update
                DebugUpdate.debugWindows.Add(View);
                return "Command complete";
            }
        }

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

            // Custom
            var field = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic 
                    | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(f => 
                    !f.Name.StartsWith("m") &&
                    f.Name != "Texture" &&
                    f.Name != "SpriteInstance" &&
                    f.Name != "LayerProvidedByContainer" &&
                    f.Name != "Visible" &&
                    string.Equals(f.Name, Fields, StringComparison.OrdinalIgnoreCase));

            if (field == null)
                return $"Field '{Fields}' not found on {type.Name}";

            try
            {
                object converted = ConvertValue(Value, field.FieldType);
                if (converted == null)
                    return $"Cannot convert '{Value}' to {field.FieldType.Name}";

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



        public static void SendConsoleLog(string text) {
            DebugConsole.Output.Add(text);
        }

    }
}