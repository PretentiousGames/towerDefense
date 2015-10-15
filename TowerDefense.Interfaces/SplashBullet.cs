using System;
using System.Drawing;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class SplashBullet : ISplashBullet
    {
        public int Range { get; set; }
        public static double HeatMultiplier => 0.25;
    }
}