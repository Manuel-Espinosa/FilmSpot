using System;
using System.Linq;
using FilmSpot.Models;

namespace FilmSpot.Models.Users
{
    public class Admin : User
    {
        public Admin(string name) : base(name)
        {
            IsAdmin = true;
        }

        public override void ShowMenu(AppData data)
        {
            int option;
            do
            {
                Console.WriteLine($"\n=== Menú Admin ({Name}) ===");
                Console.WriteLine("1. Agregar película");
                Console.WriteLine("2. Agregar locación a película");
                Console.WriteLine("3. Ver todas las películas");
                Console.WriteLine("0. Cerrar sesión");
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out option))
                    option = -1;

                switch (option)
                {
                    case 1:
                        Console.Write("Título: ");
                        string title = Console.ReadLine()?.Trim() ?? "";
                        Console.Write("Año: ");
                        if (!int.TryParse(Console.ReadLine(), out int year))
                        {
                            Console.WriteLine("Año inválido.");
                            break;
                        }
                        data.Movies.Add(new Movie(title, year));
                        Console.WriteLine("Película agregada correctamente.");
                        break;

                    case 2:
                        if (data.Movies.Count == 0)
                        {
                            Console.WriteLine("No hay películas disponibles.");
                            break;
                        }

                        Console.WriteLine("Selecciona una película:");
                        for (int i = 0; i < data.Movies.Count; i++)
                            Console.WriteLine($"{i + 1}. {data.Movies[i].Title}");

                        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > data.Movies.Count)
                        {
                            Console.WriteLine("Opción inválida.");
                            break;
                        }
                        index--;

                        Console.WriteLine("\n¿Desea usar una locación existente o crear una nueva?");
                        Console.WriteLine("1. Usar existente");
                        Console.WriteLine("2. Crear nueva");
                        Console.Write("Opción: ");
                        string locOption = Console.ReadLine() ?? "2";

                        Location chosenLocation;

                        if (locOption == "1" && data.AllLocations.Count > 0)
                        {
                            Console.WriteLine("\nLocaciones existentes:");
                            for (int i = 0; i < data.AllLocations.Count; i++)
                                Console.WriteLine($"{i + 1}. {data.AllLocations[i].Name} ({data.AllLocations[i].City}, {data.AllLocations[i].Country})");

                            Console.Write("Seleccione el número: ");
                            if (!int.TryParse(Console.ReadLine(), out int locIndex) || locIndex < 1 || locIndex > data.AllLocations.Count)
                            {
                                Console.WriteLine("Selección inválida.");
                                break;
                            }

                            chosenLocation = data.AllLocations[locIndex - 1];
                        }
                        else
                        {
                            Console.Write("Nombre locación: ");
                            string name = Console.ReadLine()?.Trim() ?? "";

                            Console.Write("Ciudad: ");
                            string city = Console.ReadLine()?.Trim() ?? "";

                            Console.Write("País: ");
                            string country = Console.ReadLine()?.Trim() ?? "";

                            chosenLocation = data.GetOrCreateLocation(name, city, country);
                        }

                        data.Movies[index].AddLocation(chosenLocation);
                        Console.WriteLine($"Locación '{chosenLocation.Name}' agregada a la película '{data.Movies[index].Title}'.");
                        break;

                    case 3:
                        if (data.Movies.Count == 0)
                            Console.WriteLine("No hay películas registradas.");
                        else
                            foreach (var movie in data.Movies)
                                movie.ShowInfo();
                        break;

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
