namespace TowerDefense.Interfaces
{
    public interface IEntity
    {
        int Id { get; }
        double X { get; }
        double Y { get; }
        Size Size { get; }
    }
}