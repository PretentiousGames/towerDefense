using System;

namespace TowerDefense.Interfaces
{
    public class Bullet : IBullet
    {
        public int Damage { get; set; }
        public int Freeze { get; set; }
	    public int SplashRange { get; set; }
        public double SplashHeatMultiplier => 2;
        public double FreezeHeatMultiplier => .5;
        public int GravityDuration { get; set; }
        public double GravityStrength { get; set; }
        public int GravityMultiplier => 10;

        public long GetReloadTime(double range)
        {
            var splash = (Abs(SplashRange) * SplashHeatMultiplier);
            var freeze = (Abs(Freeze) * FreezeHeatMultiplier);
            var gravity = (Abs(GravityDuration) * Math.Abs(GravityStrength) * GravityMultiplier);

            if (gravity > 0)
            {
                return (long)gravity;
            }
            else
            {
                return (long)(range * ((Abs(Damage) + freeze) + (Abs(Damage) * splash)) / 1000);
            }
        }

        private long Abs(long value)
        {
            return value > 0 ? value : -value;
        }
    }
}