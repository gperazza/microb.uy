using MicrobUy_API.Dtos.StatisticsDto;

namespace MicrobUy_API.Services.StatisticsService
{
    public interface IStatisticsService
    {
        Task<TotalAndPercentDto> CantUsersAllTenant();
        Task<TotalAndPercentDto> CantUsersByTenant();
        Task<TotalAndPercentDto> CantUsersThisMonthAllTenant();
        Task<TotalAndPercentDto> CantUsersThisMonthByTenant();
        Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop);
        Task<List<UserCityDto>> CantUsersByCityByTenant(int cantTop);
        Task<List<InstanceMetricsDto>> InstanceMetricsAllTenant(int cantTop);
        Task<List<InstanceMetricsDto>> InstanceMetricsByTenant(int cantTop);
        Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsAllTenant(int cantTop);
        Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsByTenant(int cantTop);
    }
}
