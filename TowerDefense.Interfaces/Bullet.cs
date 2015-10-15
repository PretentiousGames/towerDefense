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
			get { return Range * (Damage + Freeze + ((Splash.Damage * Splash.Range) * Splash.HeatMultiplier)) / 1000; }
        }
    }
}