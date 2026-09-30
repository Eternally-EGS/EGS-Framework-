using EGS_Framework.GumRuntimes;
using FlatRedBall.Screens;
using Microsoft.Xna.Framework;

namespace EGS_Framework.Services.UI
{
    public static class MenuManager
    {
        public static void MenuInit(Main_MenuGumRuntime gumScreen)
        {
            // Setting
            SettingManager.MenuInit(gumScreen);
            // Resolution
            gumScreen.Start_ButtonI.Click += (window) => HandleStartButtonClick(gumScreen);
            gumScreen.Setting_ButtonI.Click  += (window) => HandleSettingButtonClick(gumScreen);
            gumScreen.Exit_ButtonI.Click  += (window) => HandleExitButtonClick(gumScreen);
        }

        private static void HandleStartButtonClick(Main_MenuGumRuntime gumScreen)
        {
            string projectNamespace = FlatRedBall.Screens.ScreenManager.CurrentScreen.GetType().Namespace;
            string fullScreenName = $"{projectNamespace}.GameScreen";
            ScreenManager.CurrentScreen.MoveToScreen(fullScreenName);
        }
        private static void HandleSettingButtonClick(Main_MenuGumRuntime gumScreen)
        {
                gumScreen.PopapSettingI.CurrentVisibilityState = PopapRuntime.Visibility.Opened;
        }
        private static void HandleExitButtonClick(Main_MenuGumRuntime gumScreen)
        {
            FlatRedBall.FlatRedBallServices.Game.Exit();
        }
    }
}