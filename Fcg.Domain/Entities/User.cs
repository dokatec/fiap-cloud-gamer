﻿using System.Collections.Generic;
using System.Linq;

namespace Fcg.Domain.Entities
{
    public class User
    {
        public Guid Id { get; } 
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } 
        public string Role { get; private set; } 

        private readonly List<UserGaming> _library;
        public IReadOnlyCollection<UserGaming> Library => _library.AsReadOnly();

        private List<UserGaming> _gamesAdded;
        private List<UserGaming> _gamesRemoved;

        public IReadOnlyCollection<UserGaming> GamesAdded => _gamesAdded.AsReadOnly();
        public IReadOnlyCollection<UserGaming> GamesRemoved => _gamesRemoved.AsReadOnly();

        public User(string name, string email, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio ou nulo.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(email));
            if (!IsValidEmail(email)) 
                throw new ArgumentException("Formato de email é inválido", nameof(email));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Role = role;
            _library = new List<UserGaming>();
            _gamesAdded = new List<UserGaming>();
            _gamesRemoved = new List<UserGaming>();
            PasswordHash = string.Empty; 
        }

        public User(Guid id, string name, string email, string passwordHash, IEnumerable<UserGaming> gameLibrary, string role)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio ou nulo.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio ou nulo.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            _library = gameLibrary?.ToList() ?? new List<UserGaming>(); 
            Role = role;
            _gamesAdded = new List<UserGaming>(); 
            _gamesRemoved = new List<UserGaming>();
        }
        
        public void AddGameToLibrary(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game), "Game não pode ser nulo.");

            if (_library.Any(ug => ug.Game.Id == game.Id))
            {                
                throw new InvalidOperationException($"O game com ID '{game.Id}' já está na biblioteca do usuário.");
            }

            var userGamingEntry = new UserGaming(this, game); 
            _library.Add(userGamingEntry);
            _gamesAdded.Add(userGamingEntry); 
        }

        public void RemoveGameFromLibrary(Guid gameId)
        {
            var gameToRemove = _library.FirstOrDefault(ug => ug.Game.Id == gameId);

            // If the game is found, remove it. If not, do nothing (idempotent behavior).
            if (gameToRemove != null)
            {
                _library.Remove(gameToRemove);
                _gamesRemoved.Add(gameToRemove); 
            }
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));

            PasswordHash = passwordHash;
        }

        public void SetRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Role = role;
        }

        public void UpdateProfile(string newName, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Nome não pode ser vazio ou nulo.", nameof(newName));
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(newEmail));
            if (!IsValidEmail(newEmail))
                throw new ArgumentException("Formato de email é inválido (Parameter 'newEmail')", nameof(newEmail));

            Name = newName;
            Email = newEmail;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
