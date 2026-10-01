using Metro.Shared.Entities;

namespace Metro.Domain.Users.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; }

        public string Email { get; private set; }

        public string Document { get; private set; }

        public string Phone { get; private set; }

        public string Password { get; private set; }

        public string Role { get; private set; }

        public bool IsActive { get; private set; } = false;

        public void UpdatePassword(string password) => Password = password;

        public User(string name, string email, string document, string phone, string password, string role, bool isActive = false)
        {
            Name = name;
            Email = email;
            Document = document;
            Phone = phone;
            Password = password;
            Role = role;
            IsActive = isActive;
        }

        public void Update(string name, string email, string document, string phone, string role, bool? isActive)
        {
            Name = name;
            Email = email;
            Document = document;
            Phone = phone;
            Role = role;

            if (isActive is not null)
            {
                IsActive = (bool)isActive;
            }
        }
    }
}
