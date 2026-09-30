using System.Diagnostics;
using EGS_Framework.Core.SaveSystem;
using EGS_Framework.GumRuntimes;

namespace EGS_Framework.Services.UI
{
    public static class SettingManager
    {
        public static int CurrentResolution
        {
            get => SaveManager.GetSlot<int>("CurrentResolution", 1);
            set => SaveManager.SetSlot("CurrentResolution", value);
        }

        public static void MenuInit(Main_MenuGumRuntime gumScreen)
        {
            gumScreen.PopapSettingI.PExit_ButtonI.Click += (window) => HandlePExitButtonClick(gumScreen);
           
            var Resolution = new FlatRedBall.Forms.Controls.ComboBox(gumScreen.PopapSettingI.Resolution_ListI);

            // List Set
            Resolution.Items.Clear();
            Resolution.Items.Add("2560x1440");
            Resolution.Items.Add("1920x1080");
            Resolution.Items.Add("1600x900");
            Resolution.Items.Add("1366x768");
            Resolution.Items.Add("1280x720");
            Resolution.Items.Add("800x600");

            Resolution.SelectedIndex = CurrentResolution;

            Resolution.SelectionChanged += (sender, args) => HandleResolutionChange(Resolution);

            HandleResolutionChange(Resolution);

        }

        private static void HandlePExitButtonClick(Main_MenuGumRuntime gumScreen)
        {
            gumScreen.PopapSettingI.CurrentVisibilityState = PopapRuntime.Visibility.Closed;
        }

        public static void HandleResolutionChange(FlatRedBall.Forms.Controls.ComboBox comboBox)
        {
            string selectedValue = comboBox.SelectedObject as string;

         

            if (!string.IsNullOrEmpty(selectedValue))
            {
                CurrentResolution = comboBox.SelectedIndex;
                switch (selectedValue) 
                {
                    case "2560x1440": ChangeResolution(0); break;
                    case "1920x1080": ChangeResolution(1); break;
                    case "1600x900":  ChangeResolution(2); break;
                    case "1366x768":  ChangeResolution(3); break;
                    case "1280x720":  ChangeResolution(4); break;
                    case "800x600":   ChangeResolution(5); break;

                }
            }
        }

        private static void ChangeResolution(int Res)
        {
            switch (Res) { 
                case 0: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(2560, 1440);
                break;

                case 1: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(1920, 1080);
                break;

                case 2: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(1600, 900);
                break;

                case 3: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(1366, 768);
                break;

                case 4: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(1280, 720);
                break;
                
                case 5: 
                    FlatRedBall.FlatRedBallServices.GraphicsOptions.SetResolution(800, 600);
                break;
            }

        }

    }
}