using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TowerDefense.Business
{
    public class GravityEntity : IGravityEntity
    {
        public int Id { get; set; }
        public ILocation Location { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }
        public ILocation Center { get { return new Location(X + Size.Width / 2, Y + Size.Height / 2); } }
        public int Duration { get; set; }
        public double Strength { get; set; }
    }
}