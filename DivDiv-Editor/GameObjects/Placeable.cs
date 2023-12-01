using System.IO;

namespace DivDivEditor.GameObjects
{
    public class Placeable
    {
        private byte[] unknownBytes = new byte[20];
        private int x;
        private int y;
        private byte buffer1;
        private byte buffer2;
        private int name;

        public Placeable(BinaryReader reader)
        {
            for (int i = 0; i < unknownBytes.Length; i++)
                unknownBytes[i] = reader.ReadByte();

            x = reader.ReadUInt16();
            y = reader.ReadUInt16();
            buffer1 = reader.ReadByte();
            buffer2 = reader.ReadByte();
            name = reader.ReadUInt16();
        }

        public int[] ToOldArray()
        {
            int[] arr = new int[25];

            unknownBytes.CopyTo(arr, 0);
            arr[20] = x;
            arr[21] = y;
            arr[22] = buffer1;
            arr[23] = buffer2;
            arr[24] = name;

            return arr;
        }
    }
}
