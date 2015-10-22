using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class BossMonster : Monster
    {
        public BossMonster(int monsterMaxHealth) : base(monsterMaxHealth)
        {
            Size = new Size((int)(Width * 1.5), (int)(Height * 1.5));
            Speed = MaxSpeed = 0.75;
            _gravityConstant = 400;
            FoeType = FoeType.Boss;
            AbilityType = _random.Next(0, 2) == 0 ? AbilityType.RangedHeat : AbilityType.Kamakaze;
            Ability = AbilitiesDictionary[AbilityType];
        }
    }
}