using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class BossMonster : Monster
    {
        public new const int Width = 30;
        public new const int Height = 36;

        public BossMonster(int monsterMaxHealth) : base(monsterMaxHealth)
        {
            Size = new Size(Width, Height);
            Speed = MaxSpeed = 0.75;
            _gravityConstant = 400;
            FoeType = FoeType.Boss;
            Ability = gameState =>
            {
                var goal = IsAtGoal(gameState.Goals);
                if (goal != null)
                {
                    Health /= 2;
                    ((Goal)goal).Health -= 1;
                    return 10;
                }
                
                foreach (var tank in ((GameState)gameState).GameTanks)
                {
                    if (IsTankInRange(250, tank.Tank))
                    {
                        tank.Heat += 1;
                    }
                }

                return 2;
            };
        }
    }
}