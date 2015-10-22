using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
            AbilityType = AbilityType.RangedHeat;
            Ability = AbilitiesDictionary[AbilityType];
        }
    }
}