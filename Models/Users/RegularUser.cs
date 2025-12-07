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
                Console.WriteLine("4. Ver todas las ubicaciones");
                Console.WriteLine("0. Cerrar sesión");
                Console.Write("Opción: ");
                option = int.Parse(Console.ReadLine() ?? "0");

                switch (option)
                {
                    case 1:
                        SearchMovie(movieService);
                        break;

                    case 2:
                        SearchByLocation(movieService);
                        break;

                    case 3:
                        ListAllMovies(movieService);
                        break;

                    case 4:
                        ListAllLocations(movieService);
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
                movie.ShowInfo();
            }
        }

        private void SearchByLocation(MovieService movieService)
        {
            throw new Exception("Not implemented yet");
        }

        private void ListAllMovies(MovieService movieService)
        {
            var movies = movieService.GetAllMovies();
            if (movies.Length == 0)
            {
                Console.WriteLine("No hay películas registradas.");
                return;
            }

            foreach (var movie in movies)
                movie.ShowInfo();
        }

        private void ListAllLocations(MovieService movieService)
        {
            throw new Exception("Not implemented yet");
        }
    }
}
