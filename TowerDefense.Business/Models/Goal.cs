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

        public ILocation Location { get; set; }
        public double X { get { return Location.X; } }
        public double Y { get { return Location.Y; } }
        public Size Size { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public int Id { get; set; }
        public ILocation Center
        {
            get { return new Location(X + Size.Width / 2, Y + Size.Height / 2); }
        }
    }
}