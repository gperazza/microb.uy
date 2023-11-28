using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos.StatisticsDto;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;

        public StatisticsService(TenantAplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<TotalAndPercentDto> CantUsersAllTenant()
        {
            int CantUserTotal = _context.User.Count();

            // Obtener la fecha del primer día del mes actual
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Obtener la cantidad de usuarios creados este mes
            int CantUserThisMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfMonth && u.CreationDate <= DateTime.Now);

            // Calcular el porcentaje
            int percentageActualMonth = CantUserTotal == 0 ? 0 : CantUserThisMonth * 100 / CantUserTotal;

            TotalAndPercentDto userStatics = new TotalAndPercentDto(CantUserTotal, percentageActualMonth);
            return userStatics;
        }

        public async Task<TotalAndPercentDto> CantUsersThisMonthAllTenant()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            int cantUsersThisMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfMonth && u.CreationDate <= DateTime.Now);

            // Obtener la cantidad de usuarios creados el mes pasado
            int cantUsersLastMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfLastMonth && u.CreationDate < firstDayOfMonth);

            int percentageVsLastMonth = cantUsersLastMonth == 0 ? 0 : (cantUsersThisMonth - cantUsersLastMonth) * 100 / cantUsersLastMonth;

            // Crear el objeto TotalAndPercentDto
            TotalAndPercentDto userStatics = new TotalAndPercentDto(cantUsersThisMonth, percentageVsLastMonth);
            return userStatics;
        }

        public async Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop)
        {
            if (cantTop == 0) {
                cantTop = 5;
            }
            var userCityDtos = await _context.User
                .GroupBy(u => u.City.Name)
                .OrderByDescending(g => g.Count())
                .Take(cantTop)
                .Select(g => new UserCityDto
                {
                    City = g.Key,
                    UserId = g.Count()
                })
                .ToListAsync();

            return userCityDtos;
        }
    }
}
