using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmSpot.Models
{
    public class Movie
    {

        public int? Id { get; private set; }
        public int CreatedById { get; private set; }
        public string Title { get; private set; }
        public int Year { get; private set; }
        public List<Location> Locations { get; private set; }

        public Movie(int? id, string title, int year, int createdById = 0)
        {
            Id = id;
            Title = title;
            Year = year;
            Locations = new List<Location>();
            CreatedById = createdById;
        }



        public void ShowInfo(bool showLocations = true)
        {
            Console.WriteLine($"\n{Id}. {Title} ({Year})");
            if (!showLocations) return;
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
