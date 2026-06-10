using AutoMapper;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.Configurations;

namespace WebApp.MappingProfiles
{
    public class ConfigurationViewModelProfile : Profile
    {
        public ConfigurationViewModelProfile()
        {
            CreateMap<UserConfigurationSummaryDto, ConfigurationSummaryViewModel>();
            CreateMap<UserConfigurationDetailDto, ConfigurationDetailViewModel>();
            CreateMap<ConfigurationComponentDto, ConfigurationComponentViewModel>();
        }
    }
}
