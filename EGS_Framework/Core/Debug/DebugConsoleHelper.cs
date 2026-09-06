namespace EGS_Framework.Core.Debug
{
    public static class DebugConsoleHelper {
        public static void ExtractCommand(string Input) {
            // Command list
            switch(Input.ToUpper().Trim()) {
            case "HELP" : DebugConsole.Output.Clear(); DebugConsole.Output.AddRange(HELP());  break;
            case "CL" : DebugConsole.Output.Clear(); break;
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

        public static void SendConsoleLog(string text) {
            DebugConsole.Output.Add(text);
        }

    }
}