using System;
using FilmSpot.Models;
using FilmSpot.Models.Users;

namespace FilmSpot
{
    public class Program
    {
        public static void Main()
        {
            AppData data = new AppData();
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== FilmSpot ===");
                Console.WriteLine("1. Ingresar como Administrador");
                Console.WriteLine("2. Ingresar como Usuario Regular");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");

                string input = Console.ReadLine() ?? "0";

                switch (input)
                {
                    case "1":
                        Console.Write("\nIngrese su nombre: ");
                        string adminName = Console.ReadLine()!;
                        User admin = new Admin(adminName);
                        admin.ShowMenu(data);
                        break;

                    case "2":
                        Console.Write("\nIngrese su nombre: ");
                        string userName = Console.ReadLine()!;
                        User regular = new RegularUser(userName);
                        regular.ShowMenu(data);
                        break;

                    case "0":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Presione Enter para continuar...");
                        Console.ReadLine();
                        break;
                }
            }

            Console.WriteLine("\nBye.");
        }
    }
}
