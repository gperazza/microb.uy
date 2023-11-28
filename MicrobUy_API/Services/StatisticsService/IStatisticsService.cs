﻿using MicrobUy_API.Dtos.StatisticsDto;

namespace MicrobUy_API.Services.StatisticsService
{
    public interface IStatisticsService
    {
        Task<TotalAndPercentDto> CantUsersAllTenant();
        Task<TotalAndPercentDto> CantUsersThisMonthAllTenant();
        Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop);
    }
}
