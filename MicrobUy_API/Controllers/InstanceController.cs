using FluentValidation;
using FluentValidation.Results;
using MicrobUy_API.Dtos;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstanceController : ControllerBase
    {
        private readonly IInstanceService _tenantInstanceService;
        private IValidator<CreateInstanceRequestDto> _validator;

        public InstanceController(IValidator<CreateInstanceRequestDto> validator, IInstanceService tenantInstanceService)
        {
            _validator = validator;
            _tenantInstanceService = tenantInstanceService;
        }

        /// <summary>
        /// Obtienen todoas las instancias existentes
        /// </summary>
        /// <returns>Retorna una lista de todas las instancias existentes</returns>
        [HttpGet("GetActiveInstances")]
        public async Task<IActionResult> GetAllActiveInstancesAsync()
        {
            var instances = await _tenantInstanceService.GetAllActiveInstances();
            return Ok(instances);
        }

        /// <summary>
        /// Crea una nueva Instancia
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Retorna la Instancia creada</returns>
        [HttpPost("CreateInstance")]
        public async Task<IActionResult> PostAsync(CreateInstanceRequestDto request, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            ValidationResult res = await _validator.ValidateAsync(request);

            if (res.IsValid)
            {
                var result = await _tenantInstanceService.CreateInstance(request, userName);
                if (result == null)
                {

                    listOfErrors.Add("Error al crear la instancia intentnelo nuevamente");
                    errors = listOfErrors.Select(x => x);
                    return BadRequest(new UserRegistrationResponseDto { Errors = errors });

                }
                return Ok(result);
            }

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Obtener Instancia por Id
        /// </summary>
        /// <param name="instanceId">Id de la instancia a obtener</param>
        /// <returns>retorna la instancia que machea con el id</returns>
        [HttpGet("GetInstanceById")]
        public async Task<IActionResult> GetInstanceById(int instanceId)
        {
            var instance = await _tenantInstanceService.GetInstance(instanceId);
            return Ok(instance);
        }

        /// <summary>
        /// Crea una nueva Instancia
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>Retorna la Instancia creada</returns>
        [HttpPut("ModifyInstance")]
        public async Task<IActionResult> ModifyInstance([FromBody] ModifyInstanceRequest instance)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            var result = await _tenantInstanceService.ModifyInstance(instance);
            if (result == null)
            {

                listOfErrors.Add("La Instancia no existe");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });

            }
            return Ok(result);
        }

        /// <summary>
        /// Devuelve una instancia en base a un dominio
        /// </summary>
        /// <param name="domain">El dominio de la instancia que se desea obtener</param>
        /// <returns>Devuelve la instancia que coincide con el dominio</returns>
        [HttpGet("GetInstanceByDomain")]
        public async Task<IActionResult> GetInstanceByDomain(string domain)
        {
            var instance = await _tenantInstanceService.GetInstanceByDomain(domain);
            return Ok(instance);
        }

        /// <summary>
        /// Borra lógicamente la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue borrada correctamente</returns>
        [HttpPut("DeleteInstance")]
        public async Task<IActionResult> DeleteInstance()
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _tenantInstanceService.DeleteInstance();
            if (result != 1)
            {
                listOfErrors.Add("No fue posible borrar la instancia");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });
            }
            return Ok(result);
        }

        /// <summary>
        /// Activa la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue activada correctamente</returns>
        [HttpPut("ActiveInstance")]
        public async Task<IActionResult> ActiveInstance()
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _tenantInstanceService.ActiveInstance();
            if (result != 1)
            {
                listOfErrors.Add("No fue posible activar la instancia");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });
            }
            return Ok(result);
        }

        /// <summary>
        /// Desactiva la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue desactivada correctamente</returns>
        [HttpPut("DisableInstance")]
        public async Task<IActionResult> DisableInstance()
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            int result = await _tenantInstanceService.DisableInstance();
            if (result != 1)
            {
                listOfErrors.Add("No fue posible desactivar la instancia");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserRegistrationResponseDto { Errors = errors });
            }
            return Ok(result);
        }
    }
}