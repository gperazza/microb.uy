using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.TenantInstanceService
{
    public interface IInstanceService
    {
        /// <summary>
        /// Crea una nueva instancia
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Devuelve la Instancia creada</returns>
        Task<TenantInstanceModel> CreateInstance(CreateInstanceRequestDto request);

        /// <summary>
        /// Obtiene todas las instancias existentes
        /// </summary>
        /// <returns>Devuelve todas las instancias existentes</returns>
        Task<IEnumerable<TenantInstanceModel>> GetAllInstances();
    }
}