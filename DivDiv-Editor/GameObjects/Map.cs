using System.IO;

namespace DivDivEditor.GameObjects
{
    public class Map
    {
        public Tile[,] Tiles { get; private set; } = new Tile[Vars.WorldHeight, Vars.WorldWidth];

        public Map(BinaryReader reader)
        {
            var globalHash = reader.ReadBytes(Vars.GlobalHashBytes);

            for (int y = 0; y < Vars.WorldHeight; y++)
            {
                var rowHash = reader.ReadBytes(Vars.RowHashBytes);

                for (int x = 0; x < Vars.WorldWidth; x++)
                {
                    Tile tile = new(reader);
                    Tiles[y, x] = tile;
                }
            }
        }

        public int[,,] ToOldTilesArray()
        {
            int[,,] tilesArray = new int[Vars.WorldHeight, Vars.WorldWidth, 96];

            for (int y = 0; y < Vars.WorldHeight; y++)
            {
                for (int x = 0; x < Vars.WorldWidth; x++)
                {
                    var tileArray = Tiles[y, x].Array;

                    for (int i = 0; i < tileArray.Length; i++)
                    {
                        tilesArray[y, x, i] = tileArray[i];
                    }
                }
            }

            return tilesArray;
        }
    }
}
