using System.Collections.Generic;

namespace DivDivEditor
{
    public class Metaobject
    {
        /*
         * startdef metaobject WALL small_wall_i_05
           group stone_wall
           walltype normal outside
           object 1087 0 0 0
           enddef metaobject

         * object 811 front 0 0 on 0
         * object 7119 back 2 2 on 0
         * object 322 right X center 85 on 0
         * 
         * */
        public string metaobject;
        public string group;
        public string location;
        public string type;
        public string walltype;
        public int placement;
        public int[] size = new int[2];
        //public List<int>[] Object = new List<int>[4];
        public List<int[]> Object = new List<int[]>();
        static int count = 0;
        public Metaobject()
        {
            count++;
        }
        public void setMet(string met)
        {
            this.metaobject = met;
        }
        public void setGroup(string grp)
        {
            this.group = grp;
        }
        public void setLocation(string loc)
        {
            this.location = loc;
        }
        public void setPlacement(int plac)
        {
            this.placement = plac;
        }
        public void setType(string type)
        {
            this.type = type;
        }
        public void setWalltype(string wtype)
        {
            this.walltype = wtype;
        }
        public void setSize(int x, int y)
        {
            this.size[0] = x;
            this.size[1] = y;
        }
        public void addObject(int[] obj)
        {
            this.Object.Add(obj);
        }
        public static int TotalCount()
        {
            return count;
        }
    }
}
