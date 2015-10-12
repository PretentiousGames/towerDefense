using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class LocationProvider : ILocationProvider
    {
        public ILocation GetLocation(double x, double y)
        {
            return new Location(x, y);
        }
    }
}