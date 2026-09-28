namespace EGS_Framework.Core.JSON
{
    public static class ConfigManager
    {
        public static GameConfig Game { get; private set; } = new();
    }
}