namespace TowerDefense.Interfaces
{
    public interface IFoe
    {
        double X { get; }
        double Y { get; }
        double Health { get; }
        double Speed { get; }
    }

    public interface IMonster
    {
        double X { get; set; }
        double Y { get; set; }
        double Health { get; set; }
        double Speed { get; set; }
        
        IFoe Update(GameState gameState);
    }

    class Monster : IMonster, IFoe
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Health { get; set; }
        public double Speed { get; set; }
        public IFoe Update(GameState gameState)
        {
            throw new System.NotImplementedException();
        }
    }
}