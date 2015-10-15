using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface ISplashBullet
    {
        int Range { get; set; }
        Point Target { get; set; }
        double HeatMultiplier { get; }
    }
}