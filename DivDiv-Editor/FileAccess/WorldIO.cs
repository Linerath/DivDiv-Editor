using System;
using System.IO;
using DivDivEditor.GameObjects;

namespace DivDivEditor.FileAccess
{
    public static class WorldIO
    {
        public static void WriteWorld(string inputFile, int[,,] outArray)
        {
            using BinaryWriter writer = new(File.Open(inputFile, FileMode.Create));

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

        public static int[,,] ReadWorld(string inputFile)
        {
            int[,,] tileArray = new int[Vars.WorldHeight, Vars.WorldWidth, 96];

            if (File.Exists(inputFile))
            {
                using BinaryReader world = new(File.Open(inputFile, FileMode.Open));

                var globalHash = world.ReadBytes(Vars.GlobalHashBytes);

                for (int tileY = 0; tileY < Vars.WorldHeight; tileY++)
                {
                    var rowHash = world.ReadBytes(Vars.RowHashBytes);

                    for (int tileX = 0; tileX < Vars.WorldWidth; tileX++)
                    {
                        tileArray[tileY, tileX, 0] = world.ReadUInt16(); //Полные текстуры
                        tileArray[tileY, tileX, 1] = world.ReadUInt16(); //Половинчатые текстуры
                        var buf1 = world.ReadUInt16();
                        tileArray[tileY, tileX, 5] = world.ReadByte(); //Количество объектов
                        tileArray[tileY, tileX, 2] = world.ReadUInt16(); //Эффекты плитки
                        buf1 = world.ReadByte();
                        tileArray[tileY, tileX, 3] = world.ReadUInt16(); //var1
                        tileArray[tileY, tileX, 4] = world.ReadUInt16(); //var2
                        buf1 = world.ReadUInt16();

                        for (int i = 0; i < tileArray[tileY, tileX, 5]; i++)
                        {
                            var tempX = world.ReadByte();
                            var tempY = world.ReadByte();
                            var id2 = world.ReadByte();
                            var id3 = world.ReadByte();
                            var height = world.ReadByte();
                            var name1 = world.ReadByte();
                            var name2 = world.ReadByte();
                            var effect = world.ReadByte();

                            var objXY = tempX + tempY % 16 * 4 * 64;

                            var objX = objXY % 64;
                            var objY = objXY / 64;
                            var objNum = tempY / 16 + id2 * 16 + id3 * 4096;
                            var objName = name1 / 4 + name2 * 64;

                            tileArray[tileY, tileX, 6 * i + 6] = objX; //Х кор
                            tileArray[tileY, tileX, 6 * i + 7] = objY; //Y кор
                            tileArray[tileY, tileX, 6 * i + 10] = objNum; //Номер
                            tileArray[tileY, tileX, 6 * i + 8] = height; //Высота
                            tileArray[tileY, tileX, 6 * i + 9] = objName; //Имя
                            tileArray[tileY, tileX, 6 * i + 11] = effect; //var3
                        }
                    }
                }
            }

            return tileArray;
        }
    }
}
