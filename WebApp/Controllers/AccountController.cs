using AutoMapper;
using CarConfigurator.Business.DTOs.Auth;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.Account;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(vm);

            var result = await _authService.ValidateForSignInAsync(vm.Username, vm.Password);
            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(vm);
            }

            var (id, username, role) = result.Value;
            await SignInUserAsync(id, username, role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return role == "Admin"
                ? RedirectToAction("Index", "CarComponent")
                : RedirectToAction("Index", "Catalog");
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<RegisterDto>(vm);
            var (result, error) = await _authService.RegisterAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }

            await SignInUserAsync(result!.UserId, result.Username, result.Role);

            TempData["Success"] = "Welcome! Your account has been created.";
            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        private async Task SignInUserAsync(int? id, string username, string role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
            if (id.HasValue)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, id.Value.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}