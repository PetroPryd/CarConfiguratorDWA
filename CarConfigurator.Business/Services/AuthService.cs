using CarConfigurator.Business.DTOs.Auth;
using CarConfigurator.Business.Security;
using CarConfigurator.Data;
using CarConfigurator.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Services
{
    public class AuthService
    {
        private readonly CarConfiguratorContext _context;
        private readonly TokenProvider _tokenProvider;
        private readonly LogService _logService;

        public AuthService(CarConfiguratorContext context, TokenProvider tokenProvider, LogService logService)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _logService = logService;
        }

        public async Task<(AuthResponseDto? result, string? error)> RegisterAsync(RegisterDto dto)
        {
            if (await _context.ApplicationUsers.AnyAsync(u => u.Username == dto.Username))
                return (null, $"Username '{dto.Username}' is already taken.");

            if (await _context.ApplicationUsers.AnyAsync(u => u.Email == dto.Email))
                return (null, $"Email '{dto.Email}' is already registered.");

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(dto.Password, salt);

            var user = new ApplicationUser
            {
                Name = string.IsNullOrWhiteSpace($"{dto.FirstName} {dto.LastName}".Trim())
                    ? dto.Username
                    : $"{dto.FirstName} {dto.LastName}".Trim(),
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Role = "User",
                PwdSalt = salt,
                PwdHash = hash
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info", $"New user '{user.Username}' registered (id={user.IdApplicationUser}).");

            var token = _tokenProvider.CreateToken(user);
            return (new AuthResponseDto { UserId = user.IdApplicationUser, Token = token, Username = user.Username, Role = user.Role }, null);
        }

        public async Task<(AuthResponseDto? result, string? error)> LoginAsync(LoginDto dto)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                await _logService.WriteAsync("Warning", $"Failed login: unknown username '{dto.Username}'.");
                return (null, "Invalid username or password.");
            }

            var hash = PasswordHashProvider.GetHash(dto.Password, user.PwdSalt);
            if (hash != user.PwdHash)
            {
                await _logService.WriteAsync("Warning", $"Failed login: wrong password for '{dto.Username}'.");
                return (null, "Invalid username or password.");
            }

            await _logService.WriteAsync("Info", $"User '{user.Username}' logged in.");

            var token = _tokenProvider.CreateToken(user);
            return (new AuthResponseDto { UserId = user.IdApplicationUser, Token = token, Username = user.Username, Role = user.Role }, null);
        }

        public async Task<(bool ok, string? error)> ChangePasswordAsync(string username, ChangePasswordDto dto)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return (false, null);

            var oldHash = PasswordHashProvider.GetHash(dto.OldPassword, user.PwdSalt);
            if (oldHash != user.PwdHash)
            {
                await _logService.WriteAsync("Warning", $"Failed password change for '{username}': wrong current password.");
                return (false, "Current password is incorrect.");
            }

            var newSalt = PasswordHashProvider.GetSalt();
            user.PwdSalt = newSalt;
            user.PwdHash = PasswordHashProvider.GetHash(dto.NewPassword, newSalt);

            await _context.SaveChangesAsync();
            await _logService.WriteAsync("Info", $"User '{username}' changed their password.");
            return (true, null);
        }

        public async Task<(int id, string username, string role)?> ValidateForSignInAsync(string username, string password)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                await _logService.WriteAsync("Warning", $"Failed MVC login: unknown username '{username}'.");
                return null;
            }

            if (PasswordHashProvider.GetHash(password, user.PwdSalt) != user.PwdHash)
            {
                await _logService.WriteAsync("Warning", $"Failed MVC login: wrong password for '{username}'.");
                return null;
            }

            await _logService.WriteAsync("Info", $"User '{user.Username}' signed in to the website.");
            return (user.IdApplicationUser, user.Username, user.Role);
        }
    }
}