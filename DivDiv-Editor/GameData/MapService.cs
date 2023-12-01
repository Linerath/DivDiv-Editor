#define OBJECTS

using System.Collections.Generic;
using System.Linq;
using DivDivEditor.FileAccess;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DivDivEditor.GameObjects
{
    public class MapService
    {
        private List<Terrain> terrain = new();

        public int[,,] Tiles { get; private set; }
        public Tile[,] TilesNew { get; private set; }

#if OBJECTS
        public List<int[]> Objects { get; private set; }
        public List<Placeable> ObjectsNew { get; private set; }

        private bool objectCopy;
        private int cursorOffsetX;
        private int cursorOffsetY;
        private List<int[]> objectsInFrame = new();                         // [9] Имя, плитка по X, плитка по Y, номер на плитке, положение по X, положение по Y, высота, мировая координата, номер 
        private List<int> obgSort = new();                                  // Сортировка объектов 
        private static ObjectsInfo[] objectsInfo = new ObjectsInfo[11264];  // Массив объектов класса ObjectsInfo с описанием объектов
        private int[] buffObjectCopy = new int[35];                         //Буфферный массив для копирования или перемещения объекта
#endif

        public void Initialize(string worldFile, string objectsFile)
        {
            var map = WorldIO.ReadWorldMap(worldFile);

            TilesNew = map.Tiles;
            Tiles = map.ToOldTilesArray();

            terrain = TerrainIO.ReadTerrain(Settings.EditorFile);

#if OBJECTS
            ObjectsNew = ObjectsIO.ReadPlaceables(objectsFile);
            Objects = ObjectsNew.Select(x => x.ToOldArray()).ToList();

            objectsInfo = ObjectsIO.ReadObjectsInfo(@"objects.de");
#endif
        }

        public string GetTileTextureName(int x, int y, bool bottomTexture)
        {
            var value = bottomTexture
                ? TilesNew[y, x].BottomTexture
                : TilesNew[y, x].TopTexture;
            var name = AssetsHelper.GetTileTextureName(value);

            return name;
        }

        public int[] GetTile(int x, int y)
        {
            int[] tile = new int[]
            {
                Tiles[y, x, 0],
                Tiles[y, x, 1],
                Tiles[y, x, 2],
                Tiles[y, x, 3],
                Tiles[y, x, 4]
            };

            return tile;
        }

        public void SetTile(int[] tile, int x, int y)
        {
            Tiles[y, x, 0] = tile[0];
            Tiles[y, x, 1] = tile[1];
            Tiles[y, x, 2] = tile[2];
            Tiles[y, x, 3] = tile[3];
            Tiles[y, x, 4] = tile[4];
        }

        public int GetTileEffect(int x, int y)
        {
            return Tiles[y, x, 2];
        }

        public string[] GetTexturePalette(int textures)
        {
            string[] tex = new string[4];

            if (terrain[textures].baseTile[0] > 0 && terrain[textures].baseTile[0] <= 9368)
            {
                tex[0] = "floor/" + terrain[textures].baseTile[0].ToString().PadLeft(6, '0');
                tex[1] = "floor/" + terrain[textures].baseTile[0].ToString().PadLeft(6, '0');
                tex[2] = "floor/" + terrain[textures].baseTile[0].ToString().PadLeft(6, '0');
                tex[3] = "floor/" + terrain[textures].baseTile[0].ToString().PadLeft(6, '0');
            }

            for (int i = 0; i < terrain[textures].baseTile.Count && i < 4; i++)
            {
                tex[i] = "floor/" + terrain[textures].baseTile[i].ToString().PadLeft(6, '0');
            }

            return tex;
        }

        public void TextureMapping(int textures, int MouseStateX, int MouseStateY, int xCor, int yCor, bool KeyLeftShift, bool KeyLeftControl)
        {
            int quarter = 0;
            int y = MouseStateY / Vars.TileSize + yCor;
            int x = MouseStateX / Vars.TileSize + xCor;
            int[,] tilesNew = new int[3, 3];
            int[,] tileFilling = new int[3, 3];

            if (MouseStateY % 64 < 32 && MouseStateX % 64 < 32) quarter = 0;
            if (MouseStateY % 64 < 32 && MouseStateX % 64 > 32) quarter = 1;
            if (MouseStateY % 64 > 32 && MouseStateX % 64 > 32) quarter = 2;
            if (MouseStateY % 64 > 32 && MouseStateX % 64 < 32) quarter = 3;

            if (terrain[textures].transition != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        tilesNew[i, j] = Tiles[y + i - 1, x + j - 1, 0];
                        for (int k = 0; k < terrain[textures].baseTile.Count; k++)
                        {
                            if (tilesNew[i, j] == terrain[textures].baseTile[k]) tileFilling[i, j] = 1111;
                        }
                        for (int k = 0; k < 16; k++)
                        {
                            for (int t = 0; t < terrain[textures].trns[k].Count; t++)
                            {
                                if (tilesNew[i, j] == terrain[textures].trns[k][t])
                                {
                                    tileFilling[i, j] = k switch
                                    {
                                        1 => 1100,
                                        2 => 1000,
                                        3 => 1001,
                                        4 => 1,
                                        5 => 11,
                                        6 => 10,
                                        7 => 110,
                                        8 => 100,
                                        9 => 1101,
                                        10 => 1011,
                                        11 => 111,
                                        12 => 1110,
                                        14 => 101,
                                        15 => 1010,
                                        _ => 0,
                                    };
                                }
                            }
                        }
                    }
                }

                // |_0_|_1_|
                // |_3_|_2_|
                if (quarter == 0) tileFilling[1, 1] = (tileFilling[1, 1] / 10) * 10 + 1;
                if (quarter == 1) tileFilling[1, 1] = (tileFilling[1, 1] / 100) * 100 + tileFilling[1, 1] % 10 + 10;
                if (quarter == 2) tileFilling[1, 1] = (tileFilling[1, 1] / 1000) * 1000 + tileFilling[1, 1] % 100 + 100;
                if (quarter == 3) tileFilling[1, 1] = tileFilling[1, 1] % 1000 + 1000;

                if (tileFilling[1, 1] % 10 == 1) //0
                {
                    tileFilling[1, 0] = (tileFilling[1, 0] / 100) * 100 + tileFilling[1, 0] % 10 + 10;
                    tileFilling[0, 0] = (tileFilling[0, 0] / 1000) * 1000 + tileFilling[0, 0] % 100 + 100;
                    tileFilling[0, 1] = tileFilling[0, 1] % 1000 + 1000;
                }

                if ((tileFilling[1, 1] / 10) % 10 == 1) //1
                {
                    tileFilling[0, 1] = (tileFilling[0, 1] / 1000) * 1000 + tileFilling[0, 1] % 100 + 100;
                    tileFilling[0, 2] = tileFilling[0, 2] % 1000 + 1000;
                    tileFilling[1, 2] = (tileFilling[1, 2] / 10) * 10 + 1;
                }

                if ((tileFilling[1, 1] / 100) % 10 == 1) //2
                {
                    tileFilling[1, 2] = tileFilling[1, 2] % 1000 + 1000;
                    tileFilling[2, 2] = (tileFilling[2, 2] / 10) * 10 + 1;
                    tileFilling[2, 1] = (tileFilling[2, 1] / 100) * 100 + tileFilling[2, 1] % 10 + 10;
                }

                if ((tileFilling[1, 1] / 1000) % 10 == 1) //3
                {
                    tileFilling[2, 1] = (tileFilling[2, 1] / 10) * 10 + 1;
                    tileFilling[2, 0] = (tileFilling[2, 0] / 100) * 100 + tileFilling[2, 0] % 10 + 10;
                    tileFilling[1, 0] = (tileFilling[1, 0] / 1000) * 1000 + tileFilling[1, 0] % 100 + 100;
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (tileFilling[i, j] == 1111)
                        {
                            bool a = false;
                            for (int k = 0; k < terrain[textures].baseTile.Count; k++)
                            {
                                if (tilesNew[i, j] == terrain[textures].baseTile[k]) a = true;
                            }
                            if (!a) tilesNew[i, j] = terrain[textures].GetBaseTile();
                        }
                        else
                        {
                            switch (tileFilling[i, j])
                            {
                                case 1100: { int a = terrain[textures].GetTrns(1); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1000: { int a = terrain[textures].GetTrns(2); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1001: { int a = terrain[textures].GetTrns(3); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1: { int a = terrain[textures].GetTrns(4); if (a != 0) tilesNew[i, j] = a; break; }
                                case 11: { int a = terrain[textures].GetTrns(5); if (a != 0) tilesNew[i, j] = a; break; }
                                case 10: { int a = terrain[textures].GetTrns(6); if (a != 0) tilesNew[i, j] = a; break; }
                                case 110: { int a = terrain[textures].GetTrns(7); if (a != 0) tilesNew[i, j] = a; break; }
                                case 100: { int a = terrain[textures].GetTrns(8); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1101: { int a = terrain[textures].GetTrns(9); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1011: { int a = terrain[textures].GetTrns(10); if (a != 0) tilesNew[i, j] = a; break; }
                                case 111: { int a = terrain[textures].GetTrns(11); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1110: { int a = terrain[textures].GetTrns(12); if (a != 0) tilesNew[i, j] = a; break; }
                                case 101: { int a = terrain[textures].GetTrns(14); if (a != 0) tilesNew[i, j] = a; break; }
                                case 1010: { int a = terrain[textures].GetTrns(15); if (a != 0) tilesNew[i, j] = a; break; }
                            }
                        }
                        Tiles[y + i - 1, x + j - 1, 0] = tilesNew[i, j];
                    }
                }
            }
            else
            {
                if (!KeyLeftShift)
                {
                    if (!KeyLeftControl)
                    {
                        bool a = false;
                        for (int k = 0; k < terrain[textures].baseTile.Count; k++)
                        {
                            if (Tiles[y, x, 0] == terrain[textures].baseTile[k]) a = true;
                        }
                        if (!a) Tiles[y, x, 0] = terrain[textures].GetBaseTile();
                    }
                    else
                    {
                        Tiles[y, x, 0] = terrain[textures].GetBaseTile();
                    }
                }
                else
                if (!KeyLeftControl)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bool a = false;
                            for (int k = 0; k < terrain[textures].baseTile.Count; k++)
                            {
                                if (Tiles[y + i - 1, x + j - 1, 0] == terrain[textures].baseTile[k]) a = true;
                            }
                            if (!a) Tiles[y + i - 1, x + j - 1, 0] = terrain[textures].GetBaseTile();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Tiles[y + i - 1, x + j - 1, 0] = terrain[textures].GetBaseTile();
                        }
                    }
                }
            }
        }

#if OBJECTS
        public void SetObject(int n, int[] obj)
        {
            Objects[n] = obj;
        }

        public int[] GetObject(int num)
        {
            return num < Objects.Count
                ? Objects[num]
                : Objects[0];
        }

        public void UpdateDisplayedObjects(int x, int y, int width, int height, bool moving)
        {
            objectsInFrame.Clear();
            obgSort.Clear();

            for (int i = -5; i < width / Vars.TileSize; i++)
            {
                for (int j = -5; j < height / Vars.TileSize + 1; j++)
                {
                    if (j + y >= 0 && i + x >= 0)
                    {
                        for (int z = 0; z < Tiles[j + y, i + x, 5]; z++)
                        {
                            objectsInFrame.Add(new int[9]);
                            int objCount = objectsInFrame.Count - 1;
                            obgSort.Add(objCount);
                            objectsInFrame[objCount][0] = Tiles[j + y, i + x, 9 + z * 6];                                   // Имя
                            objectsInFrame[objCount][1] = i + x;                                                            // плитка по X
                            objectsInFrame[objCount][2] = j + y;                                                            // плитка по Y
                            objectsInFrame[objCount][3] = z;                                                                // номер на плитке
                            objectsInFrame[objCount][4] = i * 64 + Tiles[j + y, i + x, 6 + z * 6];                          // положение по X
                            objectsInFrame[objCount][5] = j * 64 + Tiles[j + y, i + x, 7 + z * 6];                          // положение по Y
                            objectsInFrame[objCount][6] = Tiles[j + y, i + x, 8 + z * 6];                                   // высота
                            int Y1 = objectsInFrame[objCount][5] + objectsInfo[objectsInFrame[objCount][0]].touchPointY;
                            int X1 = objectsInFrame[objCount][4] + objectsInfo[objectsInFrame[objCount][0]].touchPointX;
                            objectsInFrame[objCount][7] = Y1 * width + X1;                                                  // мировая координата
                            objectsInFrame[objCount][8] = Tiles[j + y, i + x, 10 + z * 6];                                  // номер
                        }
                    }
                }
            }

            if (moving)
            {
                objectsInFrame.Add(new int[9]);
                int objCount = objectsInFrame.Count - 1;
                obgSort.Add(objCount);
                objectsInFrame[objCount][0] = buffObjectCopy[3];
                objectsInFrame[objCount][1] = 0;
                objectsInFrame[objCount][2] = 0;
                objectsInFrame[objCount][3] = 0;
                objectsInFrame[objCount][4] = buffObjectCopy[6];
                objectsInFrame[objCount][5] = buffObjectCopy[7];
                objectsInFrame[objCount][6] = buffObjectCopy[2];
                int Y1 = objectsInFrame[objCount][5] + objectsInfo[objectsInFrame[objCount][0]].touchPointY;
                int X1 = objectsInFrame[objCount][4] + objectsInfo[objectsInFrame[objCount][0]].touchPointX;
                objectsInFrame[objCount][7] = Y1 * width + X1;
                objectsInFrame[objCount][8] = 0;
            }

            objectsInFrame.Sort((b1, b2) => Sort(b1[7], b2[7]));
        }

        private static int Sort(int a, int b)
        {
            if (a < b) return -1;
            else if (a > b) return 1;
            else if (a == 0) return 0;
            else return 0;
        }

        public int GetObjectCount()
        {
            return objectsInFrame.Count;
        }

        public int[] GetObjectInFrame(int num)
        {
            return num < objectsInFrame.Count
                ? objectsInFrame[num]
                : objectsInFrame[0];
        }

        public string GetObjectPath(int n)
        {
            return n < objectsInFrame.Count
                ? "objects/" + objectsInFrame[n][0].ToString().PadLeft(6, '0')
                : "objects/001097";
        }

        public int GetObjectName(int n)
        {
            return n < objectsInFrame.Count
                ? objectsInFrame[n][0]
                : 0;
        }

        public Vector2 GetObjectPosition(int n)
        {
            return n < objectsInFrame.Count
                ? new Vector2(objectsInFrame[n][4], objectsInFrame[n][5] - objectsInFrame[n][6])
                : new Vector2(0, 0);
        }

        public int ObjectDel(int selectedObject)
        {
            int x = objectsInFrame[selectedObject][1];
            int y = objectsInFrame[selectedObject][2];
            int z = objectsInFrame[selectedObject][3];
            int num = objectsInFrame[selectedObject][8];

            for (int i = z; i < Tiles[y, x, 5]; i++)
            {
                Tiles[y, x, 6 + i * 6] = Tiles[y, x, 6 + (i + 1) * 6];
                Tiles[y, x, 7 + i * 6] = Tiles[y, x, 7 + (i + 1) * 6];
                Tiles[y, x, 8 + i * 6] = Tiles[y, x, 8 + (i + 1) * 6];
                Tiles[y, x, 9 + i * 6] = Tiles[y, x, 9 + (i + 1) * 6];
                Tiles[y, x, 10 + i * 6] = Tiles[y, x, 10 + (i + 1) * 6];
                Tiles[y, x, 11 + i * 6] = Tiles[y, x, 11 + (i + 1) * 6];
            }

            Tiles[y, x, 5]--;

            for (int i = 0; i < 25; i++)
            {
                Objects[num][i] = 255;
            }

            Objects[num][20] = 65535;
            Objects[num][21] = 65535;
            Objects[num][24] = 65535;

            return -1;
        }

        public bool StartMovingAnObject(int selectedObject, int mousX, int mousY, int xCor, int yCor, bool Ctrl)
        {

            //Если курсор в области выделенного объекта
            if (mousX > objectsInFrame[selectedObject][4] &&
                mousX < objectsInFrame[selectedObject][4] + objectsInfo[objectsInFrame[selectedObject][0]].width &&
                mousY > objectsInFrame[selectedObject][5] - objectsInFrame[selectedObject][6] &&
                mousY < objectsInFrame[selectedObject][5] + objectsInfo[objectsInFrame[selectedObject][0]].height - objectsInFrame[selectedObject][6])
            {
                cursorOffsetX = mousX - objectsInFrame[selectedObject][4];
                cursorOffsetY = mousY - objectsInFrame[selectedObject][5];

                int x = objectsInFrame[selectedObject][1];
                int y = objectsInFrame[selectedObject][2];
                int z = objectsInFrame[selectedObject][3];

                buffObjectCopy[0] = Tiles[y, x, 6 + z * 6];
                buffObjectCopy[1] = Tiles[y, x, 7 + z * 6];
                buffObjectCopy[2] = Tiles[y, x, 8 + z * 6];
                buffObjectCopy[3] = Tiles[y, x, 9 + z * 6];
                buffObjectCopy[4] = Tiles[y, x, 10 + z * 6];
                buffObjectCopy[5] = Tiles[y, x, 11 + z * 6];
                buffObjectCopy[6] = xCor * 64 + mousX - cursorOffsetX;
                buffObjectCopy[7] = yCor * 64 + mousY - cursorOffsetY;
                buffObjectCopy[8] = selectedObject;

                for (int i = 0; i < 25; i++)
                {
                    buffObjectCopy[9 + i] = Objects[buffObjectCopy[4]][i];
                }

                if (Ctrl)
                {
                    buffObjectCopy[4] = Objects.Count;
                    objectCopy = true;
                }
                else
                {
                    for (int i = z; i < Tiles[y, x, 5]; i++)
                    {
                        Tiles[y, x, 6 + i * 6] = Tiles[y, x, 6 + (i + 1) * 6];
                        Tiles[y, x, 7 + i * 6] = Tiles[y, x, 7 + (i + 1) * 6];
                        Tiles[y, x, 8 + i * 6] = Tiles[y, x, 8 + (i + 1) * 6];
                        Tiles[y, x, 9 + i * 6] = Tiles[y, x, 9 + (i + 1) * 6];
                        Tiles[y, x, 10 + i * 6] = Tiles[y, x, 10 + (i + 1) * 6];
                        Tiles[y, x, 11 + i * 6] = Tiles[y, x, 11 + (i + 1) * 6];
                    }
                    Tiles[y, x, 5]--;
                    objectCopy = false;
                }

                return true;
            }
            else
            {
                cursorOffsetX = 0;
                cursorOffsetY = 0;

                return false;
            }
        }

        public void MovingAnObject(bool Shift, int mouseX, int mouseY)
        {
            if (Shift)
            {
                buffObjectCopy[6] = ((mouseX - cursorOffsetX) / 64) * 64 + buffObjectCopy[0];
                buffObjectCopy[7] = ((mouseY - cursorOffsetY) / 64) * 64 + buffObjectCopy[1];
            }
            else
            {
                buffObjectCopy[6] = mouseX - cursorOffsetX;
                buffObjectCopy[7] = mouseY - cursorOffsetY;
            }
        }

        public bool PasteObjectAfterMove(int xCor, int yCor, int mouseX, int mouseY, bool Shift)
        {
            int insertionCorX = xCor + (mouseX - cursorOffsetX) / 64;
            int insertionCorY = yCor + (mouseY - cursorOffsetY) / 64;
            int objCountInTile = Tiles[insertionCorY, insertionCorX, 5];

            if (objCountInTile < 15)
            {
                if (Shift)
                {
                    Tiles[insertionCorY, insertionCorX, 6 + objCountInTile * 6] = buffObjectCopy[0];
                    Tiles[insertionCorY, insertionCorX, 7 + objCountInTile * 6] = buffObjectCopy[1];
                }
                else
                {
                    Tiles[insertionCorY, insertionCorX, 6 + objCountInTile * 6] = (mouseX - cursorOffsetX) % 64;
                    Tiles[insertionCorY, insertionCorX, 7 + objCountInTile * 6] = (mouseY - cursorOffsetY) % 64;
                }

                Tiles[insertionCorY, insertionCorX, 8 + objCountInTile * 6] = buffObjectCopy[2];
                Tiles[insertionCorY, insertionCorX, 9 + objCountInTile * 6] = buffObjectCopy[3];
                Tiles[insertionCorY, insertionCorX, 10 + objCountInTile * 6] = buffObjectCopy[4];
                Tiles[insertionCorY, insertionCorX, 11 + objCountInTile * 6] = buffObjectCopy[5];
                Tiles[insertionCorY, insertionCorX, 5]++;

                if (objectCopy)
                {
                    Objects.Add(new int[25]);
                    for (int k = 0; k < 25; k++)
                    {
                        Objects[^1][k] = buffObjectCopy[9 + k];
                    }
                    Objects[^1][20] = insertionCorX * 64 + Tiles[insertionCorY, insertionCorX, 6 + objCountInTile * 6];
                    Objects[^1][21] = insertionCorY * 64 + Tiles[insertionCorY, insertionCorX, 7 + objCountInTile * 6];
                }
                else
                {
                    Objects[buffObjectCopy[4]][20] = insertionCorX * 64 + Tiles[insertionCorY, insertionCorX, 6 + objCountInTile * 6];
                    Objects[buffObjectCopy[4]][21] = insertionCorY * 64 + Tiles[insertionCorY, insertionCorX, 7 + objCountInTile * 6];
                }
                objectCopy = false;
            }

            return false;
        }

        public void ChangingHeightObject(int selObj, bool direction)
        {
            if (direction && Tiles[objectsInFrame[selObj][2], objectsInFrame[selObj][1], 8 + objectsInFrame[selObj][3] * 6] < 255) Tiles[objectsInFrame[selObj][2], objectsInFrame[selObj][1], 8 + objectsInFrame[selObj][3] * 6]++;
            if (!direction && Tiles[objectsInFrame[selObj][2], objectsInFrame[selObj][1], 8 + objectsInFrame[selObj][3] * 6] > 0) Tiles[objectsInFrame[selObj][2], objectsInFrame[selObj][1], 8 + objectsInFrame[selObj][3] * 6]--;
        }
#endif
    }
}
