using AutoMapper;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.CarComponents;

namespace WebApp.Controllers
{
    [Authorize]
    public class CatalogController : Controller
    {
        private const int PageSize = 9;
        private const string ModelKey = "buildModel";

        private readonly CarComponentService _service;
        private readonly ComponentTypeService _typeService;
        private readonly IMapper _mapper;

        public CatalogController(
            CarComponentService service,
            ComponentTypeService typeService,
            IMapper mapper)
        {
            _service = service;
            _typeService = typeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? search, int? componentTypeId, int page = 1)
            => View(await BuildListAsync(search, componentTypeId, page));

        // AJAX endpoint: returns ONLY the grid + pager as an HTML fragment.
        // The JS on the Index page fetches this and swaps it into the container.
        public async Task<IActionResult> ItemsPartial(string? search, int? componentTypeId, int page = 1)
            => PartialView("_Items", await BuildListAsync(search, componentTypeId, page));

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(_mapper.Map<CarComponentViewModel>(dto));
        }

        // Shared builder so Index and ItemsPartial stay in sync (DRY).
        private async Task<CarComponentListViewModel> BuildListAsync(string? search, int? componentTypeId, int page)
        {
            // restricts the catalogue to that car model's available components (or show everything)
            var carModelId = HttpContext.Session.GetInt32(ModelKey);
            var paged = await _service.SearchAsync(search, componentTypeId, page, PageSize, carModelId);
            var types = await _typeService.GetAllAsync();

            return new CarComponentListViewModel
            {
                Items = _mapper.Map<List<CarComponentViewModel>>(paged.Items),
                Search = search,
                ComponentTypeId = componentTypeId,
                Page = paged.Page,
                TotalPages = paged.TotalPages,
                TotalCount = paged.TotalCount,
                ComponentTypes = new SelectList(types, "Id", "Name", componentTypeId)
            };
        }
    }
}
