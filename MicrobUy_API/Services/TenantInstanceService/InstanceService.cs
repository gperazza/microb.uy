using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.TenantInstanceService
{
    public class InstanceService : IInstanceService
    {
        private readonly TenantAplicationDbContext _context;

        public InstanceService(TenantAplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea una nueva instancia
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Devuelve la Instancia creada</returns>
        public async Task<TenantInstanceModel> CreateInstance(CreateInstanceRequestDto request)
        {
            var newInstance = new TenantInstanceModel
            {
                Nombre = request.Nombre,
                Logo = request.Logo,
                Dominio = request.Dominio,
                Tematica = request.Tematica,
                Privacidad = request.Privacidad,
                EsquemaColores = request.EsquemaColores,
                Activo = request.Activo,
            };

            await _context.AddAsync(newInstance);
            await _context.SaveChangesAsync();

            return newInstance;
        }

        /// <summary>
        /// Obtiene todas las instancias existentes
        /// </summary>
        /// <returns>Devuelve todas las instancias existentes</returns>
        public async Task<IEnumerable<TenantInstanceModel>> GetAllInstances()
        {
            var instances = _context.TenantInstances.ToList();
            return instances;
        }
    }
}