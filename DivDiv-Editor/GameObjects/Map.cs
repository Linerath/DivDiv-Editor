using System.IO;

namespace DivDivEditor.GameObjects
{
    public class Map
    {
        public Tile[,] Tiles { get; private set; } = new Tile[Vars.WorldWidth, Vars.WorldHeight];

        public Map(BinaryReader reader)
        {
            var globalHash = reader.ReadBytes(Vars.GlobalHashBytes);

            for (int y = 0; y < Vars.WorldHeight; y++)
            {
                var rowHash = reader.ReadBytes(Vars.RowHashBytes);

                for (int x = 0; x < Vars.WorldWidth; x++)
                {
                    Tile tile = new(reader);
                    Tiles[x, y] = tile;
                }
            }
        }
    }
}
