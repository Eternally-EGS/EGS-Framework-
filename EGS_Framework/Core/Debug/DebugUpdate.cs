using System.Collections.Generic;

namespace EGS_Framework.Core.Debug
{
    public static class DebugUpdate {
        // All window
        public static List<DebugWindow> debugWindows = new List<DebugWindow>();
        
        // Update
        public static void Update () {
            DebugManager.GetDebug();

            foreach (var window in debugWindows)
            {
                window.UpdateAndDraw();
            }
        }

        // Clear all window
        public static void ClearAll() 
        {
            var copy = new List<DebugWindow>(debugWindows);
            foreach (var window in copy)
            {
                try { window?.Close(); } catch { }
            }
            debugWindows.Clear();
            DebugWindow.ClearAll();
        }
    
    }
}