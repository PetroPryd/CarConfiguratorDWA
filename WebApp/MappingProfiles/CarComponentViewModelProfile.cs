using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.CarComponents;

namespace WebApp.MappingProfiles
{
    // second mapping layer : Business DTO <-> WebApp ViewModel
    public class CarComponentViewModelProfile : Profile
    {
        public CarComponentViewModelProfile()
        {
            CreateMap<CarComponentResponseDto, CarComponentViewModel>();

            CreateMap<CarComponentResponseDto, CarComponentFormViewModel>();

            CreateMap<CarComponentFormViewModel, CarComponentCreateDto>();
        }
    }
}
