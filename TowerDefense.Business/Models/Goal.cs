using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Goal : IGoal
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public double Health { get; set; }
    }
}