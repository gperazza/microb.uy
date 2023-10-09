using FluentValidation;
using FluentValidation.Results;
using MicrobUy_API.Dtos;
using MicrobUy_API.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;
using System;

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
        [HttpGet("GetInstances")]
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
        [HttpPost("CreateInstance")]
        public async Task<IActionResult> PostAsync(CreateInstanceRequestDto request)
        {
            ValidationResult res = await _validator.ValidateAsync(request);

            if (res.IsValid)
            {
                var result = await _tenantInstanceService.CreateInstance(request);
                return Ok(result);
            }

            return BadRequest(res.Errors); 
        }
    }
}