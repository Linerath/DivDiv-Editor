using System.IO;
using System.Text;

namespace DivDivEditor.FileAccess
{
    public static class FileLogger
    {
        public static void LogOldTiles(int[,,] tiles, string fileName = "TilesOld")
        {
            StringBuilder sb = new();

            for (int y = 0; y < tiles.GetLength(0); y++)
            {
                for (int x = 0; x < tiles.GetLength(1); x++)
                {
                    var value = $"{tiles[y, x, 0]} ";
                    sb.Append(value);
                }
                sb.AppendLine();
            }

            Log(fileName, sb.ToString());
        }

        private static void Log(string fileName, string content)
        {
            var filePath = $@"{Settings.FilesLogDirectory}\{fileName}.txt";

            File.Delete(filePath);
            File.WriteAllText(filePath, content);
        }
    }
}
