using EGS_Framework.Entities;

namespace EGS_Framework.Core.SaveSystem
{
    public static class SaveList
    {

        public static void Collect()
        {
           SaveManager.SetSlot("step2", fdkf.step2);
           SaveManager.SetSlot("step", fdkf.step);
           //SaveManager.SetMain("MusicVolume", Settings.MusicVolume);
            SaveManager.SetMain("CurrentSlot", SaveManager.CurrentSlot);

        }
    }
}