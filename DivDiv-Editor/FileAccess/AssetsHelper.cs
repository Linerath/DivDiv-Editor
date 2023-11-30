namespace DivDivEditor.FileAccess
{
    public class AssetsHelper
    {
        private const string TileTexturesPath = "floor";

        public static string GetTileTextureName(int value)
        {
            if (value >= 9369)
                return $"{TileTexturesPath}/003271";

            var formattedValue = value.ToString().PadLeft(6, '0');
            var name = $"{TileTexturesPath}/{formattedValue}";

            return name;
        }
    }
}
