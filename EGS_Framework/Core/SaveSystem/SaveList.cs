using EGS_Framework.Entities;
using EGS_Framework.Services.UI;

namespace EGS_Framework.Core.SaveSystem
{
    public static class SaveList
    {

        public static void Collect()
        {
            // System
            SaveManager.SetMain("CurrentSlot", SaveManager.CurrentSlot);
            SaveManager.SetMain("CurrentResolution", SettingManager.CurrentResolution);
            SaveManager.SetMain("CurrentLanguage", SettingManager.CurrentLanguage);
            SaveManager.SetMain("CurrentFullScreen", SettingManager.CurrentFullScreen);
            SaveManager.SetMain("MusicVolume", SettingManager.CurrentMusicVolume);
            SaveManager.SetMain("SoundVolume", SettingManager.CurrentSoundVolume);
            SaveManager.SetSlot("step2", fdkf.step2);
            SaveManager.SetSlot("step", fdkf.step);
        }
    }
}