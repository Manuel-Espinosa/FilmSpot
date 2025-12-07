using Microsoft.Data.Sqlite;
using FilmSpot.Models;
using FilmSpot.Data;
using System.Security.Cryptography;


namespace FilmSpot.Services
{


    public class LocationService
    {
        private SqliteConnection connection;
        public LocationService(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public Location AddLocation(Location location)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Location (description, streetAddress, city, country, createdById, movieId) VALUES (@Description, @StreetAddress, @City, @Country, @CreatedById, @MovieId); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@Description", location.Description);
                command.Parameters.AddWithValue("@StreetAddress", location.StreetAddress);
                command.Parameters.AddWithValue("@City", location.City);
                command.Parameters.AddWithValue("@Country", location.Country);
                command.Parameters.AddWithValue("@CreatedById", location.CreatedById);
                command.Parameters.AddWithValue("@MovieId", location.MovieId);
                var id = (long)command.ExecuteScalar();
                return new Location(location.Description, location.StreetAddress, location.City, location.Country, location.CreatedById, location.MovieId, (int)id);
            }
        }
        public Location[] GetAllLocations(int? movieId)
        {
            List<Location> locations = new List<Location>();
            using (var command = connection.CreateCommand())
            {
                if (movieId.HasValue)
                {
                    command.CommandText = "SELECT id, description, streetAddress, city, country, createdById, movieId FROM Location WHERE movieId = @MovieId;";
                    command.Parameters.AddWithValue("@MovieId", movieId);
                }
                else
                {
                    command.CommandText = "SELECT id, description, streetAddress, city, country, createdById, movieId FROM Location;";
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string description = reader.GetString(1);
                        string streetAddress = reader.GetString(2);
                        string city = reader.GetString(3);
                        string country = reader.GetString(4);
                        int createdById = reader.GetInt32(5);
                        int filmId = reader.GetInt32(6);
                        locations.Add(new Location(description, streetAddress, city, country, createdById, filmId, id));
                    }
                }
            }
            return locations.ToArray();
        }
        public void DeleteLocation(int locationId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Location WHERE id = @LocationId;";
                command.Parameters.AddWithValue("@LocationId", locationId);
                command.ExecuteNonQuery();
            }
        }
        public void UpdateLocation(Location location)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Location SET description = @Description, streetAddress = @StreetAddress, city = @City, country = @Country WHERE id = @LocationId;";
                command.Parameters.AddWithValue("@Description", location.Description);
                command.Parameters.AddWithValue("@StreetAddress", location.StreetAddress);
                command.Parameters.AddWithValue("@City", location.City);
                command.Parameters.AddWithValue("@Country", location.Country);
                command.Parameters.AddWithValue("@LocationId", location.Id);
                command.ExecuteNonQuery();
            }
        }
        public Location[] SearchByCity(string cityName)
        {
            List<Location> locations = new List<Location>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, description, streetAddress, city, country, createdById, movieId FROM Location WHERE city LIKE @City;";
                command.Parameters.AddWithValue("@City", $"%{cityName}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string description = reader.GetString(1);
                        string streetAddress = reader.GetString(2);
                        string city = reader.GetString(3);
                        string country = reader.GetString(4);
                        int createdById = reader.GetInt32(5);
                        int movieId = reader.GetInt32(6);
                        locations.Add(new Location(description, streetAddress, city, country, createdById, movieId, id));
                    }
                }
            }
            return locations.ToArray();
        }
    }
}