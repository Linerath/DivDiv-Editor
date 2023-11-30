using System.Collections.Generic;
using System.IO;

namespace DivDivEditor.GameObjects
{
    public class Tile
    {
        public int BottomTexture { get; private set; }
        public int TopTexture { get; private set; }
        private int buffer1;
        public byte ObjectsCount { get; private set; }
        public int Effects { get; private set; }
        private byte buffer2;
        private int unknown1;
        private int unknown2;
        private int buffer3;

        public List<TileObject> Objects { get; private set; }

        public int[] Array => new[] { BottomTexture, TopTexture, buffer1, ObjectsCount, Effects, buffer2, unknown1, unknown2, buffer3 };

        public Tile(BinaryReader reader)
        {
            BottomTexture = reader.ReadUInt16();
            TopTexture = reader.ReadUInt16();
            buffer1 = reader.ReadUInt16();
            ObjectsCount = reader.ReadByte();
            Effects = reader.ReadUInt16();
            buffer2 = reader.ReadByte();
            unknown1 = reader.ReadUInt16();
            unknown2 = reader.ReadUInt16();
            buffer3 = reader.ReadUInt16();

            Objects = new List<TileObject>(ObjectsCount);

            for (byte i = 0; i < ObjectsCount; i++)
            {
                TileObject tileObj = new(reader);
                Objects.Add(tileObj);
            }
        }
    }
}
