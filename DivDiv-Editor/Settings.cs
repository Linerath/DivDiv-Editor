namespace DivDivEditor
{
    public static class Settings
    {
        public const string GameDirectory = @"E:\SteamLibrary\steamapps\common\divine_divinity";
        public const string GameStartupDirectory = @"E:\SteamLibrary\steamapps\common\divine_divinity\main\startup";
        public const string GameFilesExtension = ".x0";

        public const string WorldFile = $@"{GameStartupDirectory}\world.x0";
        public const string ObjectsFile = $@"{GameStartupDirectory}\objects.x0";
        public const string DataFile = $@"{GameStartupDirectory}\data.000";

        public static bool ShowObjects = false;
        public static bool ShowEncounters = false;
        public static bool ShowConsole = true;
        public static bool ShowMenu = false;
    }
}
