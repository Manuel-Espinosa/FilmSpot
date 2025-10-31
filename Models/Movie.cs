using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmSpot.Models
{
    public class Movie
    {
        public string Title { get; private set; }
        public int Year { get; private set; }
        public List<Location> Locations { get; private set; }

        public Movie(string title, int year)
        {
            Title = title;
            Year = year;
            Locations = new List<Location>();
        }

        public void AddLocation(Location location)
        {
            bool exists = Locations.Any(l =>
                l.Name.Equals(location.Name, StringComparison.OrdinalIgnoreCase) &&
                l.City.Equals(location.City, StringComparison.OrdinalIgnoreCase) &&
                l.Country.Equals(location.Country, StringComparison.OrdinalIgnoreCase));

            if (exists)
                Console.WriteLine("Esta locación ya está registrada para esta película.");
            else
            {
                Locations.Add(location);
                Console.WriteLine("Locación agregada correctamente.");
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"\n{Title} ({Year})");
            if (Locations.Count == 0)
                Console.WriteLine("  - No hay locaciones registradas.");
            else
            {
                Console.WriteLine("  Locaciones:");
                foreach (var loc in Locations)
                    Console.WriteLine($"   - {loc.Name} ({loc.City}, {loc.Country})");
            }
        }
    }
}
