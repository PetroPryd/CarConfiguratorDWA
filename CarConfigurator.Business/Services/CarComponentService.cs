using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.DTOs.Response;
using CarConfigurator.Data;
using CarConfigurator.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Services
{
    public class CarComponentService
    {
        private readonly CarConfiguratorContext _context;
        private readonly IMapper _mapper;
        private readonly LogService _logService;

        public CarComponentService(
            CarConfiguratorContext context,
            IMapper mapper,
            LogService logService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<List<CarComponentResponseDto>> GetAllAsync()
        {
            return await _context.CarComponents
                .ProjectTo<CarComponentResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CarComponentResponseDto?> GetByIdAsync(int id)
        {
            return await _context.CarComponents
                .Where(c => c.IdCarComponent == id)
                .ProjectTo<CarComponentResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<CarComponentResponseDto>> SearchAsync(
            string? search, int? componentTypeId, int page, int count, int? carModelId = null)
        {
            if (page < 1) page = 1;
            if (count < 1) count = 10;

            IQueryable<CarComponent> query = _context.CarComponents;

            // When configuring a specific car, only show components offered for that model
            if (carModelId.HasValue)
                query = query.Where(c => c.CarModels.Any(m => m.IdCarModel == carModelId.Value));

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)));
            }

            if (componentTypeId.HasValue)
                query = query.Where(c => c.ComponentTypeId == componentTypeId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * count)
                .Take(count)
                .ProjectTo<CarComponentResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<CarComponentResponseDto>
            {
                Items = items,
                Page = page,
                Count = count,
                TotalCount = totalCount
            };
        }

        public async Task<List<int>> FilterToModelAsync(int carModelId, IEnumerable<int> componentIds)
        {
            var ids = componentIds.ToList();
            if (ids.Count == 0) return new List<int>();

            return await _context.CarComponents
                .Where(c => ids.Contains(c.IdCarComponent) &&
                            c.CarModels.Any(m => m.IdCarModel == carModelId))
                .Select(c => c.IdCarComponent)
                .ToListAsync();
        }

        private static readonly HashSet<string> SingleChoiceTypes =
            new(StringComparer.OrdinalIgnoreCase) { "Engine", "Color", "Wheels", "Interior" };

        public async Task<List<int>> GetReplacedComponentIdsAsync(int candidateId, IEnumerable<int> existingIds)
        {
            var ids = existingIds.Where(i => i != candidateId).ToList();
            if (ids.Count == 0) return new List<int>();

            var candidate = await _context.CarComponents
                .Where(c => c.IdCarComponent == candidateId)
                .Select(c => new { c.ComponentTypeId, TypeName = c.ComponentType.Name })
                .FirstOrDefaultAsync();

            if (candidate == null || !SingleChoiceTypes.Contains(candidate.TypeName))
                return new List<int>();

            return await _context.CarComponents
                .Where(c => ids.Contains(c.IdCarComponent) && c.ComponentTypeId == candidate.ComponentTypeId)
                .Select(c => c.IdCarComponent)
                .ToListAsync();
        }

        public async Task<string?> FindConflictAsync(int candidateId, IEnumerable<int> existingIds)
        {
            var ids = existingIds.Where(i => i != candidateId).ToList();
            if (ids.Count == 0) return null;

            var conflictId = await _context.ComponentCompatibilities
                .Where(cc => !cc.IsCompatible &&
                    ((cc.ComponentId == candidateId && ids.Contains(cc.CompatibleWithId)) ||
                     (cc.CompatibleWithId == candidateId && ids.Contains(cc.ComponentId))))
                .Select(cc => cc.ComponentId == candidateId ? cc.CompatibleWithId : cc.ComponentId)
                .FirstOrDefaultAsync();

            if (conflictId == 0) return null;

            return await _context.CarComponents
                .Where(c => c.IdCarComponent == conflictId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<(CarComponentResponseDto? created, string? error)> CreateAsync(CarComponentCreateDto dto)
        {
            if (await _context.CarComponents.AnyAsync(c => c.Name == dto.Name))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to create a CarComponent with a duplicate name '{dto.Name}'.");
                return (null, $"A component named '{dto.Name}' already exists.");
            }

            if (!await _context.ComponentTypes.AnyAsync(t => t.IdComponentType == dto.ComponentTypeId))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to create a CarComponent with non-existent ComponentType id={dto.ComponentTypeId}.");
                return (null, $"Component type with id {dto.ComponentTypeId} does not exist.");
            }

            var entity = _mapper.Map<CarComponent>(dto);

            _context.CarComponents.Add(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"CarComponent with id={entity.IdCarComponent} has been created.");

            var result = await GetByIdAsync(entity.IdCarComponent);
            return (result, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(int id, CarComponentCreateDto dto)
        {
            var entity = await _context.CarComponents.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning",
                    $"Cannot find CarComponent id={id} to update.");
                return (false, null);
            }

            if (await _context.CarComponents.AnyAsync(c => c.Name == dto.Name && c.IdCarComponent != id))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to rename CarComponent id={id} to existing name '{dto.Name}'.");
                return (false, $"Another component named '{dto.Name}' already exists.");
            }

            if (!await _context.ComponentTypes.AnyAsync(t => t.IdComponentType == dto.ComponentTypeId))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to set CarComponent id={id} to non-existent ComponentType id={dto.ComponentTypeId}.");
                return (false, $"Component type with id {dto.ComponentTypeId} does not exist.");
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.ImageUrl = dto.ImageUrl;
            entity.StockQuantity = dto.StockQuantity;
            entity.IsAvailable = dto.IsAvailable;
            entity.ComponentTypeId = dto.ComponentTypeId;

            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"CarComponent with id={id} has been updated.");

            return (true, null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.CarComponents.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning",
                    $"Cannot find CarComponent id={id} to delete.");
                return false;
            }

            var compatibilityRows = await _context.ComponentCompatibilities
                .Where(cc => cc.ComponentId == id || cc.CompatibleWithId == id)
                .ToListAsync();
            _context.ComponentCompatibilities.RemoveRange(compatibilityRows);

            _context.CarComponents.Remove(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"CarComponent with id={id} has been deleted.");

            return true;
        }
    }
}
