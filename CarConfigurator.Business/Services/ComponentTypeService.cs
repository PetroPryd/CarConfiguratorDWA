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
    public class ComponentTypeService
    {
        private readonly CarConfiguratorContext _context;
        private readonly IMapper _mapper;
        private readonly LogService _logService;

        public ComponentTypeService(CarConfiguratorContext context, IMapper mapper, LogService logService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<List<ComponentTypeResponseDto>> GetAllAsync()
        {
            return await _context.ComponentTypes
                .OrderBy(t => t.Name)
                .ProjectTo<ComponentTypeResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ComponentTypeResponseDto?> GetByIdAsync(int id)
        {
            return await _context.ComponentTypes
                .Where(t => t.IdComponentType == id)
                .ProjectTo<ComponentTypeResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<(ComponentTypeResponseDto? created, string? error)> CreateAsync(ComponentTypeCreateDto dto)
        {
            if (await _context.ComponentTypes.AnyAsync(t => t.Name == dto.Name))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to create a ComponentType with duplicate name '{dto.Name}'.");
                return (null, $"A component type named '{dto.Name}' already exists.");
            }

            var entity = _mapper.Map<ComponentType>(dto);
            _context.ComponentTypes.Add(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info",
                $"ComponentType with id={entity.IdComponentType} has been created.");

            var result = await GetByIdAsync(entity.IdComponentType);
            return (result, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(int id, ComponentTypeCreateDto dto)
        {
            var entity = await _context.ComponentTypes.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning", $"Cannot find ComponentType id={id} to update.");
                return (false, null);
            }

            if (await _context.ComponentTypes.AnyAsync(t => t.Name == dto.Name && t.IdComponentType != id))
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to rename ComponentType id={id} to existing name '{dto.Name}'.");
                return (false, $"Another component type named '{dto.Name}' already exists.");
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info", $"ComponentType with id={id} has been updated.");
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int id)
        {
            var entity = await _context.ComponentTypes.FindAsync(id);
            if (entity == null)
            {
                await _logService.WriteAsync("Warning", $"Cannot find ComponentType id={id} to delete.");
                return (false, null);
            }

            // block deletion if components still use this type
            var inUse = await _context.CarComponents.AnyAsync(c => c.ComponentTypeId == id);
            if (inUse)
            {
                await _logService.WriteAsync("Warning",
                    $"Attempted to delete ComponentType id={id} that still has components.");
                return (false, "Cannot delete this component type because components are still assigned to it.");
            }

            _context.ComponentTypes.Remove(entity);
            await _context.SaveChangesAsync();

            await _logService.WriteAsync("Info", $"ComponentType with id={id} has been deleted.");
            return (true, null);
        }
    }
}
