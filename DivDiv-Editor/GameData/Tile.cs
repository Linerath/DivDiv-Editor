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

        public int[] ToOldArray()
        {
            int[] arr = new int[96];

            var i = 0;
            arr[i++] = BottomTexture;
            arr[i++] = TopTexture;
            arr[i++] = Effects;
            arr[i++] = unknown1;
            arr[i++] = unknown2;
            arr[i++] = ObjectsCount;

            foreach (var obj in Objects)
            {
                foreach (var objValue in obj.ToOldArray())
                    arr[i++] = objValue;
            }

            return arr;
        }
    }

    public enum TileEffect
    {
        None = 0,
        Water = 2,
        Indoors = 4,
        Fog = 8,
        WaterFog = 10,
    }
}
