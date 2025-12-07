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
                                    movie.ShowInfo();
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

                    case 0:
                        Console.WriteLine("Cerrando sesión...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            } while (option != 0);
        }
    }
}
