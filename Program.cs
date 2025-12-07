using Microsoft.Data.Sqlite;
using System;
using FilmSpot.Models;
using FilmSpot.Models.Users;
using FilmSpot.Data;
using FilmSpot.Services;

namespace FilmSpot
{
    public class Program
    {
        public static void Main()
        {
            Db db = new Db();
            SqliteConnection connection = db.GetConnection();
            UserService userService = new UserService(connection);
            db.Initialize();
            AppData data = new AppData();
            bool exit = false;

            while (!exit)
            {

                Console.WriteLine("=== FilmSpot ===");
                Console.WriteLine("1. Ingresar");
                Console.WriteLine("2. Registrar usuario nuevo");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");

                string input = Console.ReadLine() ?? "0";

                switch (input)
                {
                    case "1":
                        {
                            Console.Write("\nIngrese su nombre de usuario: ");
                            string username = Console.ReadLine();
                            User user;
                            try
                            {
                                user = userService.FindUserByUsername(username);
                                Console.Write("Ingrese su contraseña: ");
                                string password = Console.ReadLine();
                                userService.VerifyUserPassword(user, password);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                                continue;
                            }
                            user.ShowMenu(data);
                            Console.Clear();
                            break;
                        }

                    case "2":
                        {
                            Console.Write("\nIngrese su nombre: ");
                            string userName = Console.ReadLine()!;
                            Console.Write("Ingrese su contraseña: ");
                            string password = Console.ReadLine()!;
                            Console.Write("¿Registrar como administrador? (s/n): ");
                            string isAdminInput = Console.ReadLine()!;
                            bool isAdmin = isAdminInput.Trim().ToLower() == "s";
                            User newUser = isAdmin ? new Admin(null, userName) : new RegularUser(null, userName);
                            try
                            {

                                User registeredUser = userService.RegisterUser(newUser, password);
                                Console.WriteLine($"\nUsuario '{registeredUser.Name}' registrado con éxito.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\nNo se pudo registrar el usuario: {ex.Message}");
                            }
                            Console.Clear();
                            break;
                        }


                    case "0":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }

            Console.WriteLine("\nBye.");
        }
    }
}
