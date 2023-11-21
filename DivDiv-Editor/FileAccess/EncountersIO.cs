using System;
using System.Collections.Generic;
using System.IO;

namespace DivDivEditor.FileAccess
{
    public static class EncountersIO
    {
        static List<byte> start = new();
        static List<byte> end = new();
        static int countEgg;

        // Читаем информацию о точках спавна
        public static List<int[]> ReadEggs(string inpFile)
        {
            start.Clear();
            end.Clear();
            bool eggsRead = false;
            string buff = "aaaaaa";
            List<int[]> eggs = new();

            if (File.Exists(inpFile))
            {
                byte[] buffer = File.ReadAllBytes(inpFile);
                Console.WriteLine(buffer.Length);
                for (long i = 0; i < buffer.Length; i++)
                {
                    if (!eggsRead) start.Add(buffer[i]);
                    buff = buff.Substring(1) + Convert.ToChar(buffer[i]);
                    if (buff == "EggsV0")
                    {
                        i += 17;
                        countEgg = buffer[i + 1] * 256 + buffer[i];
                        Console.WriteLine(countEgg);
                        i += 4;
                        for (int j = 0; j < countEgg; j++)
                        {
                            int a = 0;
                            eggs.Add(new int[15]);
                            for (int k = 0; k < 46; k++)
                            {
                                if (k < 19 && k != 1 && k != 3 && k != 5 && k != 7)
                                {
                                    eggs[j][a] = buffer[i + 1] * 256 + buffer[i];
                                    a++;
                                }
                                i++;
                                i++;
                            }
                        }
                        eggsRead = true;
                    }
                    if (eggsRead) end.Add(buffer[i]);
                }
            }
            start.RemoveRange(start.Count - 6, 6);

            return eggs;
        }

        // Записываем информацию о точках спавна
        public static void WriteEggs(string inpFile, List<int[]> eggs)
        {
            countEgg = eggs.Count;
            using BinaryWriter writer = new(File.Open(inpFile, FileMode.Create));

            for (int i = 0; i < start.Count; i++)
            {
                writer.Write(start[i]);
            }

            byte[] rawData = { 0x45, 0x67, 0x67, 0x73, 0x56, 0x30, 0x2E, 0x39, 0x33, 0x35, 0x20, 0x32, 0x35, 0x2D, 0x30, 0x32, 0x2D, 0x32, 0x30, 0x30, 0x32 };
            writer.Write(rawData);
            writer.Write((byte)0);
            writer.Write(countEgg);

            for (int i = 0; i < countEgg; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (j != 12) writer.Write((ushort)eggs[i][j]);
                    else writer.Write((ushort)i);
                    if (j < 4) writer.Write((ushort)0);

                }
                for (int j = 0; j < 54; j++) writer.Write((byte)0);
            }

            for (int i = 0; i < end.Count; i++)
            {
                writer.Write(end[i]);
            }
        }
    }
}
