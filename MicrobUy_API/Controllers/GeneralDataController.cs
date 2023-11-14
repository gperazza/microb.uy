using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Services.GeneralDataService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GeneralDataController : ControllerBase
    {
        private readonly IGeneralDataService _generalDataService;

        public GeneralDataController(IGeneralDataService generalDataService)
        {

            _generalDataService = generalDataService;
        }

        [HttpPost("CreateTematicas")]
        public async Task<IActionResult> CreateTematicas(CreateTematicaRequestDto tematica)
        {
            var newTematica = await _generalDataService.CreateTematica(tematica);
            return Created("Tematica crerated", newTematica);
        }

        [HttpGet("GetTematicas")]
        public async Task<IActionResult> GetTematicas()
        {
            var tematicas = await _generalDataService.GetTematicas();
            return Ok(tematicas);
        }

        [HttpGet("GetTematicasById")]
        public async Task<IActionResult> GetTematicasById(int tematicaId)
        {
            var tematica = await _generalDataService.GetTematicaById(tematicaId);
            return Ok(tematica);
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _generalDataService.GetCities();
            return Ok(cities);
        }

    }
}
