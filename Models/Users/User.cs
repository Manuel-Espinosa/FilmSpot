using System;

namespace FilmSpot.Models.Users
{
    public abstract class User
    {
        public string Name { get; private set; }
        public bool IsAdmin { get; protected set; }

        protected User(string name)
        {
            Name = name;
        }

        public abstract void ShowMenu(AppData data);
    }
}
