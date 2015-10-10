using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Location : ILocation
    {
        public Location() : this(0, 0)
        {
        }

        public Location(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}