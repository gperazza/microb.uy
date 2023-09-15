using MicrobUy_API.Dtos;
using MicrobUy_API.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstanceController : ControllerBase
    {
        private readonly IInstanceService _tenantInstanceService;

        public InstanceController(IInstanceService tenantInstanceService)
        {
            _tenantInstanceService = tenantInstanceService;
        }
        /// <summary>
        /// Obtienen todoas las instancias existentes
        /// </summary>
        /// <returns>Retorna una lista de todas las instancias existentes</returns>
        [HttpGet]
        [Route("/GetInstances")]
        public async Task<IActionResult> GetAllInstancesAsync()
        {
            var instances = await _tenantInstanceService.GetAllInstances();
            return Ok(instances);
        }

        /// <summary>
        /// Crea una nueva Instancia
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Retorna la Instancia creada</returns>
        [HttpPost]
        [Route("/CreateInstance")]
        public async Task<IActionResult> PostAsync(CreateInstanceRequest request)
        {
            var result = await _tenantInstanceService.CreateInstance(request);
            return Ok(result);
        }
    }
}