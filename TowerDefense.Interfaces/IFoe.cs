using System.Threading;

namespace TowerDefense.Interfaces
{
    public interface IFoe : IEntity, IKillable
    {
        FoeType FoeType { get; }
        AbilityType AbilityType { get; }
        double Speed { get; }
        double MaxSpeed { get; }
    }
}