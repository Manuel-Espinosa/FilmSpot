using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace FilmSpot.Data
{
    public class Db
    {
        private readonly string _dataSource;

        public Db(string databaseFile = "filmSpot.db")
        {
            _dataSource = $"Data Source={databaseFile}";
        }
        public void Initialize()
        {
            string sql = File.ReadAllText("./setup.sql");
            if (!File.Exists(_dataSource))
            {

                using (var connection = GetConnection())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                    Console.WriteLine("Base de datos generada correctamente.");
                }
            }
        }
        public SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(_dataSource);
            connection.Open();
            return connection;
        }
    }
}
