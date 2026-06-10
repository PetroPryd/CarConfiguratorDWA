using CarConfigurator.Business.DTOs.Create;
using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/componenttype")]
    public class ComponentTypeController : ControllerBase
    {
        private readonly ComponentTypeService _service;

        public ComponentTypeController(ComponentTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound($"ComponentType with id={id} not found.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComponentTypeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (created, error) = await _service.CreateAsync(dto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ComponentTypeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok && error == null) return NotFound($"ComponentType with id={id} not found.");
            if (error != null) return BadRequest(error);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _service.DeleteAsync(id);
            if (!ok && error == null) return NotFound($"ComponentType with id={id} not found.");
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
