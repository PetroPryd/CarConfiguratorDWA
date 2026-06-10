using AutoMapper;
using CarConfigurator.Business.DTOs.Response;
using CarConfigurator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.MappingProfiles
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            // Entity -> DTO.
            CreateMap<Log, LogResponseDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.IdLog));
        }
    }
}
