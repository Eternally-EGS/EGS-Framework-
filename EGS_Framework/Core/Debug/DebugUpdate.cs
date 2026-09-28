using System.Collections.Generic;

namespace EGS_Framework.Core.Debug
{
    public static class DebugUpdate {
        public static List<DebugWindow> debugWindows = new List<DebugWindow>();
        



        public static void Update () {
            DebugManager.GetDebug();

            foreach (var window in debugWindows)
            {
                window.UpdateAndDraw();
            }
       

        }
    
    }
}