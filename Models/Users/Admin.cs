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
                option = int.Parse(Console.ReadLine() ?? "0");

                switch (option)
                {
                    case 1:
                        Console.Write("Título: ");
                        string title = Console.ReadLine()!;
                        Console.Write("Año: ");
                        int year = int.Parse(Console.ReadLine()!);
                        data.Movies.Add(new Movie(title, year));
                        Console.WriteLine("Película agregada.");
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
                        int index = int.Parse(Console.ReadLine()!) - 1;

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
                            int locIndex = int.Parse(Console.ReadLine()!) - 1;
                            chosenLocation = data.AllLocations[locIndex];
                        }
                        else
                        {
                            Console.Write("Nombre locación: ");
                            string name = Console.ReadLine()!;
                            Console.Write("Ciudad: ");
                            string city = Console.ReadLine()!;
                            Console.Write("País: ");
                            string country = Console.ReadLine()!;
                            chosenLocation = data.GetOrCreateLocation(name, city, country);
                        }

                        data.Movies[index].AddLocation(chosenLocation);
                        break;

                    case 3:
                        foreach (var movie in data.Movies)
                            movie.ShowInfo();
                        break;
                }
            } while (option != 0);
        }
    }
}
