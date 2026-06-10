using CarConfigurator.Business.DTOs.Response;
using CarConfigurator.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Services
{
    public class UserService
    {
        private readonly CarConfiguratorContext _context;

        public UserService(CarConfiguratorContext context)
        {
            _context = context;
        }

        public async Task<List<UserSummaryDto>> GetAllAsync()
        {
            return await _context.ApplicationUsers
                .OrderBy(u => u.Username)
                .Select(u => new UserSummaryDto
                {
                    Id = u.IdApplicationUser,
                    Name = u.Name,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    ConfigurationCount = u.UserConfigurations.Count
                })
                .ToListAsync();
        }

        public async Task<UserSummaryDto?> GetByIdAsync(int id)
        {
            return await _context.ApplicationUsers
                .Where(u => u.IdApplicationUser == id)
                .Select(u => new UserSummaryDto
                {
                    Id = u.IdApplicationUser,
                    Name = u.Name,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    ConfigurationCount = u.UserConfigurations.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            return await _context.ApplicationUsers
                .Where(u => u.IdApplicationUser == userId)
                .Select(u => new UserProfileDto
                {
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Phone = u.Phone
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool ok, string? error)> UpdateProfileAsync(
            int userId, string email, string? firstName, string? lastName, string? phone)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            if (await _context.ApplicationUsers.AnyAsync(u => u.Email == email && u.IdApplicationUser != userId))
                return (false, "That email is already in use by another account.");

            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Phone = phone;

            // Keep the display Name in sync with first/last when provided.
            var composed = $"{firstName} {lastName}".Trim();
            if (!string.IsNullOrWhiteSpace(composed)) user.Name = composed;

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
