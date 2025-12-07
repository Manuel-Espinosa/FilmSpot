using Microsoft.Data.Sqlite;
using FilmSpot.Models.Users;
using FilmSpot.Data;
using System.Security.Cryptography;


namespace FilmSpot.Services
{

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }
    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException(string message) : base(message) { }
    }


    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] testHash = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
    }
    public class UserService
    {
        private SqliteConnection connection;
        public UserService(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public User RegisterUser(User user, string password)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO User (username, passwordHash, isAdmin) VALUES (@Username, @PasswordHash, @IsAdmin); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@Username", user.Name);
                command.Parameters.AddWithValue("@PasswordHash", PasswordHelper.HashPassword(password));
                command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin ? 1 : 0);
                long userId = (long)command.ExecuteScalar()!;
                if (user.IsAdmin)
                {
                    return new Admin((int)userId, user.Name);
                }
                return new RegularUser((int)userId, user.Name);
            }
        }
        public User FindUserByUsername(string username)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, username, isAdmin FROM User WHERE username = @Username LIMIT 1;";
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var id = reader.GetInt64(0);
                        var name = reader.GetString(1);
                        var isAdmin = reader.GetInt32(2) == 1;
                        if (isAdmin)
                        {
                            return new Admin((int)id, name);
                        }
                        else
                        {
                            return new RegularUser((int)id, name);
                        }
                    }

                    throw new UserNotFoundException($"Usuario '{username}' no encontrado.");

                }
            }
        }
        public bool VerifyUserPassword(User user, string password)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT passwordHash FROM User WHERE id = @UserId LIMIT 1;";
                command.Parameters.AddWithValue("@UserId", user.Id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var storedHash = reader.GetString(0);
                        var result = PasswordHelper.VerifyPassword(password, storedHash);
                        if (!result)
                        {
                            throw new UnauthorizedAccessException("Contraseña incorrecta.");
                        }
                        return result;
                    }
                    throw new UserNotFoundException($"Usuario con ID '{user.Id}' no encontrado.");
                }
            }
        }
    }
}