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

        public Location GetOrCreateLocation(string description, string streetAddress, string city, string country, int createdById)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT id, description, streetAddress, city, country, createdById
                    FROM Location
                    WHERE description = @Description
                      AND streetAddress = @StreetAddress
                      AND city = @City
                      AND country = @Country
                    LIMIT 1;";

                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@StreetAddress", streetAddress);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@Country", country);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string desc = reader.GetString(1);
                        string street = reader.GetString(2);
                        string cty = reader.GetString(3);
                        string cntry = reader.GetString(4);
                        int createdBy = reader.GetInt32(5);
                        Console.WriteLine($"Reutilizando locación existente: {desc}");
                        return new Location(desc, street, cty, cntry, createdBy, id);
                    }
                }
            }

            Console.WriteLine($"Creando nueva locación: {description}");
            return AddLocation(new Location(description, streetAddress, city, country, createdById));
        }

        public Location AddLocation(Location location)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Location (description, streetAddress, city, country, createdById)
                    VALUES (@Description, @StreetAddress, @City, @Country, @CreatedById);
                    SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@Description", location.Description);
                command.Parameters.AddWithValue("@StreetAddress", location.StreetAddress);
                command.Parameters.AddWithValue("@City", location.City);
                command.Parameters.AddWithValue("@Country", location.Country);
                command.Parameters.AddWithValue("@CreatedById", location.CreatedById);

                var id = (long)command.ExecuteScalar();
                return new Location(location.Description, location.StreetAddress,
                                  location.City, location.Country,
                                  location.CreatedById, (int)id);
            }
        }

        public void AddLocationToMovie(int movieId, int locationId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO MovieLocation (movieId, locationId)
                    VALUES (@MovieId, @LocationId);";

                command.Parameters.AddWithValue("@MovieId", movieId);
                command.Parameters.AddWithValue("@LocationId", locationId);

                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Locación asociada a la película correctamente.");
                }
                catch (SqliteException ex)
                {
                    if (ex.SqliteErrorCode == 19)
                    {
                        Console.WriteLine("Esta locación ya está asociada a esta película.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public Location[] GetLocationsForMovie(int movieId)
        {
            List<Location> locations = new List<Location>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT l.id, l.description, l.streetAddress, l.city, l.country, l.createdById
                    FROM Location l
                    JOIN MovieLocation ml ON l.id = ml.locationId
                    WHERE ml.movieId = @MovieId;";

                command.Parameters.AddWithValue("@MovieId", movieId);

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
                        locations.Add(new Location(description, streetAddress, city, country, createdById, id));
                    }
                }
            }
            return locations.ToArray();
        }

        public Location[] GetAllLocations()
        {
            List<Location> locations = new List<Location>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT id, description, streetAddress, city, country, createdById
                    FROM Location
                    ORDER BY city, description;";

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
                        locations.Add(new Location(description, streetAddress, city, country, createdById, id));
                    }
                }
            }
            return locations.ToArray();
        }

        public void RemoveLocationFromMovie(int movieId, int locationId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    DELETE FROM MovieLocation
                    WHERE movieId = @MovieId AND locationId = @LocationId;";

                command.Parameters.AddWithValue("@MovieId", movieId);
                command.Parameters.AddWithValue("@LocationId", locationId);
                command.ExecuteNonQuery();
            }
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
                command.CommandText = @"
                    UPDATE Location
                    SET description = @Description,
                        streetAddress = @StreetAddress,
                        city = @City,
                        country = @Country
                    WHERE id = @LocationId;";

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
                command.CommandText = @"
                    SELECT id, description, streetAddress, city, country, createdById
                    FROM Location
                    WHERE city LIKE @City;";
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
                        locations.Add(new Location(description, streetAddress, city, country, createdById, id));
                    }
                }
            }
            return locations.ToArray();
        }
    }
}
