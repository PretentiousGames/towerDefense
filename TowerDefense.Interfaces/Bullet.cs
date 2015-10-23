using System;
using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Bullet : IBullet
    {
        public double Range { get; set; }
        public int Damage { get; set; }
        public int Freeze { get; set; }
	    public double SplashRange { get; set; }

        public double SplashHeatMultiplier => 2;
        public double FreezeHeatMultiplier => .5;

        public double ReloadTime
        {
	        get
	        {
	            var splash = (Math.Abs(SplashRange) * SplashHeatMultiplier) + 1; // MUST have +1 or reload time will become 0 if SplashRange is 0
	            var freeze = (Math.Abs(Freeze) * FreezeHeatMultiplier);

                return Range * ((Math.Abs(Damage) + freeze) * splash) / 1000;
	        }
        }
    }
}