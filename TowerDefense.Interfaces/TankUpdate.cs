using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense.Interfaces
{
	public class TankUpdate : ITankUpdate
	{
		public ILocation ShotTarget { get; set; }
        public ILocation MovementTarget { get; set; }
	}
}
