using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Services;
using WebApp.ViewModels.CarComponents;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarComponentController : Controller
    {
        private readonly CarComponentService _service;
        private readonly ComponentTypeService _typeService;
        private readonly IWebHostEnvironment _env;   // wwwroot path
        private readonly IMapper _mapper;

        public CarComponentController(
            CarComponentService service,
            ComponentTypeService typeService,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _service = service;
            _typeService = typeService;
            _env = env;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? search, int? componentTypeId, int page = 1)
        {
            const int pageSize = 10;

            var paged = await _service.SearchAsync(search, componentTypeId, page, pageSize);
            var types = await _typeService.GetAllAsync();

            var vm = new CarComponentListViewModel
            {
                Items = _mapper.Map<List<CarComponentViewModel>>(paged.Items),
                Search = search,
                ComponentTypeId = componentTypeId,
                Page = paged.Page,
                TotalPages = paged.TotalPages,
                TotalCount = paged.TotalCount,
                ComponentTypes = new SelectList(types, "Id", "Name", componentTypeId)
            };
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new CarComponentFormViewModel { ComponentTypes = await GetTypeOptions() };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarComponentFormViewModel vm)
        {
            var (imagePath, imageError) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "components");
            if (imageError != null) ModelState.AddModelError(nameof(vm.ImageFile), imageError);
            if (imagePath != null) vm.ImageUrl = imagePath;

            if (!ModelState.IsValid)
            {
                vm.ComponentTypes = await GetTypeOptions(vm.ComponentTypeId);
                return View(vm);
            }

            var dto = _mapper.Map<CarComponentCreateDto>(vm);
            var (created, error) = await _service.CreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                vm.ComponentTypes = await GetTypeOptions(vm.ComponentTypeId);
                return View(vm);
            }

            TempData["Success"] = $"Component '{created!.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = _mapper.Map<CarComponentFormViewModel>(dto);
            vm.ComponentTypes = await GetTypeOptions(dto.ComponentTypeId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarComponentFormViewModel vm)
        {
            var oldImageUrl = vm.ImageUrl;
            var (imagePath, imageError) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "components");
            if (imageError != null) ModelState.AddModelError(nameof(vm.ImageFile), imageError);
            if (imagePath != null) vm.ImageUrl = imagePath;

            if (!ModelState.IsValid)
            {
                vm.ComponentTypes = await GetTypeOptions(vm.ComponentTypeId);
                return View(vm);
            }

            var dto = _mapper.Map<CarComponentCreateDto>(vm);
            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok && error == null) return NotFound();
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                vm.ComponentTypes = await GetTypeOptions(vm.ComponentTypeId);
                return View(vm);
            }

            // a new image replaced an old one
            if (imagePath != null) ImageUploadHelper.DeleteIfLocal(oldImageUrl, _env.WebRootPath);

            TempData["Success"] = "Component updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<CarComponentViewModel>(dto));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();

            if (dto != null) ImageUploadHelper.DeleteIfLocal(dto.ImageUrl, _env.WebRootPath);

            TempData["Success"] = "Component deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetTypeOptions(int? selected = null)
        {
            var types = await _typeService.GetAllAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = selected.HasValue && t.Id == selected.Value
            });
        }
    }
}
