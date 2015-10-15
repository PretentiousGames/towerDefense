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

        public double SplashHeatMultiplier => 4;

        public double ReloadTime
        {
	        get
	        {
                return Range * ((Damage + Freeze) * (SplashRange * SplashHeatMultiplier)) / 1000;
	        }
        }
    }
}