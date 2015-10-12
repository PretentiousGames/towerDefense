using System.Data.Common;

namespace TowerDefense.Interfaces
{
    public interface IEntity
    {
        int Id { get; }
        ILocation Location { get; }
        double X { get; }
        double Y { get; }
        Size Size { get; }
    }
}