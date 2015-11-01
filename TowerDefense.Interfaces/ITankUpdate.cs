using System;
namespace TowerDefense.Interfaces
{
	interface ITankUpdate
	{
	    ILocation MovementTarget { get; }
		ILocation ShotTarget { get; }
        string TankColor { get; }
	}
}
