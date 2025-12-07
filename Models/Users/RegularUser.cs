using System;
using System.Linq;
using FilmSpot.Models;
using Microsoft.Data.Sqlite;
using FilmSpot.Services;

namespace FilmSpot.Models.Users
{
    public class RegularUser : User
    {
        public RegularUser(int? id, string name) : base(id, name)
        {
            IsAdmin = false;
        }

        public override void ShowMenu(SqliteConnection connection)
        {
            MovieService movieService = new MovieService(connection);
            int option;
            do
            {
                Console.WriteLine($"\n=== Bienvenido {Name} ===");
                Console.WriteLine("1. Buscar película");
                Console.WriteLine("2. Buscar peliculas por ciudad");
                Console.WriteLine("3. Ver todas las películas");
                Console.WriteLine("0. Cerrar sesión");
                Console.Write("Opción: ");
                option = int.Parse(Console.ReadLine() ?? "0");

                switch (option)
                {
                    case 1:
                        SearchMovie(movieService);
                        break;

                    case 2:
                        SearchByCity(movieService);
                        break;

                    case 3:
                        ListAllMovies(movieService);
                        break;


                }

            } while (option != 0);
        }

        private void SearchMovie(MovieService movieService)
        {

            Console.Write("Ingrese el título de la película: ");
            string title = Console.ReadLine()!;

            var movies = movieService.SearchByTitle(title);

            if (movies.Length == 0)
            {
                Console.WriteLine("No se encontró ninguna película con ese nombre.");
                return;
            }
            foreach (var movie in movies)
            {
                movie.ShowInfo(true);
            }
        }

        private void SearchByCity(MovieService movieService)
        {
            Console.Write("Ingrese el nombre de la ciudad: ");
            string cityName = Console.ReadLine()!;

            var movies = movieService.SearchMoviesByCity(cityName);

            if (movies.Length == 0)
            {
                Console.WriteLine("No se encontró ninguna ubicación en esa ciudad.");
                return;
            }
            foreach (var movie in movies)
            {
                movie.ShowInfo(false);
            }
        }

        private void ListAllMovies(MovieService movieService)
        {
            var movies = movieService.GetAllMovies(true);
            if (movies.Length == 0)
            {
                Console.WriteLine("No hay películas registradas.");
                return;
            }

            foreach (var movie in movies)
                movie.ShowInfo();
        }

    }
}
