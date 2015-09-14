using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Player
    {
        public string Name { get; set; }
        public ITower Tower { get; set; }
    }
}