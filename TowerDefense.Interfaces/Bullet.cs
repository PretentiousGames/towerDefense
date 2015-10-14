using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Bullet : IBullet
    {
        public double Range { get; set; }
        public int Damage { get; set; }
        public int Freeze { get; set; }
	    public int SplashDamage { get; set; }
	    public int SplashRange { get; set; }

	    public double ReloadTime
        {
			get { return Range * (Damage + Freeze + ((SplashDamage * SplashRange) / 4)) / 1000; }
        }
    }
}