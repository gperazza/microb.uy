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
        /// Obtiene la cantidad de usuarios de toda la plataforma
        /// </summary>
        /// <returns> </returns>
        [HttpPost("CantUsersAllTenant")]
        public async Task<TotalAndPercentDto> CantUsersAllTenant()
        {
            return await _statisticsService.CantUsersAllTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios nuevos este mes y su porcentaje contra el mes anterior
        /// </summary>
        /// <returns> </returns>
        [HttpPost("CantUsersThisMonthAllTenant")]
        public async Task<TotalAndPercentDto> CantUsersThisMonthAllTenant()
        {
            return await _statisticsService.CantUsersThisMonthAllTenant();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios para cada ciudad de toda la plataforma
        /// </summary>
        /// <returns> </returns>
        [HttpPost("CantUsersByCityAllTenant")]
        public async Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop)
        {
            return await _statisticsService.CantUsersByCityAllTenant(cantTop);
        }
    }
}
