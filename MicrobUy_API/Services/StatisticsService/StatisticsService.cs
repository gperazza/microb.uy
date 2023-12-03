using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos.StatisticsDto;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MicrobUy_API.Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;
        //Limita la cantidad de registros maximos que se pueden pedir
        const int LIMITDATA = 15;

        public StatisticsService(TenantAplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<TotalAndPercentDto> CantUsersAllTenant()
        {
            int CantUserTotal = _context.User.IgnoreQueryFilters().Count();

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            TotalAndPercentDto userStatics = new TotalAndPercentDto(CantUserTotal, 100);
            return userStatics;
        }
        public async Task<TotalAndPercentDto> CantUsersByTenant()
        {
            int CantUserTotal = _context.User.Count();

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Obtener la cantidad de usuarios creados este mes
            int CantUserThisMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfMonth && u.CreationDate <= DateTime.Now);

            int percentageActualMonth = CantUserTotal == 0 ? 0 : CantUserThisMonth * 100 / CantUserTotal;

            TotalAndPercentDto userStatics = new TotalAndPercentDto(CantUserTotal, percentageActualMonth);
            return userStatics;
        }
        public async Task<TotalAndPercentDto> CantUsersThisMonthByTenant()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            int cantUsersThisMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfMonth && u.CreationDate <= DateTime.Now);

            // Obtener la cantidad de usuarios creados el mes pasado
            int cantUsersLastMonth = _context.User
                .Count(u => u.CreationDate >= firstDayOfLastMonth && u.CreationDate < firstDayOfMonth);

            int percentageVsLastMonth = cantUsersLastMonth == 0 ? 0 : (cantUsersThisMonth - cantUsersLastMonth) * 100 / cantUsersLastMonth;

            TotalAndPercentDto userStatics = new TotalAndPercentDto(cantUsersThisMonth, percentageVsLastMonth);
            return userStatics;
        }
        public async Task<TotalAndPercentDto> CantUsersThisMonthAllTenant()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            int cantUsersThisMonth = _context.User.IgnoreQueryFilters()
                .Count(u => u.CreationDate >= firstDayOfMonth && u.CreationDate <= DateTime.Now);

            // Obtener la cantidad de usuarios creados el mes pasado
            int cantUsersLastMonth = _context.User.IgnoreQueryFilters()
                .Count(u => u.CreationDate >= firstDayOfLastMonth && u.CreationDate < firstDayOfMonth);

            int percentageVsLastMonth = cantUsersLastMonth == 0 ? 0 : (cantUsersThisMonth - cantUsersLastMonth) * 100 / cantUsersLastMonth;

            TotalAndPercentDto userStatics = new TotalAndPercentDto(cantUsersThisMonth, percentageVsLastMonth);
            return userStatics;
        }
        public async Task<List<UserCityDto>> CantUsersByCityAllTenant(int cantTop)
        {
            if (cantTop == 0) {
                cantTop = 5;
            }
            else if (cantTop > LIMITDATA)
            {
                cantTop = LIMITDATA;
            }
            var userCityDtos = await _context.User.IgnoreQueryFilters()
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
        public async Task<List<UserCityDto>> CantUsersByCityByTenant(int cantTop)
        {
            if (cantTop == 0)
            {
                cantTop = 5;
            }
            else if (cantTop > LIMITDATA)
            {
                cantTop = LIMITDATA;
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
        public async Task<List<InstanceMetricsDto>> InstanceMetricsAllTenant(int cantTop)
        {
            if (cantTop == 0)
            {
                cantTop = 5;
            } else if (cantTop > LIMITDATA)
            {
                cantTop = LIMITDATA;
            }

            var totalUsersAllInstances = _context.User.IgnoreQueryFilters().Count();

            var instanceMetrics = await _context.TenantInstances.IgnoreQueryFilters()
                .OrderByDescending(i => i.CreationDate)
                .Take(cantTop)
                .Select(instance => new InstanceMetricsDto
                {
                    InstanceName = instance.Nombre,
                    TotalPost = _context.Post.IgnoreQueryFilters().Count(p => p.TenantInstanceId == instance.TenantInstanceId),
                    TotalUsers = _context.User.IgnoreQueryFilters().Count(u => u.TenantInstanceId == instance.TenantInstanceId),
                    PercentUserPlatform = totalUsersAllInstances == 0
                        ? 0
                        : (float)_context.User.IgnoreQueryFilters().Count(u => u.TenantInstanceId == instance.TenantInstanceId) * 100 / totalUsersAllInstances,
                    CreationDate = instance.CreationDate
                })
                .ToListAsync();

            // Calcular PositionTop basado en PercentUserPlatform y TotalPost
            instanceMetrics = instanceMetrics
                .OrderByDescending(im => im.PercentUserPlatform)
                .ThenByDescending(im => im.TotalPost)
                .ToList();

            for (int i = 0; i < instanceMetrics.Count; i++)
            {
                instanceMetrics[i].PositionTop = i + 1;
            }

            instanceMetrics = instanceMetrics.OrderBy(im => im.PositionTop).ToList();

            return instanceMetrics;
        }

        public async Task<List<InstanceMetricsDto>> InstanceMetricsByTenant(int cantTop)
        {
            if (cantTop == 0)
            {
                cantTop = 5;
            }
            else if (cantTop > LIMITDATA)
            {
                cantTop = LIMITDATA;
            }

            var totalUsersAllInstances = _context.User.Count();

            var instanceMetrics = await _context.TenantInstances
                .OrderByDescending(i => i.CreationDate)
                .Take(cantTop)
                .Select(instance => new InstanceMetricsDto
                {
                    InstanceName = instance.Nombre,
                    TotalPost = _context.Post.Count(p => p.TenantInstanceId == instance.TenantInstanceId),
                    TotalUsers = _context.User.Count(u => u.TenantInstanceId == instance.TenantInstanceId),
                    PercentUserPlatform = totalUsersAllInstances == 0
                        ? 0
                        : (float)_context.User.Count(u => u.TenantInstanceId == instance.TenantInstanceId) * 100 / totalUsersAllInstances,
                    CreationDate = instance.CreationDate
                })
                .ToListAsync();

            // Calcular PositionTop basado en PercentUserPlatform y TotalPost
            instanceMetrics = instanceMetrics
                .OrderByDescending(im => im.PercentUserPlatform)
                .ThenByDescending(im => im.TotalPost)
                .ToList();

            for (int i = 0; i < instanceMetrics.Count; i++)
            {
                instanceMetrics[i].PositionTop = i + 1;
            }

            instanceMetrics = instanceMetrics.OrderBy(im => im.PositionTop).ToList();

            return instanceMetrics;
        }

        public Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsAllTenant(int cantTop)
        {
            if (cantTop == 0)
            {
                cantTop = 5;
            }
            else if (cantTop > 12)
            {
                cantTop = 12;
            }

            var currentDate = DateTime.UtcNow; 

            var monthlyRegistrations = Enumerable.Range(0, 12)
                .Select(offset => currentDate.AddMonths(-offset)) // Obtener los últimos 12 meses
                .Select(month => new NewMonthlyRegistrationsDto
                {
                    Month = month.ToString("MMMM yyyy", CultureInfo.InvariantCulture), // Formatear el mes
                    NewTotalUser = _context.User.IgnoreQueryFilters().Count(u => u.CreationDate.Year == month.Year && u.CreationDate.Month == month.Month),
                    NewTotalInstance = _context.TenantInstances.IgnoreQueryFilters().Count(i => i.CreationDate.Year == month.Year && i.CreationDate.Month == month.Month)
                })
                //.OrderByDescending(dto => dto.NewTotalUser + dto.NewTotalInstance) 
                .Take(cantTop)
                .ToList();

            return Task.FromResult(monthlyRegistrations);
        }
        public Task<List<NewMonthlyRegistrationsDto>> NewMonthlyRegistrationsByTenant(int cantTop)
        {
            if (cantTop == 0)
            {
                cantTop = 5;
            }
            else if (cantTop > 12)
            {
                cantTop = 12;
            }

            var currentDate = DateTime.UtcNow;

            var monthlyRegistrations = Enumerable.Range(0, 12)
                .Select(offset => currentDate.AddMonths(-offset)) // Obtener los últimos 12 meses
                .Select(month => new NewMonthlyRegistrationsDto
                {
                    Month = month.ToString("MMMM yyyy", CultureInfo.InvariantCulture), // Formatear el mes
                    NewTotalUser = _context.User.Count(u => u.CreationDate.Year == month.Year && u.CreationDate.Month == month.Month),
                    NewTotalInstance = _context.TenantInstances.Count(i => i.CreationDate.Year == month.Year && i.CreationDate.Month == month.Month)
                })
                .OrderByDescending(dto => dto.NewTotalUser + dto.NewTotalInstance)
                .Take(cantTop)
                .ToList();

            return Task.FromResult(monthlyRegistrations);
        }
    }
}
