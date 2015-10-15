using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TowerDefense.Interfaces
{
    public abstract class Tank : ITank
    {
        private static int _id = 0;
        private double _speed;

        public static void SetLocationProvider(ILocationProvider locationProvider)
        {
            if (LocationProvider == null)
            {
                LocationProvider = locationProvider;
            }
        }
        public static ILocationProvider LocationProvider { get; set; }
        
        public Tank(double x, double y)
        {
            Id = _id++;
            Size = new Size(32);
            Location = LocationProvider.GetLocation(x, y);
            Speed = 1;
        }

        public ILocation Location { get; }
        public double X { get { return Location.X; } }
        public double Y { get { return Location.Y; } }
        public Size Size { get; }
        public int Id { get; }

        public abstract string Name { get; }

        public double Speed
        {
            get { return _speed; }
            set { _speed = Math.Min(value, 1); }
        }

        public abstract TankUpdate Update(IGameState gameState);
        public abstract IBullet GetBullet();
        
        protected double GetDistanceToGoal(IFoe foe, List<IGoal> goals)
        {
            var minDistance = double.MaxValue;
            foreach (var goal in goals)
            {
                var xDistance = (goal.X + goal.Size.Width) - (foe.X + foe.Size.Width);
                var yDistance = (goal.Y + goal.Size.Height) - (foe.Y + foe.Size.Height);
                minDistance = Math.Min(Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2)), minDistance);
            }
            return minDistance;
        }

        protected double GetTimeToGoal(IFoe foe, List<IGoal> goals)
        {
            var minDistance = double.MaxValue;
            foreach (var goal in goals)
            {
                var xDistance = (goal.X + goal.Size.Width) - (foe.X + foe.Size.Width);
                var yDistance = (goal.Y + goal.Size.Height) - (foe.Y + foe.Size.Height);
                minDistance = Math.Min(Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2)) / foe.Speed, minDistance);
            }
            return minDistance;
        }

        protected static double GetDistance(IEntity entity1, double x, double y)
        {
            var xDistance = (entity1.X + entity1.Size.Width) - (x);
            var yDistance = (entity1.Y + entity1.Size.Height) - (y);
            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }
        protected static double GetDistance(IEntity entity1, IEntity entity2)
        {
            var xDistance = (entity1.X + entity1.Size.Width) - (entity2.X + entity2.Size.Width);
            var yDistance = (entity1.Y + entity1.Size.Height) - (entity2.Y + entity2.Size.Height);
            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }

        protected double GetDistanceFromTank(IEntity entity)
        {
            return GetDistance(this, entity);
        }
    }
}