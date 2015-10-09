namespace TowerDefense.Interfaces
{
    public interface IEntity
    {
        int Id { get; }
		double X { get; set; }
		double Y { get; set; }
        Size Size { get; }
    }
}