using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class BossMonster : Monster
    {
        public new const int Width = 32;
        public new const int Height = 32;

        public BossMonster(int monsterMaxHealth) : base(monsterMaxHealth)
        {
            Size = new Size(Width, Height);
            Speed = MaxSpeed = 0.5;
            _gravityConstant = 200;
            FoeType = FoeType.Boss;
            Ability = state =>
            {
                int cooldown = 500; // ticks
                int range = 250;

                foreach (var tank in ((GameState)state).GameTanks)
                {
                    if (IsTankInRange(range, tank.Tank))
                    {
                        tank.Heat += 100;
                    }
                }

                return cooldown;
            };
        }
    }
}