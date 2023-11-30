using System.IO;

namespace DivDivEditor.GameObjects
{
    public class TileObject
    {
        private byte tempX;
        private byte tempY;
        private byte id2;
        private byte id3;
        public byte Height { get; set; }
        private byte name1;
        private byte name2;
        public byte Effect { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Number { get; set; }
        public int Name { get; set; }

        public TileObject(BinaryReader reader)
        {
            tempX = reader.ReadByte();
            tempY = reader.ReadByte();
            id2 = reader.ReadByte();
            id3 = reader.ReadByte();
            Height = reader.ReadByte();
            name1 = reader.ReadByte();
            name2 = reader.ReadByte();
            Effect = reader.ReadByte();

            var XY = tempX + tempY % 16 * 4 * 64;

            X = XY % 64;
            Y = XY / 64;

            Number = tempY / 16 + id2 * 16 + id3 * 4096;
            Name = name1 / 4 + name2 * 64;
        }

        public int[] ToOldArray() => new int[] { X, Y, Height, Name, Number, Effect };
    }
}
