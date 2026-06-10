using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.CarModels;

namespace WebApp.MappingProfiles
{
    public class CarModelViewModelProfile : Profile
    {
        public CarModelViewModelProfile()
        {
            CreateMap<CarModelResponseDto, CarModelViewModel>();
            CreateMap<CarModelResponseDto, CarModelFormViewModel>();
            CreateMap<CarModelFormViewModel, CarModelCreateDto>();
        }
    }
}
