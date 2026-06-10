using CarConfigurator.Business.DTOs.Response;
using CarConfigurator.Data;
using CarConfigurator.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Services
{
    public class UserConfigurationService
    {
        private readonly CarConfiguratorContext _context;
        private readonly LogService _logService;

        public UserConfigurationService(CarConfiguratorContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // Shared projection for the detail DTO — reused by the user lookup and the
        // admin lookup so the mapping lives in exactly one place (DRY).
        private static readonly Expression<Func<UserConfiguration, UserConfigurationDetailDto>> DetailProjection =
            c => new UserConfigurationDetailDto
            {
                Id = c.IdUserConfiguration,
                Name = c.Name,
                CarModelName = c.CarModel.Name,
                Notes = c.Notes,
                TotalPrice = c.TotalPrice,
                CreatedAt = c.CreatedAt,
                Components = c.CarComponents.Select(cc => new ConfigurationComponentDto
                {
                    Name = cc.Name,
                    ComponentTypeName = cc.ComponentType.Name,
                    Price = cc.Price
                }).ToList()
            };

        public async Task<(int? id, string? error)> SaveAsync(
            int userId, string name, int carModelId, string? notes, List<int> componentIds)
        {
            var model = await _context.CarModels.FindAsync(carModelId);
            if (model == null) return (null, "The selected car model does not exist.");

            if (await _context.UserConfigurations.AnyAsync(c => c.UserId == userId && c.Name == name))
                return (null, $"You already have a configuration named '{name}'.");

            var components = await _context.CarComponents
                .Where(c => componentIds.Contains(c.IdCarComponent))
                .ToListAsync();

            // Reject a build that contains an incompatible component pair (almost always it will not come
            // to this part of code, it will be blocked earlier)
            var hasConflict = await _context.ComponentCompatibilities
                .AnyAsync(cc => !cc.IsCompatible &&
                    componentIds.Contains(cc.ComponentId) &&
                    componentIds.Contains(cc.CompatibleWithId));
            if (hasConflict)
                return (null, "Your build contains incompatible components. Please remove the conflicting part.");

            var total = model.BasePrice + components.Sum(c => c.Price);

            var config = new UserConfiguration
            {
                Name = name,
                UserId = userId,
                CarModelId = carModelId,
                Notes = notes,
                TotalPrice = total,
                CreatedAt = DateTime.Now
            };

            foreach (var comp in components)
                config.CarComponents.Add(comp);

            _context.UserConfigurations.Add(config);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"User id={userId} saved configuration '{name}' (id={config.IdUserConfiguration}).");
            return (config.IdUserConfiguration, null);
        }

        public async Task<List<UserConfigurationSummaryDto>> GetForUserAsync(int userId)
        {
            return await _context.UserConfigurations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new UserConfigurationSummaryDto
                {
                    Id = c.IdUserConfiguration,
                    Name = c.Name,
                    CarModelName = c.CarModel.Name,
                    TotalPrice = c.TotalPrice,
                    CreatedAt = c.CreatedAt,
                    ComponentCount = c.CarComponents.Count
                })
                .ToListAsync();
        }

        public async Task<UserConfigurationDetailDto?> GetDetailsAsync(int id, int userId) =>
            await _context.UserConfigurations
                .Where(c => c.IdUserConfiguration == id && c.UserId == userId)
                .Select(DetailProjection)
                .FirstOrDefaultAsync();

        public async Task<UserConfigurationDetailDto?> GetDetailsForAdminAsync(int id) =>
            await _context.UserConfigurations
                .Where(c => c.IdUserConfiguration == id)
                .Select(DetailProjection)
                .FirstOrDefaultAsync();
    }
}
