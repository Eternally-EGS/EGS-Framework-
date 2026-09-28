namespace EGS_Framework.Core.Debug
{
    public static class DebugCleanup
    {
        public static void Clear() {
            DebugConsole.RemoveAllConsole();
            DebugDrawManager.RemoveDebugStatOnScreen(); 
            DebugDashBoard.DashBoardAllClear();
            DebugUpdate.ClearAll();
        }

    }
}