using System;
using System.Collections.Generic;
using System.Drawing;

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

        public ILocation Location { get; private set; }
        public double X { get { return Location.X; } }
        public double Y { get { return Location.Y; } }
        public Size Size { get; private set; }
        public double Heat { get; set; }
        public ILocation Center
        {
            get { return LocationProvider.GetLocation(X + Size.Width / 2, Y + Size.Height / 2); }
        }
        public int Id { get; private set; }
    
        public abstract string Name { get; }

        public double Speed
        {
            get { return _speed; }
            set { _speed = Math.Min(value, 1); }
        }

        public abstract TankUpdate Update(IGameState gameState);

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
            var xDistance = (entity1.X + (entity1.Size.Width / 2)) - (x);
            var yDistance = (entity1.Y + (entity1.Size.Height / 2)) - (y);
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
        protected double GetDistanceFromTank(ILocation location)
        {
            return GetDistance(this, location.X, location.Y);
        }
        protected double GetDistanceFromTank(double x, double y, Size size)
        {
            return GetDistance(this, x + size.Width / 2, y + size.Height / 2);
        }

        protected string ConvertColorToHexString(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}