using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class LogService
    {
        private readonly CarConfiguratorContext _context;
        private readonly IMapper _mapper;

        public LogService(CarConfiguratorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task WriteAsync(string level, string message)
        {
            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<LogResponseDto>> GetLastAsync(int n)
        {
            if (n < 1) n = 10;

            return await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .ThenByDescending(l => l.IdLog)
                .Take(n)
                .ProjectTo<LogResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Logs.CountAsync();
        }
    }
}
