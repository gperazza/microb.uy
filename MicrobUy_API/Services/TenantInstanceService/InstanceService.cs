using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Dtos.Enums;
using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;


namespace MicrobUy_API.Services.TenantInstanceService
{
    public class InstanceService : IInstanceService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;

        public InstanceService(TenantAplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Crea una nueva instancia
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Devuelve la Instancia creada</returns>
        public async Task<TenantInstanceModel> CreateInstance(CreateInstanceRequestDto request, string userName)
        {
            TenantInstanceModel newInstance = _mapper.Map<TenantInstanceModel>(request);
            UserModel userExist = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (userExist == null)
                return null;

            newInstance.InstanceAdministrators = new List<UserModel> { userExist };
            newInstance.CreationDate = DateTime.Now;

            await _context.AddAsync(newInstance);
            await _context.SaveChangesAsync();

            return newInstance;
        }

        /// <summary>
        /// Obtiene todas las instancias existentes
        /// </summary>
        /// <returns>Devuelve todas las instancias existentes</returns>
        public async Task<IEnumerable<TenantInstanceModel>> GetAllActiveInstances()
        {
            var instances = _context.TenantInstances.Include(x => x.InstanceAdministrators).Include(x => x.Tematica).Where(x => x.Activo).ToList();
            return instances;
        }

        /// <summary>
        /// Obtiene la instancia que machea con el id
        /// </summary>
        /// <param name="instanceId">Id de la instancia</param>
        /// <returns>Si la instancia con el id existe la devuelve</returns>
        public async Task<TenantInstanceModel> GetInstance(int instanceId)
        {
            var instance = _context.TenantInstances.Include(x => x.InstanceAdministrators).Include(x => x.Tematica).FirstOrDefault(x => x.TenantInstanceId == instanceId && x.Activo);
            return instance;
        }

        /// <summary>
        /// Devuelve la instancia que coincida con el dominio dado
        /// </summary>
        /// <param name="domain">dominio de la instancia</param>
        /// <returns>retorna una instancia</returns>
        public async Task<TenantInstanceModel> GetInstanceByDomain(string domain)
        {
            var instance = _context.TenantInstances.Include(x => x.InstanceAdministrators).Include(x => x.Tematica).FirstOrDefault(x => x.Dominio == domain && x.Activo);
            return instance;
        }

        /// <summary>
        /// Modifica la Instancia 
        /// </summary>
        /// <param name="instance">Datos de la instancia modificada</param>
        /// <returns>Retorna la instancia modificada</returns>
        public async Task<int> ModifyInstance(ModifyInstanceRequest instance)
        {

            TenantInstanceModel newInstance = _mapper.Map<TenantInstanceModel>(instance);

            var result = _context.TenantInstances.Where(b => b.TenantInstanceId == _context._tenant)
                .ExecuteUpdate(setters => setters.SetProperty(b => b.Nombre, newInstance.Nombre)
                                                 .SetProperty(b => b.Description, newInstance.Description)
                                                 .SetProperty(b => b.Logo, newInstance.Logo)
                                                 .SetProperty(b => b.EsquemaColores, newInstance.EsquemaColores)
                                                 .SetProperty(b => b.Privacidad, newInstance.Privacidad));

            if (result == 1)
            {
                var check = _context.TenantInstances.Include(b => b.Tematica).FirstOrDefault(b => b.TenantInstanceId == _context._tenant);
                if (check.Tematica != newInstance.Tematica)
                {
                    check.Tematica = newInstance.Tematica;
                    _context.Update(check);
                    _context.SaveChanges();
                }
            }
            return result;
        }

        /// <summary>
        /// Borra lógicamente la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue borrada correctamente</returns>
        public async Task<int> DeleteInstance()
        {

            return _context.TenantInstances.Where(b => b.TenantInstanceId == _context._tenant &&
            b.Activo || !b.Activo && b.ActiveDescription == ActiveDescription.ActivationPending || b.ActiveDescription == ActiveDescription.ActivatedByAdmin || b.ActiveDescription == ActiveDescription.DisabledByAdmin)
               .ExecuteUpdate(setters => setters.SetProperty(b => b.Activo, false)
                                                .SetProperty(b => b.ActiveDescription, ActiveDescription.Deleted));
        }

        /// <summary>
        /// Activa la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue activada correctamente</returns>
        public async Task<int> ActiveInstance()
        {
         
            return _context.TenantInstances.Where(b => b.TenantInstanceId == _context._tenant && !b.Activo && b.ActiveDescription == ActiveDescription.ActivationPending || b.ActiveDescription == ActiveDescription.DisabledByAdmin)
                           .ExecuteUpdate(setters => setters.SetProperty(b => b.Activo, true)
                                                            .SetProperty(b => b.ActiveDescription, ActiveDescription.ActivatedByAdmin));
        }

        /// <summary>
        /// Desactiva la Instancia 
        /// </summary>
        /// <returns>Retorna 1 si la instancia fue desactivada correctamente</returns>
        public async Task<int> DisableInstance()
        {
           
            return _context.TenantInstances.Where(b => b.TenantInstanceId == _context._tenant && b.Activo)
                          .ExecuteUpdate(setters => setters.SetProperty(b => b.Activo, false)
                                                           .SetProperty(b => b.ActiveDescription, ActiveDescription.DisabledByAdmin));
        }
    }
}