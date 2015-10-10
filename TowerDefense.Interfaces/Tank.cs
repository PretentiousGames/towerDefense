using System;
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
    }
}