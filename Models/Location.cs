using System;

namespace FilmSpot.Models
{
    public class Location
    {
        public string Description { get; private set; }
        public string StreetAddress { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public int? Id { get; private set; }
        public int CreatedById { get; private set; }

        public Location(string description, string streetAddress, string city, string country, int createdById, int? id = null)
        {
            Description = description;
            StreetAddress = streetAddress;
            City = city;
            Country = country;
            CreatedById = createdById;
            Id = id;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"   {Id}. {Description} - {StreetAddress}, {City}, {Country}");
        }
    }
}
