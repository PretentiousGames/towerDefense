using System.Threading;

namespace TowerDefense.Interfaces
{
    public interface IFoe : IEntity, IKillable
    {
        double Speed { get; }
    }
}