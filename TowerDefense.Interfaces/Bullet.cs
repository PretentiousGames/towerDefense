using System;

namespace TowerDefense.Interfaces
{
    public class Bullet : IBullet
    {
        public int Damage { get; set; }
        public int Freeze { get; set; }
	    public double SplashRange { get; set; }
        public double SplashHeatMultiplier => 2;
        public double FreezeHeatMultiplier => .5;
        public double GravityDuration { get; set; }
        public double GravityStrength { get; set; }
        public double GravityMultiplier => 10000;

        public double GetReloadTime(double range)
        {
            var splash = (Math.Abs(SplashRange) * SplashHeatMultiplier);
            var freeze = (Math.Abs(Freeze) * FreezeHeatMultiplier);
            var gravity = (Math.Abs(GravityDuration) * Math.Abs(GravityStrength) * GravityMultiplier);

            if (gravity > 0)
            {
                return gravity;
            }
            else
            {
                return range * ((Math.Abs(Damage) + freeze) + (Math.Abs(Damage) * splash)) / 1000;
            }
        }
    }
}