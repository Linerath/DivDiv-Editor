using System;
using System.Collections.Generic;
using System.IO;
using DivDivEditor.GameObjects;

namespace DivDivEditor.FileAccess
{
    public static class ObjectsIO
    {
        private static ObjectsInfo[] objectsInfo = new ObjectsInfo[11264]; //Массив объектов класса ObjectsInfo с описанием объектов

        public static List<Placeable> ReadPlaceables(string inputFile)
        {
            FileInfo fileInfo = new(inputFile);
            var objectsCount = (int)(fileInfo.Length / 28);
            List<Placeable> objects = new(objectsCount);

            using BinaryReader reader = new(File.Open(inputFile, FileMode.Open));

            for (int i = 0; i < objectsCount; i++)
            {
                var obj = new Placeable(reader);
                objects.Add(obj);
            }

            return objects;
        }

        public static int ReadObjectsCount(string inputFile)
        {
            long objCount = 0;

            if (File.Exists(inputFile))
            {
                FileInfo fileInfo = new(inputFile);
                objCount = fileInfo.Length / 28;
            }

            return (int)objCount;
        }

        public static int[,] ReadObjects(string inputFile, int maxObjectCount)
        {
            int[,] objects = new int[maxObjectCount, 25];

            if (File.Exists(inputFile))
            {
                FileInfo fileInfo = new(inputFile);
                long objCount = fileInfo.Length / 28;

                using BinaryReader obj = new(File.Open(inputFile, FileMode.Open));

                for (long i = 0; i < objCount; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        objects[i, j] = obj.ReadByte();
                    }
                    objects[i, 20] = obj.ReadUInt16();
                    objects[i, 21] = obj.ReadUInt16();
                    objects[i, 22] = obj.ReadByte();
                    objects[i, 23] = obj.ReadByte();
                    objects[i, 24] = obj.ReadUInt16();
                }
            }

            return objects;
        }

        public static void WriteObjects(string inputFile, int[,] outArray, int objCount)
        {
            using BinaryWriter writer = new(File.Open(inputFile, FileMode.Create));

            for (int i = 0; i < objCount; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    writer.Write((byte)outArray[i, j]);
                }

                writer.Write(Convert.ToUInt16(outArray[i, 20]));
                writer.Write(Convert.ToUInt16(outArray[i, 21]));
                writer.Write((byte)outArray[i, 22]);
                writer.Write((byte)outArray[i, 23]);
                writer.Write(Convert.ToUInt16(outArray[i, 24]));
            }
        }

        // Записываем файл World
        public static void WriteObjects2(string inputFile, List<int[]> obj)
        {
            using BinaryWriter writer = new(File.Open(inputFile, FileMode.Create));

            for (int i = 0; i < obj.Count; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    writer.Write((byte)obj[i][j]);
                }

                writer.Write(Convert.ToUInt16(obj[i][20]));
                writer.Write(Convert.ToUInt16(obj[i][21]));
                writer.Write((byte)obj[i][22]);
                writer.Write((byte)obj[i][23]);
                writer.Write(Convert.ToUInt16(obj[i][24]));
            }
        }

        public static ObjectsInfo[] ReadObjectsInfo(string inputFile)
        {
            int a = 0;
            string line;

            using StreamReader reader = new(inputFile);

            while ((line = reader.ReadLine()) != null)
            {
                objectsInfo[a] = new ObjectsInfo();
                string[] words = line.Split(new char[] { '|' });
                objectsInfo[a].name = words[0];

                int.TryParse(words[1], out objectsInfo[a].width);
                int.TryParse(words[2], out objectsInfo[a].height);
                int.TryParse(words[3], out objectsInfo[a].touchPointX);
                int.TryParse(words[4], out objectsInfo[a].touchPointY);
                int.TryParse(words[5], out objectsInfo[a].var_1);
                int.TryParse(words[6], out objectsInfo[a].var_2);
                int.TryParse(words[7], out objectsInfo[a].var_3);

                objectsInfo[a].TP.X = objectsInfo[a].touchPointX;
                objectsInfo[a].TP.Y = objectsInfo[a].touchPointY;
                objectsInfo[a].SP0.X = 0;
                objectsInfo[a].SP0.Y = objectsInfo[a].height;
                objectsInfo[a].SP2.Y = objectsInfo[a].height - (objectsInfo[a].height - objectsInfo[a].touchPointY) * 2;
                objectsInfo[a].SP2.X = objectsInfo[a].touchPointX * 2;
                objectsInfo[a].SP3.Y = objectsInfo[a].height;
                objectsInfo[a].SP3.X = objectsInfo[a].touchPointX * 2 - (objectsInfo[a].SP0.Y - objectsInfo[a].SP2.Y);
                objectsInfo[a].SP1.Y = objectsInfo[a].SP2.Y;
                objectsInfo[a].SP1.X = objectsInfo[a].touchPointX * 2 - objectsInfo[a].SP3.X;

                a++;
            }

            return objectsInfo;
        }
    }
}
