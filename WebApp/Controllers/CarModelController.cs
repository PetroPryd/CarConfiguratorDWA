using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels.CarModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarModelController : Controller
    {
        private readonly CarModelService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CarModelController(CarModelService service, IWebHostEnvironment env, IMapper mapper)
        {
            _service = service;
            _env = env;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllAsync();
            return View(_mapper.Map<List<CarModelViewModel>>(dtos));
        }

        public IActionResult Create()
            => View(new CarModelFormViewModel { Year = DateTime.Now.Year });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarModelFormViewModel vm)
        {
            var (imagePath, imageError) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "cars");
            if (imageError != null) ModelState.AddModelError(nameof(vm.ImageFile), imageError);
            if (imagePath != null) vm.ImageUrl = imagePath;

            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<CarModelCreateDto>(vm);
            var (created, error) = await _service.CreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }
            TempData["Success"] = $"Car model '{created!.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<CarModelFormViewModel>(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarModelFormViewModel vm)
        {
            var oldImageUrl = vm.ImageUrl;
            var (imagePath, imageError) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "cars");
            if (imageError != null) ModelState.AddModelError(nameof(vm.ImageFile), imageError);
            if (imagePath != null) vm.ImageUrl = imagePath;

            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<CarModelCreateDto>(vm);
            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok && error == null) return NotFound();
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }
            if (imagePath != null) ImageUploadHelper.DeleteIfLocal(oldImageUrl, _env.WebRootPath);

            TempData["Success"] = "Car model updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<CarModelViewModel>(dto));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            var (ok, error) = await _service.DeleteAsync(id);
            if (!ok && error == null) return NotFound();
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            if (dto != null) ImageUploadHelper.DeleteIfLocal(dto.ImageUrl, _env.WebRootPath);

            TempData["Success"] = "Car model deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
