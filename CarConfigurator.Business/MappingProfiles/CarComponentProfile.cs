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
    public class CarComponentProfile : Profile
    {
        public CarComponentProfile()
        {
            // Entity -> Response DTO
            CreateMap<CarComponent, CarComponentResponseDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.IdCarComponent))
                .ForMember(d => d.ComponentTypeName, o => o.MapFrom(s => s.ComponentType.Name));

            // Create DTO -> Entity
            CreateMap<CarComponentCreateDto, CarComponent>()
                .ForMember(d => d.IdCarComponent, o => o.Ignore())
                .ForMember(d => d.ComponentType, o => o.Ignore())
                .ForMember(d => d.Images, o => o.Ignore())
                .ForMember(d => d.CarModels, o => o.Ignore())
                .ForMember(d => d.UserConfigurations, o => o.Ignore())
                .ForMember(d => d.ComponentCompatibilityComponents, o => o.Ignore())
                .ForMember(d => d.ComponentCompatibilityCompatibleWiths, o => o.Ignore());
        }
    }
}
