using System;
namespace TowerDefense.Interfaces
{
	interface ITankUpdate
	{
		Movement MoveDirection { get; }
		IFoe Target { get; }
	}
}
