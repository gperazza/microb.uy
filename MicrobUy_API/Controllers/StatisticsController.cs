using AutoMapper;
using MicrobUy_API.Dtos.StatisticsDto;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.StatisticsService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStatisticsService _statisticsService;
        private readonly IInstanceService _tenantInstanceService;

        public StatisticsController(IMapper mapper, IStatisticsService statisticsService, IInstanceService tenantInstanceService)
        {
            _mapper = mapper;
            _statisticsService = statisticsService;
            _tenantInstanceService = tenantInstanceService; 
        }
        /// <summary>
        /// Obtiene la cantidad de usuarios de todas las instacias
        /// </summary>
        /// <returns> </returns>
        [HttpGet("CantUsersAllTenant")]
        public async Task<TotalAndPercentDto> CantUsersAllTenant()
        {
            return await _statisticsService.CantUsersAllTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios de una instancia pasada por header (tenant)
        /// </summary>
        /// <returns> </returns>
        [HttpGet("CantUsersByTenant")]
        public async Task<TotalAndPercentDto> CantUsersByTenant()
        {
            return await _statisticsService.CantUsersByTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios nuevos este mes y su porcentaje contra el mes anterior, para todas las instacias
        /// </summary>
        /// <returns> </returns>
        [HttpGet("CantUsersThisMonthAllTenant")]
        public async Task<TotalAndPercentDto> CantUsersThisMonthAllTenant()
        {
            return await _statisticsService.CantUsersThisMonthAllTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios nuevos este mes y su porcentaje contra el mes anterior, para la instianca pasada por header (tenant)
        /// </summary>
        /// <returns> </returns>
        [HttpGet("CantUsersThisMonthByTenant")]
        public async Task<TotalAndPercentDto> CantUsersThisMonthByTenant()
        {
            return await _statisticsService.CantUsersThisMonthByTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios para cada ciudad de todas las instacias. Maximo 20 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("CantUsersByCityAllTenant")]
        public async Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop)
        {
            return await _statisticsService.CantUsersByCityAllTenant(cantTop);
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios para cada ciudad de La instancia pasada por header (tenant). Maximo 20 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("CantUsersByCityByTenant")]
        public async Task<List<UserCityDto>> CantUsersByCityByTenant(int cantTop)
        {
            return await _statisticsService.CantUsersByCityByTenant(cantTop);
        }

        /// <summary>
        /// Devuelve metricas generales de todas las instancias en la plataforma. Maximo 20 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("InstanceMetricsAllTenant")]
        public async Task<List<InstanceMetricsDto>> InstanceMetricsAllTenant(int cantTop)
        {
            return await _statisticsService.InstanceMetricsAllTenant(cantTop);
        }

        /// <summary>
        /// Devuelve metricas generales de una instancia en la plataforma pasada por header (tenant). Maximo 20 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("InstanceMetricsByTenant")]
        public async Task<List<InstanceMetricsDto>> InstanceMetricsByTenant(int cantTop)
        {
            return await _statisticsService.InstanceMetricsByTenant(cantTop);
        }

        /// <summary>
        /// Devuelve metricas sobre cuantos usuarios neuvos y instancias hay cada mes, para todas las instancias en la plataforma. Maximo 12 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("NewMonthlyRegistrationsAllTenant")]
        public async Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsAllTenant(int cantTop)
        {
            return await _statisticsService.NewMonthlyRegistrationsAllTenant(cantTop);
        }

        /// <summary>
        /// Devuelve metricas sobre cuantos usuarios neuvos y instancias hay cada mes, para una instancia en la plataforma pasada por header (tenant). Maximo 12 Registros
        /// </summary>
        /// <param int="cantTop"></param>
        /// <returns> </returns>
        [HttpGet("NewMonthlyRegistrationsByTenant")]
        public async Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsByTenant(int cantTop)
        {
            return await _statisticsService.NewMonthlyRegistrationsByTenant(cantTop);
        }
    }
}
