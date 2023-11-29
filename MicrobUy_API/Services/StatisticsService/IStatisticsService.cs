using MicrobUy_API.Dtos.StatisticsDto;

namespace MicrobUy_API.Services.StatisticsService
{
    public interface IStatisticsService
    {
        Task<TotalAndPercentDto> CantUsersAllTenant();
        Task<TotalAndPercentDto> CantUsersThisMonthAllTenant();
        Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop);
        Task<List<InstanceMetricsDto>> InstanceMetricsAllTenant(int cantTop);
        Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsAllTenant(int cantTop);
    }
}
