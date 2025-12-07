using System;
using System.Linq;
using FilmSpot.Models;
using FilmSpot.Services;
using Microsoft.Data.Sqlite;

namespace FilmSpot.Models.Users
{
    public class Admin : User
    {
        public Admin(int? id, string name) : base(id, name)
        {
            IsAdmin = true;
        }

        private Movie? pickMovie(MovieService movieService)
        {
            Movie[] movies = movieService.GetAllMovies();
            if (movies.Length == 0)
            {
                Console.WriteLine("No hay películas disponibles.");
                return null;
            }

            Console.WriteLine("Selecciona una película:");
            foreach (var movie in movies)
                movie.ShowInfo(false);

            var id = Console.ReadLine();


            if (!int.TryParse(id, out int movieId) || !movies.Any(m => m.Id == movieId))
            {
                Console.WriteLine("Opción inválida.");
                return pickMovie(movieService);
            }

            Movie result = movies.First(m => m.Id == movieId);

            Console.WriteLine($"Seleccionaste la película ID {result.Title}.");
            return result;
        }

        public override void ShowMenu(SqliteConnection connection)
        {
            MovieService movieService = new MovieService(connection);
            var id = this.Id;
            if (!id.HasValue)
            {
                throw new Exception("El ID del administrador no puede ser nulo.");
            }
            int option;
            do
            {
                Console.WriteLine($"\n=== Menú Admin ({Name}) ===");
                Console.WriteLine("1. Agregar película");
                Console.WriteLine("2. Ver todas las películas");
                Console.WriteLine("3. Eliminar película");
                Console.WriteLine("4. Actualizar película");
                Console.WriteLine("5. Administrar ubicaciones");

                Console.WriteLine("0. Cerrar sesión");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out option))
                    option = -1;

                switch (option)
                {
                    case 1:
                        {
                            Console.Write("Título: ");
                            string title = Console.ReadLine()?.Trim() ?? "";
                            Console.Write("Año: ");
                            if (!int.TryParse(Console.ReadLine(), out int year))
                            {
                                Console.WriteLine("Año inválido.");
                                break;
                            }

                            var newMovie = movieService.AddMovie(new Movie(null, title, year, id.Value));
                            Console.WriteLine($"Película '{newMovie.Title}' agregada correctamente.");
                            break;
                        }
                    case 2:
                        {
                            Movie[] movies = movieService.GetAllMovies();
                            if (movies.Length == 0)
                                Console.WriteLine("No hay películas registradas.");
                            else
                                foreach (var movie in movies)
                                    movie.ShowInfo(false);
                            break;
                        }
                    case 3:
                        {
                            Movie? movie = pickMovie(movieService);
                            if (movie != null)
                            {
                                movieService.DeleteMovie(movie.Id.Value);
                                Console.WriteLine($"Película con ID {movie.Id} eliminada correctamente.");
                            }
                            break;
                        }
                    case 4:
                        {
                            Movie? movie = pickMovie(movieService);
                            if (movie != null)
                            {
                                Console.Write("Nuevo título: ");
                                string newTitle = Console.ReadLine()?.Trim() ?? movie.Title;
                                Console.Write("Nuevo año: ");
                                if (!int.TryParse(Console.ReadLine(), out int newYear))
                                {
                                    Console.WriteLine("Año inválido.");
                                    break;
                                }
                                Movie updatedMovie = new Movie(movie.Id, newTitle, newYear, movie.CreatedById);
                                movieService.UpdateMovie(updatedMovie);
                                Console.WriteLine($"Película actualizada correctamente.");
                            }
                            break;
                        }

                    case 5:
                        {
                            Movie? movie = pickMovie(movieService);
                            if (movie != null)
                            {
                                ManageLocations(connection, movie);
                            }
                            break;

                        }

                    case 0:
                        Console.WriteLine("Cerrando sesión...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            } while (option != 0);
        }

        private Location? pickLocation(LocationService locationService, int movieId)
        {
            Location[] locations = locationService.GetAllLocations(movieId);
            if (locations.Length == 0)
            {
                Console.WriteLine("No hay ubicaciones disponibles para esta película.");
                return null;
            }

            Console.WriteLine("Selecciona una ubicación:");
            foreach (var location in locations)
                location.ShowInfo();

            var id = Console.ReadLine();


            if (!int.TryParse(id, out int locationId) || !locations.Any(l => l.Id == locationId))
            {
                Console.WriteLine("Opción inválida.");
                return pickLocation(locationService, movieId);
            }

            Location result = locations.First(l => l.Id == locationId);

            Console.WriteLine($"Seleccionaste la ubicación con ID {result.Id}.");
            result.ShowInfo();
            return result;
        }

        private void ManageLocations(SqliteConnection connection, Movie movie)
        {
            int option;
            do
            {

                Console.WriteLine($"\n=== Administrar Ubicaciones para '{movie.Title}' ===");
                Console.WriteLine("1. Agregar ubicación");
                Console.WriteLine("2. Ver ubicaciones");
                Console.WriteLine("3. Eliminar ubicación");
                Console.WriteLine("4. Actualizar ubicación");
                Console.WriteLine("0. Volver al menú anterior");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out option))
                    option = -1;

                LocationService locationService = new LocationService(connection);

                switch (option)
                {
                    case 1:
                        {
                            Console.Write("Descripción: ");
                            string description = Console.ReadLine()?.Trim() ?? "";
                            Console.Write("Dirección: ");
                            string streetAddress = Console.ReadLine()?.Trim() ?? "";
                            Console.Write("Ciudad: ");
                            string city = Console.ReadLine()?.Trim() ?? "";
                            Console.Write("País: ");
                            string country = Console.ReadLine()?.Trim() ?? "";

                            var newLocation = locationService.AddLocation(new Location(description, streetAddress, city, country, this.Id.Value, movie.Id.Value));
                            Console.WriteLine($"Ubicación '{newLocation.Description}' agregada correctamente.");
                            break;
                        }
                    case 2:
                        {
                            Location[] locations = locationService.GetAllLocations(movie.Id.Value);
                            if (locations.Length == 0)
                                Console.WriteLine("No hay ubicaciones registradas para esta película.");
                            else
                                foreach (var location in locations)
                                    location.ShowInfo();
                            break;
                        }
                    case 3:
                        {
                            Location? location = pickLocation(locationService, movie.Id.Value);
                            if (location != null)
                            {
                                locationService.DeleteLocation(location.Id.Value);
                                Console.WriteLine($"Ubicación con ID {location.Id} eliminada correctamente.");
                            }
                            break;
                        }
                    case 4:
                        {
                            Location? location = pickLocation(locationService, movie.Id.Value);
                            if (location != null)
                            {

                                Console.Write("Nueva descripción (o presiona Enter para mantener): ");
                                string inputDescription = Console.ReadLine()?.Trim();
                                string newDescription = string.IsNullOrEmpty(inputDescription) ? location.Description : inputDescription;

                                Console.Write("Nueva dirección (o presiona Enter para mantener): ");
                                string inputStreetAddress = Console.ReadLine()?.Trim();
                                string newStreetAddress = string.IsNullOrEmpty(inputStreetAddress) ? location.StreetAddress : inputStreetAddress;

                                Console.Write("Nueva ciudad (o presiona Enter para mantener): ");
                                string inputCity = Console.ReadLine()?.Trim();
                                string newCity = string.IsNullOrEmpty(inputCity) ? location.City : inputCity;

                                Console.Write("Nuevo país (o presiona Enter para mantener): ");
                                string inputCountry = Console.ReadLine()?.Trim();
                                string newCountry = string.IsNullOrEmpty(inputCountry) ? location.Country : inputCountry;
                                Location updatedLocation = new Location(newDescription, newStreetAddress, newCity, newCountry, location.CreatedById, location.MovieId, location.Id);
                                locationService.UpdateLocation(updatedLocation);
                                Console.WriteLine($"Ubicación con ID {updatedLocation.Id} actualizada correctamente.");
                            }
                            break;
                        }
                    case 0:
                        {
                            Console.WriteLine("Volviendo al menú anterior...");
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Opción no válida. Intente de nuevo.");
                            break;
                        }
                }

            } while (option != 0);
        }
    }
}
