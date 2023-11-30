using System.IO;
using System.Text;

namespace DivDivEditor.FileAccess
{
    public static class FileLogger
    {
        public static void LogTiles(int[,,] tiles, string fileName = "Tiles")
        {
            if (!Settings.LogginOn)
                return;

            StringBuilder sb = new();

            for (int y = 0; y < tiles.GetLength(0); y++)
            {
                for (int x = 0; x < tiles.GetLength(1); x++)
                {
                    for (int z = 0; z < tiles.GetLength(2); z++)
                    {
                        var value = $"{tiles[y, x, z]} ";
                        sb.Append(value);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            Log(fileName, sb.ToString());
        }

        private static void Log(string fileName, string content)
        {
            if (!Settings.LogginOn)
                return;

            var filePath = $@"{Settings.FilesLogDirectory}\{fileName}.txt";

            File.Delete(filePath);
            File.WriteAllText(filePath, content);
        }
    }
}
