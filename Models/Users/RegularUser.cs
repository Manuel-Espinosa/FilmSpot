using System;
using System.Linq;
using FilmSpot.Models;

namespace FilmSpot.Models.Users
{
    public class RegularUser : User
    {
        public RegularUser(string name) : base(name)
        {
            IsAdmin = false;
        }

        public override void ShowMenu(AppData data)
        {
            int option;
            do
            {
                Console.WriteLine($"\n=== Bienvenido {Name} ===");
                Console.WriteLine("1. Buscar película");
                Console.WriteLine("2. Buscar locación");
                Console.WriteLine("3. Ver todas las películas");
                Console.WriteLine("4. Ver todas las locaciones");
                Console.WriteLine("0. Cerrar sesión");
                Console.Write("Opción: ");
                option = int.Parse(Console.ReadLine() ?? "0");

                switch (option)
                {
                    case 1:
                        SearchMovie(data);
                        break;

                    case 2:
                        SearchByLocation(data);
                        break;

                    case 3:
                        ListAllMovies(data);
                        break;

                    case 4:
                        VerTodasLasLocaciones(data);
                        break;
                }

            } while (option != 0);
        }

        private void SearchMovie(AppData data)
        {
            if (data.Movies.Count == 0)
            {
                Console.WriteLine("No hay películas registradas.");
                return;
            }

            Console.Write("Ingrese el título de la película: ");
            string title = Console.ReadLine()!;

            var movie = data.Movies
                .FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (movie != null)
            {
                movie.ShowInfo();
            }
            else
            {
                Console.WriteLine("No se encontró ninguna película con ese nombre.");
            }
        }

        private void SearchByLocation(AppData data)
        {
            if (data.AllLocations.Count == 0)
            {
                Console.WriteLine("No hay locaciones registradas aún.");
                return;
            }

            Console.WriteLine("\nLocaciones disponibles:");
            for (int i = 0; i < data.AllLocations.Count; i++)
                Console.WriteLine($"{i + 1}. {data.AllLocations[i].Name} ({data.AllLocations[i].City}, {data.AllLocations[i].Country})");

            Console.Write("Ingrese el nombre o ciudad de la locación: ");
            string query = Console.ReadLine()!;

            var matchedLocations = data.AllLocations
                .Where(l =>
                    l.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.City.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matchedLocations.Count == 0)
            {
                Console.WriteLine("No se encontró ninguna locación que coincida.");
                return;
            }

            foreach (var loc in matchedLocations)
            {
                Console.WriteLine($"\n {loc.Name} ({loc.City}, {loc.Country})");
                var moviesAtLoc = data.Movies
                    .Where(m => m.Locations.Any(l => l == loc))
                    .ToList();

                if (moviesAtLoc.Count == 0)
                    Console.WriteLine("No hay películas registradas en esta locación.");
                else
                {
                    Console.WriteLine("Películas filmadas aquí:");
                    foreach (var movie in moviesAtLoc)
                        Console.WriteLine($"   - {movie.Title} ({movie.Year})");
                }
            }
        }

        private void ListAllMovies(AppData data)
        {
            if (data.Movies.Count == 0)
            {
                Console.WriteLine("No hay películas registradas.");
                return;
            }

            foreach (var movie in data.Movies)
                movie.ShowInfo();
        }

        private void VerTodasLasLocaciones(AppData data)
        {
            if (data.AllLocations.Count == 0)
            {
                Console.WriteLine("No hay locaciones registradas.");
                return;
            }

            Console.WriteLine("\nLocaciones registradas:");
            foreach (var loc in data.AllLocations)
                Console.WriteLine($"- {loc.Name} ({loc.City}, {loc.Country})");
        }
    }
}
