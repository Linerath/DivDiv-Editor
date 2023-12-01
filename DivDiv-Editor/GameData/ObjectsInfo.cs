using Microsoft.Xna.Framework;

namespace DivDivEditor.GameObjects
{
    public class ObjectsInfo
    {
        //     
        //   A1|--------------------------|A2
        //     |                          |
        //     |                          |
        //     |                          |
        //     |                          |
        //     |                          |
        //     |                          |
        //     |                          |
        //     |              SP1 /-------|SP2
        //     |                /       / |
        //     |              /       /   |
        //     |            /       /     |
        //     |          /  TP   /       |
        //     |        /   .   /         |
        //     |      /       /           |
        //     |    /       /             |
        //     |  /       /               |
        //     |/SP0    /SP3              |
        //   A0|--------------------------|A3
        //
        public string name;             // Имя объекта
        public int height;              // Высота спрайта объета
        public int width;               // Ширина спрайта
        public int touchPointX;         // Точка касания по х
        public int touchPointY;         // Точка касания по y
        public int var_1;               // 
        public int var_2;               // 
        public int var_3;               // 
        public Point SP0 = new();
        public Point SP1 = new();
        public Point SP2 = new();
        public Point SP3 = new();
        public Point TP = new();
    }
}
