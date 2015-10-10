namespace TowerDefense.Interfaces
{
    public interface ILocationProvider
    {
        ILocation GetLocation(double x, double y);
    }
}