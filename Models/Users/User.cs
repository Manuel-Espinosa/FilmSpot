using Microsoft.Data.Sqlite;

using System;

namespace FilmSpot.Models.Users
{
    public abstract class User
    {
        public int? Id { get; private set; }
        public string Name { get; private set; }
        public bool IsAdmin { get; protected set; }

        protected User(int? id, string name)
        {
            Id = id;
            Name = name;
        }

        public abstract void ShowMenu(SqliteConnection connection);
    }
}
