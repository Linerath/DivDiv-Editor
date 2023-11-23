using System.Collections.Generic;
using System.IO;
using System.Linq;
using DivDivEditor.GameObjects;

namespace DivDivEditor.FileAccess
{
    public static class TerrainIO
    {
        // Читаем информацию о текстурах
        public static List<Terrain> ReadTerrain(string inputFile)
        {
            bool terrain = true;
            int terCount = -1;
            string line;
            string[] words;
            List<Terrain> ter = new();

            using StreamReader reader = new(inputFile);

            while ((line = reader.ReadLine()) != null && terrain)
            {
                words = line.Split(new char[] { ' ' });

                if (words[0] == "endsection" && words[1] == "terrain") terrain = false;
                if (words[0] == "startdef" && words[1] == "terrain")
                {
                    terCount++;
                    ter.Add(new Terrain());
                    ter[terCount].SetTerrain(words[2]);

                }
                if (words[0] == "transition") ter[terCount].SetTransition(words[1]);
                if (words[0] == "system") ter[terCount].SetSystem(int.Parse(words[1]));
                if (words[0] == "tile" && words[1] == "base") ter[terCount].AddBaseTile(int.Parse(words[3]));
                if (words[0] == "tile" && words[1] == "transition") ter[terCount].AddTrns(int.Parse(words[3]), int.Parse(words[5]));
            }

            return ter;
        }

        // Читаем информацию о текстурах
        public static List<Metaobject> ReadMetaobject(string inputFile)
        {
            bool metaobject = false;
            int metCount = -1;
            string line;
            string[] words;
            List<Metaobject> met = new();

            using StreamReader reader = new(inputFile);

            while ((line = reader.ReadLine()) != null && !metaobject)
            {
                words = line.Split(new char[] { ' ' });
                if (words[0] == "startsection" && words[1] == "metaobjects") metaobject = true;
            }

            while ((line = reader.ReadLine()) != null && metaobject)
            {
                words = line.Split(new char[] { ' ' });
                if (words[0] == "endsection" && words[1] == "metaobjects") metaobject = false;
                if (words[0] == "startdef" && words[1] == "metaobject")
                {
                    metCount++;
                    met.Add(new Metaobject());
                    if (words.Count() > 3) met[metCount].setMet(words[2] + " " + words[3]);
                    else met[metCount].setMet(words[2]);

                }
                if (words[0] == "group") met[metCount].setGroup(words[1]);
                if (words[0] == "type") met[metCount].setType(words[1]);
                if (words[0] == "location") met[metCount].setLocation(words[1]);
                if (words[0] == "walltype") met[metCount].setWalltype(words[1] + " " + words[2]);
                //if (words[0] == "placement") met[metCount].setPlacement(int.Parse(words[2]));
                if (words[0] == "size")
                {
                    met[metCount].setSize(int.Parse(words[1]), int.Parse(words[2]));
                }
                if (words[0] == "object")
                {
                    int[] buff = new int[4];
                    int a = 0;
                    for (int i = 0; i < buff.Length; i++)
                    {
                        if (int.TryParse(words[i], out int b))
                        {
                            buff[a] = b;
                            a++;
                        }
                    }
                    met[metCount].addObject(buff);
                }
            }

            return met;
        }
    }
}
