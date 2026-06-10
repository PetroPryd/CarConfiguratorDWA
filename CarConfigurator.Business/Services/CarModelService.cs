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
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Services
{
    public class CarModelService
    {
        private readonly CarConfiguratorContext _context;
        private readonly IMapper _mapper;
        private readonly LogService _logService;

        public CarModelService(CarConfiguratorContext context, IMapper mapper, LogService logService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<List<CarModelResponseDto>> GetAllAsync()
        {
            return await _context.CarModels
                .OrderBy(m => m.Name)
                .ProjectTo<CarModelResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CarModelResponseDto?> GetByIdAsync(int id)
        {
            return await _context.CarModels
                .Where(m => m.IdCarModel == id)
                .ProjectTo<CarModelResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<(CarModelResponseDto? created, string? error)> CreateAsync(CarModelCreateDto dto)
        {
            if (await _context.CarModels.AnyAsync(m => m.Name == dto.Name))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to create a CarModel with duplicate name '{dto.Name}'.");
                return (null, $"A car model named '{dto.Name}' already exists.");
            }

            var entity = _mapper.Map<CarModel>(dto);
            _context.CarModels.Add(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"CarModel with id={entity.IdCarModel} has been created.");

            var result = await GetByIdAsync(entity.IdCarModel);
            return (result, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(int id, CarModelCreateDto dto)
        {
            var entity = await _context.CarModels.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning", $"Cannot find CarModel id={id} to update.");
                return (false, null);
            }

            if (await _context.CarModels.AnyAsync(m => m.Name == dto.Name && m.IdCarModel != id))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to rename CarModel id={id} to existing name '{dto.Name}'.");
                return (false, $"Another car model named '{dto.Name}' already exists.");
            }

            entity.Name = dto.Name;
            entity.Manufacturer = dto.Manufacturer;
            entity.BasePrice = dto.BasePrice;
            entity.Year = dto.Year;
            entity.Description = dto.Description;
            entity.ImageUrl = dto.ImageUrl;
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info", $"CarModel with id={id} has been updated.");
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id)
        {
            var entity = await _context.CarModels.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning", $"Cannot find CarModel id={id} to delete.");
                return (false, null);
            }

            // block delete action if saved user configurations reference this model
            var hasConfigs = await _context.UserConfigurations.AnyAsync(uc => uc.CarModelId == id);
            if (hasConfigs)
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to delete CarModel id={id} that still has saved configurations.");
                return (false, "Cannot delete this car model because saved configurations reference it.");
            }

            // The CarModelCarComponent bridge rows are removed automatically by the
            // database ON DELETE CASCADE, so we only remove the model itself here.
            _context.CarModels.Remove(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info", $"CarModel with id={id} has been deleted.");
            return (true, null);
        }
    }
}
