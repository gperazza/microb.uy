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

        /// <summary>
        /// Crea tematicas para usar en las instancias
        /// </summary>
        /// <param name="tematica"></param>
        /// <returns></returns>
        [HttpPost("CreateTematicas")]
        public async Task<IActionResult> CreateTematicas(CreateTematicaRequestDto tematica)
        {
            var newTematica = await _generalDataService.CreateTematica(tematica);
            return Created("Tematica crerated", newTematica);
        }

        /// <summary>
        /// Obtiene las tematicas
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTematicas")]
        public async Task<IActionResult> GetTematicas()
        {
            var tematicas = await _generalDataService.GetTematicas();
            return Ok(tematicas);
        }

        /// <summary>
        /// Obtiene la tematica de id
        /// </summary>
        /// <param name="tematicaId"></param>
        /// <returns></returns>
        [HttpGet("GetTematicasById")]
        public async Task<IActionResult> GetTematicasById(int tematicaId)
        {
            var tematica = await _generalDataService.GetTematicaById(tematicaId);
            return Ok(tematica);
        }

        /// <summary>
        /// Obtiene una lista de ciudades
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _generalDataService.GetCities();
            return Ok(cities);
        }

    }
}
