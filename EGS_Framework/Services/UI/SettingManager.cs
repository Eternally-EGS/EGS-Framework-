using System;
using System.Diagnostics;
using EGS_Framework.Core.SaveSystem;
using EGS_Framework.GumRuntimes;
using FlatRedBall;

namespace EGS_Framework.Services.UI
{
    public static class SettingManager
    {
        // Load
        public static int CurrentResolution
        {
            get => SaveManager.GetMain<int>("CurrentResolution", 0);
            set => SaveManager.SetMain("CurrentResolution", value);
        }

        public static int CurrentLanguage
        {
            get => SaveManager.GetMain<int>("CurrentLanguage", 0);
            set => SaveManager.SetMain("CurrentLanguage", value);
        }

        public static bool CurrentFullScreen
        {
            get => SaveManager.GetMain<bool>("CurrentFullScreen", true);
            set => SaveManager.SetMain("CurrentFullScreen", value);
        }

        public static int CurrentMusicVolume
        {
            get => SaveManager.GetMain<int>("CurrentMusicVolume", 100);
            set => SaveManager.SetMain("CurrentMusicVolume", value);
        }

        public static int CurrentSoundVolume
        {
            get => SaveManager.GetMain<int>("CurrentSoundVolume", 100);
            set => SaveManager.SetMain("CurrentSoundVolume", value);
        }

        public static void MenuInit(Main_MenuGumRuntime gumScreen)
        {
            // Exit
            gumScreen.PopapSettingI.PExit_ButtonI.Click += (window) => HandlePExitButtonClick(gumScreen);

            // Input
            var Resolution = new FlatRedBall.Forms.Controls.ComboBox(gumScreen.PopapSettingI.Resolution_ListI);
            var Language = new FlatRedBall.Forms.Controls.ComboBox(gumScreen.PopapSettingI.Language_ListI);
            var FullScreen = new FlatRedBall.Forms.Controls.CheckBox(gumScreen.PopapSettingI.FullSreen_BoxI);
            
            var monitorWidth = Microsoft.Xna.Framework.Graphics.GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var monitorHeight = Microsoft.Xna.Framework.Graphics.GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            var MusicSlider = new FlatRedBall.Forms.Controls.Slider(gumScreen.PopapSettingI.MusicVolume_SliderI);
            var SoundSlider = new FlatRedBall.Forms.Controls.Slider(gumScreen.PopapSettingI.SoundVolume_SliderI);

            // Volum
            MusicSlider.Minimum = 0;
            MusicSlider.Maximum = 100;
            MusicSlider.Value = CurrentMusicVolume;

            SoundSlider.Minimum = 0;
            SoundSlider.Maximum = 100;
            SoundSlider.Value = CurrentSoundVolume;

            MusicSlider.ValueChanged += (sender, args) => HandleMusicVolumeChange(MusicSlider, gumScreen);
            SoundSlider.ValueChanged += (sender, args) => HandleSoundVolumeChange(SoundSlider, gumScreen);

            HandleMusicVolumeChange(MusicSlider, gumScreen);
            HandleSoundVolumeChange(SoundSlider, gumScreen);

            gumScreen.PopapSettingI.MusicVolume_TextI.Text = CurrentMusicVolume.ToString();
            gumScreen.PopapSettingI.SoundVolume_TextI.Text = CurrentSoundVolume.ToString();

            // Resolution
            Resolution.Items.Clear();

            void AddIfSupported(string resText, int targetW, int targetH)
            {
                if (targetW <= monitorWidth && targetH <= monitorHeight)
                {
                    Resolution.Items.Add(resText);
                }
            }

            AddIfSupported("2560x1440", 2560, 1440);
            AddIfSupported("1920x1080", 1920, 1080);
            AddIfSupported("1600x900",  1600, 900);
            AddIfSupported("1366x768",  1366, 768);
            AddIfSupported("1280x720",  1280, 720);
            AddIfSupported("800x600",   800,  600);
            Resolution.ListBox.Height = 20 * 5;

            if (CurrentResolution >= Resolution.Items.Count)
            {
                CurrentResolution = 0; 
            }
            
            Resolution.SelectedIndex = CurrentResolution;
            Resolution.SelectionChanged += (sender, args) => HandleResolutionChange(Resolution);
            HandleResolutionChange(Resolution);
            
            // Language
            Language.Items.Clear();
            Language.Items.Add("Russian");
            Language.Items.Add("English");
            Language.ListBox.Height = 40;

            Language.SelectedIndex = CurrentLanguage;
            Language.SelectionChanged += (sender, args) => HandleLanguageChange(Language);
            HandleLanguageChange(Language);

            // FullScreen
            FullScreen.IsChecked = CurrentFullScreen;
            FullScreen.Checked += (sender, args) => HandleFullscreenChange(FullScreen);
            FullScreen.Unchecked += (sender, args) => HandleFullscreenChange(FullScreen);
            HandleFullscreenChange(FullScreen);
        }

        // Exit
        private static void HandlePExitButtonClick(Main_MenuGumRuntime gumScreen)
        {
            gumScreen.PopapSettingI.CurrentVisibilityState = PopapRuntime.Visibility.Closed;
        }

        // Resolution
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

        // Language
        public static void HandleLanguageChange(FlatRedBall.Forms.Controls.ComboBox comboBox)
        {
            string selectedValue = comboBox.SelectedObject as string;

            if (!string.IsNullOrEmpty(selectedValue))
            {
                CurrentLanguage = comboBox.SelectedIndex;
                switch (selectedValue) 
                {
                    case "Russian": ChangeLanguage(0); break;
                    case "English": ChangeLanguage(1); break;
                }
            }

        }

        private static void ChangeLanguage(int Lan)
        {
            switch (Lan) { 
                case 0: 

                break;
            }
        }

        // FullScreen
        public static void HandleFullscreenChange(FlatRedBall.Forms.Controls.CheckBox checkbox)
        {
            CurrentFullScreen = checkbox.IsChecked ?? false;
            if (checkbox.IsChecked == false) { ChangeFullscreen(0); }
            if (checkbox.IsChecked == true) { ChangeFullscreen(1); }
        }

        private static void ChangeFullscreen(int Vol) 
        {

            if (Vol == 0) {
                FlatRedBallServices.GraphicsOptions.IsFullScreen = false;
                ChangeResolution(SettingManager.CurrentResolution);
            } 
            else 
            { 
                var displayMode = FlatRedBallServices.GraphicsDevice.DisplayMode;
                FlatRedBallServices.GraphicsOptions.SetFullScreen(displayMode.Width, displayMode.Height);
            }
        }

        // Volume
        private static void HandleMusicVolumeChange(FlatRedBall.Forms.Controls.Slider slider,Main_MenuGumRuntime gumScreen)
        {
            int volume = (int)Math.Round(slider.Value);
            CurrentMusicVolume = volume;
            gumScreen.PopapSettingI.MusicVolume_TextI.Text = CurrentMusicVolume.ToString();
        }

        private static void HandleSoundVolumeChange(FlatRedBall.Forms.Controls.Slider slider,Main_MenuGumRuntime gumScreen)
        {
            int volume = (int)Math.Round(slider.Value);
            CurrentSoundVolume = volume;
            gumScreen.PopapSettingI.SoundVolume_TextI.Text = CurrentSoundVolume.ToString();
        }
    }
}