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
            
            string[] parts = Input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
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



        public static void SendConsoleLog(string text) {
            DebugConsole.Output.Add(text);
        }

    }
}