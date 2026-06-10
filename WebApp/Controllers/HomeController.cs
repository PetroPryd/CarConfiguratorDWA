using AutoMapper;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.ViewModels.CarModels;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarModelService _carModelService;
        private readonly IMapper _mapper;

        public HomeController(CarModelService carModelService, IMapper mapper)
        {
            _carModelService = carModelService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var models = await _carModelService.GetAllAsync();
            return View(_mapper.Map<List<CarModelViewModel>>(models));
        }
    }
}
