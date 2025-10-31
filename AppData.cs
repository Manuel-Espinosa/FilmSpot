using System.Collections.Generic;
using System.Linq;
using FilmSpot.Models;

namespace FilmSpot
{
    public class AppData
    {
        public List<Movie> Movies { get; private set; }
        public List<Location> AllLocations { get; private set; }

        public AppData()
        {
            Movies = new List<Movie>();
            AllLocations = new List<Location>();
        }

        public Location GetOrCreateLocation(string name, string city, string country)
        {
            var existing = AllLocations.FirstOrDefault(l =>
                l.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase) &&
                l.City.Equals(city, System.StringComparison.OrdinalIgnoreCase) &&
                l.Country.Equals(country, System.StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing;

            var newLoc = new Location(name, city, country);
            AllLocations.Add(newLoc);
            return newLoc;
        }
    }
}
