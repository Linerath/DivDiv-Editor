using System;
using System.Collections.Generic;

namespace DivDivEditor.GameObjects
{
    public class Terrain
    {
        public string terrain;
        public string transition;
        public int system;
        public List<int> baseTile = new();
        public List<int>[] trns = new List<int>[16];
        static int count = 0;

        public Terrain()
        {
            count++;
            for (int i = 0; i < 16; i++) trns[i] = new List<int>();
        }

        public void SetTerrain(string ter)
        {
            terrain = ter;
        }

        public void SetTransition(string trn)
        {
            transition = trn;
        }

        public void SetSystem(int sys)
        {
            system = sys;
        }

        public void AddBaseTile(int tile)
        {
            baseTile.Add(tile);
        }

        public int GetBaseTile()
        {
            Random rnd = new();
            return baseTile[rnd.Next(0, baseTile.Count - 1)];
        }

        public void AddTrns(int num, int trn)
        {
            trns[num].Add(trn);
        }

        public int GetTrns(int num)
        {
            Random rnd = new();
            if (trns[num].Count > 0) return trns[num][rnd.Next(0, trns[num].Count - 1)];
            else return 0;
        }

        public static int TotalCount()
        {
            return count;
        }
    }
}
