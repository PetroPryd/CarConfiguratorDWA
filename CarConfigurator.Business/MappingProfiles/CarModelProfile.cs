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
    public class CarModelProfile : Profile
    {
        public CarModelProfile()
        {
            CreateMap<CarModel, CarModelResponseDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.IdCarModel));

            CreateMap<CarModelCreateDto, CarModel>();
        }
    }
}
