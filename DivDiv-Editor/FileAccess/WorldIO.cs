using System;
using System.IO;

namespace DivDivEditor.FileAccess
{
    public static class WorldIO
    {
        public static void WriteWorld(string inpFile, int[,,] outArray)
        {
            using BinaryWriter writer = new(File.Open(inpFile, FileMode.Create));

            int bufStart = 4096;
            writer.Write(4096);

            for (int i = 0; i < 1023; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    bufStart += 18 + outArray[i, j, 5] * 8;
                }
                writer.Write(bufStart);
            }

            ushort bufStartSmall = 0;

            for (int i = 0; i < 1024; i++)
            {
                writer.Write(Convert.ToUInt16(0));

                for (int j = 0; j < 511; j++)
                {
                    bufStartSmall += Convert.ToUInt16(16 + outArray[i, j, 5] * 8);
                    writer.Write(bufStartSmall);
                }

                bufStartSmall = 0;

                for (int j = 0; j < 512; j++)
                {
                    writer.Write(Convert.ToUInt16(outArray[i, j, 0]));
                    writer.Write(Convert.ToUInt16(outArray[i, j, 1]));
                    writer.Write(Convert.ToUInt16(0));
                    writer.Write(Convert.ToByte(outArray[i, j, 5]));
                    writer.Write(Convert.ToUInt16(outArray[i, j, 2]));
                    writer.Write((byte)0);
                    writer.Write(Convert.ToUInt16(outArray[i, j, 3]));
                    writer.Write(Convert.ToUInt16(outArray[i, j, 4]));
                    writer.Write(Convert.ToUInt16(0));

                    for (int k = 0; k < outArray[i, j, 5]; k++)
                    {
                        int XY = outArray[i, j, 6 * k + 7] * 64 + outArray[i, j, 6 * k + 6];
                        int Y = XY / 256;
                        int X = XY % 256;
                        int c = outArray[i, j, 6 * k + 10] / 4096;
                        int b = (outArray[i, j, 6 * k + 10] - c * 4096) / 16;
                        int a = (outArray[i, j, 6 * k + 10] - c * 4096) - b * 16;
                        a = a * 16 + Y;
                        int e = outArray[i, j, 6 * k + 9] / 64;
                        int d = (outArray[i, j, 6 * k + 9] % 64) * 4;
                        writer.Write((byte)X);
                        writer.Write((byte)a);
                        writer.Write((byte)b);
                        writer.Write((byte)c);
                        writer.Write((byte)outArray[i, j, 6 * k + 8]);
                        writer.Write((byte)d);
                        writer.Write((byte)e);
                        writer.Write((byte)outArray[i, j, 6 * k + 11]);
                    }
                }
            }

            writer.Write(103);
        }

        public static int[,,] ReadWorld(string inpFile)
        {
            int[,,] tileArray = new int[Vars.WorldHeight, Vars.WorldWidth, 96];
            byte[] tile = new byte[9];
            byte[] object_ = new byte[8];
            int XY, buf1;

            if (File.Exists(inpFile))
            {
                using BinaryReader world = new(File.Open(inpFile, FileMode.Open));

                var globalHash = world.ReadBytes(Vars.GlobalHashBytes);

                for (int y = 0; y < Vars.WorldHeight; y++)
                {
                    var rowHash = world.ReadBytes(Vars.RowHashBytes);

                    for (int x = 0; x < Vars.WorldWidth; x++)
                    {
                        tileArray[y, x, 0] = world.ReadUInt16(); //Полные текстуры
                        tileArray[y, x, 1] = world.ReadUInt16(); //Половинчатые текстуры
                        buf1 = world.ReadUInt16();
                        tileArray[y, x, 5] = world.ReadByte(); //Количество объектов
                        tileArray[y, x, 2] = world.ReadUInt16(); //Эффекты плитки
                        buf1 = world.ReadByte();
                        tileArray[y, x, 3] = world.ReadUInt16(); //var1
                        tileArray[y, x, 4] = world.ReadUInt16(); //var2
                        buf1 = world.ReadUInt16();

                        for (int i = 0; i < tileArray[y, x, 5]; i++)
                        {
                            object_ = world.ReadBytes(8);
                            XY = object_[0] + object_[1] % 16 * 4 * 64;
                            tileArray[y, x, 6 * i + 6] = XY % 64; //Х кор
                            tileArray[y, x, 6 * i + 7] = XY / 64; //Y кор
                            tileArray[y, x, 6 * i + 10] = object_[1] / 16 + object_[2] * 16 + object_[3] * 4096; //Номер
                            tileArray[y, x, 6 * i + 8] = object_[4]; //Высота
                            tileArray[y, x, 6 * i + 9] = object_[5] / 4 + object_[6] * 64; //Имя
                            tileArray[y, x, 6 * i + 11] = object_[7]; //var3
                        }
                    }
                }
            }

            return tileArray;
        }
    }
}
