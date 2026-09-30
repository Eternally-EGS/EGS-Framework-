using EGS_Framework.Entities;

namespace EGS_Framework.Core.SaveSystem
{
    public static class SaveList
    {

        public static void Collect()
        {
            // System
            SaveManager.SetMain("CurrentSlot", SaveManager.CurrentSlot);
            //SaveManager.SetMain("CurrentResolution", );
            //SaveManager.SetMain("CurrentLanguage", );
            //SaveManager.SetMain("FullScreenEnable", );
            //SaveManager.SetMain("MusicVolume", SaveManager.CurrentSlot);
            //SaveManager.SetMain("SoundVolume", SaveManager.CurrentSlot);

           SaveManager.SetSlot("step2", fdkf.step2);
           SaveManager.SetSlot("step", fdkf.step);
           //SaveManager.SetMain("MusicVolume", Settings.MusicVolume);
            

        }
    }
}