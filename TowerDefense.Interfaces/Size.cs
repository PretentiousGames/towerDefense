using System.CodeDom;

namespace TowerDefense.Interfaces
{
    public class Size
    {
        public Size()
            : this(0, 0)
        {

        }

        public Size(int i)
            : this(i, i)
        {
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; set; }
        public double Height { get; set; }
    }
}