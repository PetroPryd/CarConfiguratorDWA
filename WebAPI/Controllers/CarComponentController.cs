using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/carcomponent")]
    public class CarComponentController : ControllerBase
    {
        private readonly CarComponentService _service;

        public CarComponentController(CarComponentService service)
        {
            _service = service;
        }

        // GET /api/carcomponent
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // GET /api/carcomponent/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound($"CarComponent with id={id} not found.");
            return Ok(item);
        }

        // GET /api/carcomponent/search?name=...&componentTypeId=...&page=1&count=10
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] int? componentTypeId,
            [FromQuery] int page = 1,
            [FromQuery] int count = 10)
        {
            var result = await _service.SearchAsync(name, componentTypeId, page, count);
            return Ok(result);
        }

        // POST /api/carcomponent
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CarComponentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (created, error) = await _service.CreateAsync(dto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }

        // PUT /api/carcomponent/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CarComponentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok && error == null) return NotFound($"CarComponent with id={id} not found.");
            if (error != null) return BadRequest(error);

            return NoContent();
        }

        // DELETE /api/carcomponent/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound($"CarComponent with id={id} not found.");
            return NoContent();
        }
    }
}
