using AutoMapper;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.Profile;

namespace WebApp.MappingProfiles
{
    public class ProfileViewModelProfile : Profile // omg, such a tautology
    {
        public ProfileViewModelProfile()
        {
            CreateMap<UserProfileDto, ProfileViewModel>();
        }
    }
}
