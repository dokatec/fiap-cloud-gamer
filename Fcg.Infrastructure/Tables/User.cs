﻿namespace Fcg.Infrastructure.Tables
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;

        public ICollection<UserGaming>? Library { get; set; } = new List<UserGaming>();
    }
}
