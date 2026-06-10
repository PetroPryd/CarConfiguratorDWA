using AutoMapper;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.Profile;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public ProfileController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var dto = await _userService.GetProfileAsync(UserId);
            if (dto == null) return NotFound();
            return View(_mapper.Map<ProfileViewModel>(dto));
        }

        // AJAX was used here
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return Json(new { success = false, error = string.Join(" ", errors) });
            }

            var (ok, error) = await _userService.UpdateProfileAsync(
                UserId, model.Email, model.FirstName, model.LastName, model.Phone);

            return ok
                ? Json(new { success = true, message = "Profile updated successfully." })
                : Json(new { success = false, error });
        }
    }
}
