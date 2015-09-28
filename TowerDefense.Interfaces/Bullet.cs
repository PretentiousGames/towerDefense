﻿using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Bullet : IBullet
    {
        public double Range { get; set; }
        public double Damage { get; set; }
        public double ReloadTime
        {
            get { return Range * Damage; }
        }
    }
}