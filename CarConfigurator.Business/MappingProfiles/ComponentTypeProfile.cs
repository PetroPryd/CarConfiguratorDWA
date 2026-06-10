using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.DTOs.Response;
using CarConfigurator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.MappingProfiles
{
    public class ComponentTypeProfile : Profile
    {
        public ComponentTypeProfile()
        {
            // Entity -> DTO
            CreateMap<ComponentType, ComponentTypeResponseDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.IdComponentType));

            CreateMap<ComponentTypeCreateDto, ComponentType>();
        }
    }
}
