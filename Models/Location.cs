using System;

namespace FilmSpot.Models
{
    public class Location
    {
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }

        public Location(string name, string city, string country)
        {
            Name = name;
            City = city;
            Country = country;
        }

        public void ShowInfo()
        {
            Console.WriteLine($" {Name} - {City}, {Country}");
        }
    }
}
