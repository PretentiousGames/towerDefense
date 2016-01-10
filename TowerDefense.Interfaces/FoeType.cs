using System;

namespace TowerDefense.Interfaces
{
    [Flags]
    public enum FoeType
    {
        Monster = 0,
        Boss = 1,
    }

    [Flags]
    public enum AbilityType
    {
        None = 0,
        Kamakaze = 1,
        RangedHeat = 2,
        Healing = 3
    }
}