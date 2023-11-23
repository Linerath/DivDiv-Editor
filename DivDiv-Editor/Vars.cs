namespace DivDivEditor
{
    public static class Vars
    {
        public const int WorldWidth = 512;
        public const int WorldHeight = 1024;

        public const int TileBytesPerHash = 2;
        public const int RowHashBytes = WorldWidth * TileBytesPerHash;

        public const int GlobalHashBytesPerRow = 4;
        public const int GlobalHashBytes = WorldHeight * GlobalHashBytesPerRow;
    }
}
