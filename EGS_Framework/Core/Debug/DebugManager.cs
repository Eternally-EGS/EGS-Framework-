
using System.Collections.Generic;
using FlatRedBall.Input;
namespace EGS_Framework.Core.Debug
{
    public static class DebugManager {

        // Hud flag
        private static int DashBoard_Enebled = 0;
        private static int Console_Enebled = 0;

        public static void GetDebug() {

            // Open hud
            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.F3) && Console_Enebled == 0)
            {
                DashBoard_Enebled = 1 - DashBoard_Enebled;

                if (DashBoard_Enebled == 0) { DebugDrawManager.RemoveDebugStatOnScreen(); DebugDashBoard.DashBoardClear(); }
            }

            if (DashBoard_Enebled == 1) {
                DebugDrawManager.Reset();
                DebugDashBoard.GetDashBoardInfo();
            } 

            // Open console
            var keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.F4) && DashBoard_Enebled == 0)
                {
                    Console_Enebled = 1 - Console_Enebled;

                    if (Console_Enebled == 0) { DebugConsole.RemoveConsole(); }
                }

            if (Console_Enebled == 1) {
                DebugDrawManager.Reset();
                DebugConsole.GetConsole();
            }
        } 
    }
}
