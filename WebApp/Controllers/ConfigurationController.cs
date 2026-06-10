using AutoMapper;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Json;
using WebApp.ViewModels.CarComponents;
using WebApp.ViewModels.Configurations;

namespace WebApp.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        private const string BuildKey = "build";
        private const string ModelKey = "buildModel";

        private readonly CarComponentService _componentService;
        private readonly CarModelService _carModelService;
        private readonly UserConfigurationService _configService;
        private readonly IMapper _mapper;

        public ConfigurationController(
            CarComponentService componentService,
            CarModelService carModelService,
            UserConfigurationService configService,
            IMapper mapper)
        {
            _componentService = componentService;
            _carModelService = carModelService;
            _configService = configService;
            _mapper = mapper;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private List<int> GetBuild()
        {
            var json = HttpContext.Session.GetString(BuildKey);
            return string.IsNullOrEmpty(json) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(json)!;
        }
        private void SetBuild(List<int> ids) => HttpContext.Session.SetString(BuildKey, JsonSerializer.Serialize(ids));
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int modelId)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "CarComponent");

            var model = await _carModelService.GetByIdAsync(modelId);
            if (model == null) return NotFound();

            HttpContext.Session.SetInt32(ModelKey, modelId);
            SetBuild(new List<int>());

            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Catalog");

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectModel(int carModelId)
        {
            if (carModelId <= 0) return RedirectToAction(nameof(Build));

            HttpContext.Session.SetInt32(ModelKey, carModelId);

            // Drop any components the newly selected car is not capable with.
            var build = GetBuild();
            if (build.Any())
            {
                var kept = await _componentService.FilterToModelAsync(carModelId, build);
                var removed = build.Count - kept.Count;
                if (removed > 0)
                {
                    SetBuild(kept);
                    TempData["Info"] = $"Removed {removed} component{(removed == 1 ? "" : "s")} the new car doesn't offer.";
                }
            }
            return RedirectToAction(nameof(Build));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var build = GetBuild();
            if (build.Contains(id))
                return RedirectToAction("Details", "Catalog", new { id });

            var toReplace = await _componentService.GetReplacedComponentIdsAsync(id, build);
            var remaining = build.Where(x => !toReplace.Contains(x)).ToList();

            var conflict = await _componentService.FindConflictAsync(id, remaining);
            if (conflict != null)
            {
                TempData["Error"] = $"Can't add this component — it conflicts with \"{conflict}\" already in your build.";
                return RedirectToAction("Details", "Catalog", new { id });
            }

            remaining.Add(id);
            SetBuild(remaining);
            TempData["Success"] = toReplace.Any()
                ? "Component swapped into your build."
                : "Component added to your build.";
            return RedirectToAction("Details", "Catalog", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var build = GetBuild();
            build.Remove(id);
            SetBuild(build);
            return RedirectToAction(nameof(Build));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            SetBuild(new List<int>());
            return RedirectToAction(nameof(Build));
        }

        public async Task<IActionResult> Build()
            => View(await BuildViewModelAsync(new SaveConfigurationViewModel()));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SaveConfigurationViewModel save)
        {
            var modelId = HttpContext.Session.GetInt32(ModelKey);
            if (modelId == null)
            {
                TempData["Error"] = "Please choose a car model first.";
                return RedirectToAction(nameof(Build));
            }

            var build = GetBuild();
            if (!build.Any())
            {
                TempData["Error"] = "Your build is empty — add some components first.";
                return RedirectToAction("Index", "Catalog");
            }

            if (!ModelState.IsValid)
                return View(nameof(Build), await BuildViewModelAsync(save));

            var (id, error) = await _configService.SaveAsync(
                UserId, save.Name, modelId.Value, save.Notes, build);

            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(nameof(Build), await BuildViewModelAsync(save));
            }

            SetBuild(new List<int>());
            HttpContext.Session.Remove(ModelKey);
            TempData["Success"] = "Configuration saved!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _configService.GetForUserAsync(UserId);
            return View(_mapper.Map<List<ConfigurationSummaryViewModel>>(dtos));
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _configService.GetDetailsAsync(id, UserId);
            if (dto == null) return NotFound();
            return View(_mapper.Map<ConfigurationDetailViewModel>(dto));
        }

        private async Task<BuildViewModel> BuildViewModelAsync(SaveConfigurationViewModel save)
        {
            var build = GetBuild();
            var components = new List<CarComponentViewModel>();
            foreach (var cid in build)
            {
                var dto = await _componentService.GetByIdAsync(cid);
                if (dto != null) components.Add(_mapper.Map<CarComponentViewModel>(dto));
            }

            var models = await _carModelService.GetAllAsync();
            var modelId = HttpContext.Session.GetInt32(ModelKey);

            var vm = new BuildViewModel
            {
                Components = components,
                ComponentsSubtotal = components.Sum(c => c.Price),
                Save = save,
                CarModels = models.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Manufacturer} {m.Name} (base {m.BasePrice:C})",
                    Selected = modelId != null && m.Id == modelId.Value
                })
            };

            if (modelId != null)
            {
                var model = models.FirstOrDefault(m => m.Id == modelId.Value);
                if (model != null)
                {
                    vm.SelectedModelId = model.Id;
                    vm.SelectedModelName = $"{model.Manufacturer} {model.Name}";
                    vm.SelectedModelBasePrice = model.BasePrice;
                }
            }
            return vm;
        }
    }
}
