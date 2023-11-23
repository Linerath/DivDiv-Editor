using System;
using System.Collections.Generic;
using System.IO;

namespace DivDivEditor.FileAccess
{
    public static class EncountersIO
    {
        static List<byte> start = new();
        static List<byte> end = new();
        static int encountersCount;

        public static List<int[]> ReadEncounters(string inputFile)
        {
            start.Clear();
            end.Clear();
            bool encountersRead = false;
            string buff = "aaaaaa";
            List<int[]> encounters = new();

            if (File.Exists(inputFile))
            {
                byte[] buffer = File.ReadAllBytes(inputFile);
                Console.WriteLine(buffer.Length);
                for (long i = 0; i < buffer.Length; i++)
                {
                    if (!encountersRead) start.Add(buffer[i]);
                    buff = buff.Substring(1) + Convert.ToChar(buffer[i]);
                    if (buff == "EggsV0")
                    {
                        i += 17;
                        encountersCount = buffer[i + 1] * 256 + buffer[i];
                        Console.WriteLine(encountersCount);
                        i += 4;
                        for (int j = 0; j < encountersCount; j++)
                        {
                            int a = 0;
                            encounters.Add(new int[15]);
                            for (int k = 0; k < 46; k++)
                            {
                                if (k < 19 && k != 1 && k != 3 && k != 5 && k != 7)
                                {
                                    encounters[j][a] = buffer[i + 1] * 256 + buffer[i];
                                    a++;
                                }
                                i++;
                                i++;
                            }
                        }
                        encountersRead = true;
                    }
                    if (encountersRead) end.Add(buffer[i]);
                }
            }
            start.RemoveRange(start.Count - 6, 6);

            return encounters;
        }

        public static void WriteEncounters(string inputFile, List<int[]> encounters)
        {
            encountersCount = encounters.Count;
            using BinaryWriter writer = new(File.Open(inputFile, FileMode.Create));

            for (int i = 0; i < start.Count; i++)
            {
                writer.Write(start[i]);
            }

            byte[] rawData = { 0x45, 0x67, 0x67, 0x73, 0x56, 0x30, 0x2E, 0x39, 0x33, 0x35, 0x20, 0x32, 0x35, 0x2D, 0x30, 0x32, 0x2D, 0x32, 0x30, 0x30, 0x32 };
            writer.Write(rawData);
            writer.Write((byte)0);
            writer.Write(encountersCount);

            for (int i = 0; i < encountersCount; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (j != 12) writer.Write((ushort)encounters[i][j]);
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
