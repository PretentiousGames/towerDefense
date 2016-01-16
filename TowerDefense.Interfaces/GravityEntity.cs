namespace TowerDefense.Interfaces
{
    public class GravityEntity : IGravityEntity
    {
        public int Id { get; }
        public ILocation Location { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }
        public ILocation Center { get; }
        public int Duration { get; set; }
        public double Strength { get; set; }
    }
}