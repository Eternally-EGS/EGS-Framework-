namespace EGS_Framework.Core.Debug
{
    public static class DebugCleanup
    {
        public static void Clear() {
            // Console
            DebugConsole.ResetConsole();
            // Dashboard text
            DebugDrawManager.RemoveDebugStatOnScreen(); 
            // Dashboard
            DebugDashBoard.DashBoardAllClear();
            // Update
            DebugUpdate.ClearAll();
        }

    }
}