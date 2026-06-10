using AutoMapper;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.Configurations;
using WebApp.ViewModels.Users;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly UserConfigurationService _configService;
        private readonly IMapper _mapper;

        public UsersController(
            UserService userService,
            UserConfigurationService configService,
            IMapper mapper)
        {
            _userService = userService;
            _configService = configService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(_mapper.Map<List<UserSummaryViewModel>>(users));
        }

        public async Task<IActionResult> Configurations(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var configs = await _configService.GetForUserAsync(id);

            var vm = new UserConfigurationsViewModel
            {
                User = _mapper.Map<UserSummaryViewModel>(user),
                Configurations = _mapper.Map<List<ConfigurationSummaryViewModel>>(configs)
            };
            return View(vm);
        }

        public async Task<IActionResult> ConfigurationDetails(int id)
        {
            var dto = await _configService.GetDetailsForAdminAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<ConfigurationDetailViewModel>(dto));
        }
    }
}
