using AutoMapper;
using CarConfigurator.Business.DTOs.Response;
using WebApp.ViewModels.Users;

namespace WebApp.MappingProfiles
{
    public class UserViewModelProfile : Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<UserSummaryDto, UserSummaryViewModel>();
        }
    }
}
