using Microsoft.Data.Sqlite;
using FilmSpot.Models;
using FilmSpot.Data;
using System.Security.Cryptography;


namespace FilmSpot.Services
{


    public class MovieService
    {
        private SqliteConnection connection;
        public MovieService(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public Movie AddMovie(Movie movie)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Movie (title, year, createdById) VALUES (@Title, @Year, @CreatedById); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@Title", movie.Title);
                command.Parameters.AddWithValue("@Year", movie.Year);
                command.Parameters.AddWithValue("@CreatedById", movie.CreatedById);
                var id = (long)command.ExecuteScalar();
                return new Movie((int)id, movie.Title, movie.Year, movie.CreatedById);
            }
        }
        public Movie[] GetAllMovies()
        {
            List<Movie> movies = new List<Movie>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, title, year, createdById FROM Movie;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        int year = reader.GetInt32(2);
                        int createdById = reader.GetInt32(3);
                        movies.Add(new Movie(id, title, year, createdById));
                    }
                }
            }
            return movies.ToArray();
        }
        public void DeleteMovie(int movieId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Movie WHERE id = @MovieId;";
                command.Parameters.AddWithValue("@MovieId", movieId);
                command.ExecuteNonQuery();
            }
        }
        public void UpdateMovie(Movie movie)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Movie SET title = @Title, year = @Year WHERE id = @MovieId;";
                command.Parameters.AddWithValue("@Title", movie.Title);
                command.Parameters.AddWithValue("@Year", movie.Year);
                command.Parameters.AddWithValue("@MovieId", movie.Id);
                command.ExecuteNonQuery();
            }
        }
        public Movie[] SearchByTitle(string title)
        {
            List<Movie> movies = new List<Movie>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, title, year, createdById FROM Movie WHERE title LIKE @Title;";
                command.Parameters.AddWithValue("@Title", $"%{title}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string movieTitle = reader.GetString(1);
                        int year = reader.GetInt32(2);
                        int createdById = reader.GetInt32(3);
                        movies.Add(new Movie(id, movieTitle, year, createdById));
                    }
                }
            }
            return movies.ToArray();
        }
    }
}