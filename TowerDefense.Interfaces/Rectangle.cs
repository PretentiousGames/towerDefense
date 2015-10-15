namespace TowerDefense.Interfaces
{
    public class Rectangle
    {
        private readonly double _x;
        private readonly double _y;
        private readonly double _width;
        private readonly double _height;

        public Rectangle(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public bool Contains(double x, double y)
        {
            return x <= _x + _width && x >= _x &&
                   y <= _y + _height && y <= _y;
        }
    }
}