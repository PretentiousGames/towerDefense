using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface ISplashBullet
    {
        int Damage { get; set; }
        int Range { get; set; }
        Point Target { get; set; }
        double HeatMultiplier { get; }
    }
}