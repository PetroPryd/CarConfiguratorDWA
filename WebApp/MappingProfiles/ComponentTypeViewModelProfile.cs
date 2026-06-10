using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.ComponentTypes;

namespace WebApp.MappingProfiles
{
    public class ComponentTypeViewModelProfile : Profile
    {
        public ComponentTypeViewModelProfile()
        {
            CreateMap<ComponentTypeResponseDto, ComponentTypeViewModel>();
            CreateMap<ComponentTypeResponseDto, ComponentTypeFormViewModel>();
            CreateMap<ComponentTypeFormViewModel, ComponentTypeCreateDto>();
        }
    }
}
