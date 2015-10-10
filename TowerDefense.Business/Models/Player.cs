﻿using System.Collections.Generic;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<Tank> Tanks { get; set; }
    }
}