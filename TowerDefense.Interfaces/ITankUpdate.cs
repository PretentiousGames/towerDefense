using System;
namespace TowerDefense.Interfaces
{
	interface ITankUpdate
	{
	    ILocation MovementTarget { get; }
	    IFoe Target { get; }
	}
}
