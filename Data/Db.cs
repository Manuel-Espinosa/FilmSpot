using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace FilmSpot.Data
{
    public class Db
    {
        private readonly string _dataSource;

        public Db(string databaseFile = "filmspot.db")
        {
            _dataSource = $"Data Source={databaseFile}";
        }
        public void Initialize()
        {
            string sql = File.ReadAllText("./setup.sql");
            if (!File.Exists(_dataSource))
            {
                using (var connection = new SqliteConnection(_dataSource))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                    Console.WriteLine("Base de datos generada correctamente.");
                }
            }

        }
    }
}
