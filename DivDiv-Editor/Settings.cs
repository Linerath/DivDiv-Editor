namespace DivDivEditor
{
    public static class Settings
    {
        public const bool LogginOn = false;

        public const string GameDirectory = @"E:\SteamLibrary\steamapps\common\divine_divinity";
        public const string GameStartupDirectory = @"E:\SteamLibrary\steamapps\common\divine_divinity\main\startup";
        public const string GameFilesExtension = ".x0";

        public const string WorldFile = $@"{GameStartupDirectory}\world.x0";
        public const string ObjectsFile = $@"{GameStartupDirectory}\objects.x0";
        public const string DataFile = $@"{GameStartupDirectory}\data.000";
        public const string ObjectsInfoFile = "objects.de";
        public const string EditorFile = @"editor.dat";

        public const string FilesLogDirectory = @"C:\prging\DivDiv-Editor\FileLogs";

        public static bool ShowObjects = true;
        public static bool ShowEncounters = false;
        public static bool ShowConsole = true;
        public static bool ShowMenu = true;

        public static bool ShowTileEffect = false;
    }
}
