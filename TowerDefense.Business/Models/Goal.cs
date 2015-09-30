using System.Diagnostics;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Goal : IGoal
    {
        public const int Width = 32;
        public const int Height = 48;
        public const int MaxHealth = 1;
        private static int _id = 0;
        public Goal()
        {
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }
        public double Health { get; set; }
        public int Id { get; set; }
    }
}