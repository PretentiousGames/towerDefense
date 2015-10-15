using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Bullet : IBullet
    {
        public double Range { get; set; }
        public int Damage { get; set; }
        public int Freeze { get; set; }
	    public SplashBullet Splash { get; set; }

	    public double ReloadTime
        {
	        get
	        {
	            int splashRange = Splash != null ? Splash.Range <= 0 ? 1 : Splash.Range : 1;

                return Range * ((Damage + Freeze) * (splashRange * SplashBullet.HeatMultiplier)) / 1000;
	        }
        }
    }
}