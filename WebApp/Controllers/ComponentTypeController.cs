using AutoMapper;
using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.ComponentTypes;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComponentTypeController : Controller
    {
        private readonly ComponentTypeService _service;
        private readonly IMapper _mapper;

        public ComponentTypeController(ComponentTypeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllAsync();
            return View(_mapper.Map<List<ComponentTypeViewModel>>(dtos));
        }

        public IActionResult Create() => View(new ComponentTypeFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComponentTypeFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<ComponentTypeCreateDto>(vm);
            var (created, error) = await _service.CreateAsync(dto);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }
            TempData["Success"] = $"Component type '{created!.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<ComponentTypeFormViewModel>(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComponentTypeFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<ComponentTypeCreateDto>(vm);
            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok && error == null) return NotFound();
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }
            TempData["Success"] = "Component type updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<ComponentTypeViewModel>(dto));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (ok, error) = await _service.DeleteAsync(id);
            if (!ok && error == null) return NotFound();
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }
            TempData["Success"] = "Component type deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
