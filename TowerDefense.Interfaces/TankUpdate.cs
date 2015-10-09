using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense.Interfaces
{
	public class TankUpdate : ITankUpdate
	{
		public IFoe Target { get; set; }
		public Movement MoveDirection { get; set; }
	}
}
