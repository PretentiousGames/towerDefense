using System;
using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class SplashBullet : ISplashBullet
    {
        public int Range { get; set; }
        public Point Target { get; set; }
        public double HeatMultiplier => 0.25;
    }
}