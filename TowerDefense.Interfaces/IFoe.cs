using System.Threading;

namespace TowerDefense.Interfaces
{
    public interface IFoe
    {
        int Id { get; }
        double X { get; }
        double Y { get; }
        double Health { get; }
        double Speed { get; }
    }

    public interface IMonster
    {
        int Id { get; }
        double X { get; set; }
        double Y { get; set; }
        double Health { get; set; }
        double Speed { get; set; }
        
        IFoe Update(GameState gameState);
    }

    public class Monster : IMonster, IFoe
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Health { get; set; }
        public double Speed { get; set; }
        public IFoe Update(GameState gameState)
        {
            Thread.Sleep(10);
            X++;
            Y++;
            return this;
        }
    }
}