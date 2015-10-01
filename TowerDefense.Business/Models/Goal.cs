using System.Diagnostics;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Goal : IGoal
    {
        public const int Width = 38;
        public const int Height = 38;
        public const int GoalMaxHealth = 100;
        private static int _id = 0;
        public Goal()
        {
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = GoalMaxHealth;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public int Id { get; set; }
    }
}