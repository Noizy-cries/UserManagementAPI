using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService
    {
        private readonly List<User> _users = new()
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Department = "HR", CreatedDate = DateTime.Now.AddDays(-30), IsActive = true },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Department = "IT", CreatedDate = DateTime.Now.AddDays(-15), IsActive = true },
            new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Department = "Finance", CreatedDate = DateTime.Now.AddDays(-7), IsActive = true }
        };

        public IEnumerable<User> GetAllUsers() => _users.Where(u => u.IsActive);

        public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id && u.IsActive);

        public User CreateUser(User user)
        {
            // Check for duplicate email
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Email '{user.Email}' is already registered");

            // Auto-generate ID
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;

            _users.Add(user);
            return user;
        }

        public User? UpdateUser(int id, User updatedUser)
        {
            var user = GetUserById(id);
            if (user == null) return null;

            // Check for duplicate email (excluding current user)
            if (_users.Any(u => u.Id != id && u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Email '{updatedUser.Email}' is already registered");

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;
            user.UpdatedDate = DateTime.Now;

            return user;
        }

        public bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user == null) return false;

            // Soft delete (mark as inactive)
            user.IsActive = false;
            user.UpdatedDate = DateTime.Now;
            return true;
        }

        public bool EmailExists(string email, int? excludeUserId = null)
        {
            return _users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.IsActive &&
                (excludeUserId == null || u.Id != excludeUserId));
        }
    }
}