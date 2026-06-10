using AutoMapper;
using CarConfigurator.Business.DTOs.Auth;
using WebApp.ViewModels.Account;

namespace WebApp.MappingProfiles
{
    public class AccountViewModelProfile : Profile
    {
        public AccountViewModelProfile()
        {
            CreateMap<RegisterViewModel, RegisterDto>();
        }
    }
}
