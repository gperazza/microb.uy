using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.TenantInstanceService
{
    public interface IInstanceService
    {
        /// <summary>
        /// Crea una nueva instancia
        /// </summary>
        /// <param name="request">Instance data</param>
        /// <param name="userEmail">Admin user of Instance</param>
        /// <returns>Devuelve la Instancia creada</returns>
        Task<TenantInstanceModel> CreateInstance(CreateInstanceRequestDto request, string userEmail);

        /// <summary>
        /// Obtiene todas las instancias existentes
        /// </summary>
        /// <returns>Devuelve todas las instancias existentes</returns>
        Task<IEnumerable<TenantInstanceModel>> GetAllActiveInstances();

        /// <summary>
        /// Obtiene la instancia que machea con el id
        /// </summary>
        /// <param name="instanceId">Id de la instancia</param>
        /// <returns>Si la instancia con el id existe la devuelve</returns>
        Task<TenantInstanceModel> GetInstance(int instanceId);

        /// <summary>
        /// Modifica la Instancia 
        /// </summary>
        /// <param name="instance">Datos de la instancia modificada</param>
        /// <returns>Devuelve la instancia modificada</returns>
        Task<int> ModifyInstance(ModifyInstanceRequest instance);
    }
}