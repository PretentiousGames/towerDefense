using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Goal : IGoal
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }
        public double Health { get; set; }
        public int Id { get; set; }
    }
}