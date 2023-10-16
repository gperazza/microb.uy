using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

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
        public async Task<TenantInstanceModel> CreateInstance(CreateInstanceRequestDto request, string userEmail)
        {
            TenantInstanceModel newInstance = _mapper.Map<TenantInstanceModel>(request);
            UserModel userExist = _context.User.Where(x=>x.Email == userEmail && x.TenantInstanceId == 0).FirstOrDefault();

            if (userExist == null)
                return null;

            newInstance.InstanceAdministrators = new List<UserModel> { userExist };

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
            var instances = _context.TenantInstances.Where(x => x.Activo).ToList();
            return instances;
        }

        /// <summary>
        /// Obtiene la instancia que machea con el id
        /// </summary>
        /// <param name="instanceId">Id de la instancia</param>
        /// <returns>Si la instancia con el id existe la devuelve</returns>
        public async Task<TenantInstanceModel> GetInstance(int instanceId) 
        {
            var instance = _context.TenantInstances.FirstOrDefault(x => x.TenantInstanceId == instanceId && x.Activo); 
            return instance;
        }

        /// <summary>
        /// Modifica la Instancia 
        /// </summary>
        /// <param name="instance">Datos de la instancia modificada</param>
        /// <returns>Devuelve la instancia modificada</returns>
        public async Task<TenantInstanceModel> ModifyInstance( TenantInstanceModel instance)
        {
            var instanceExist = _context.TenantInstances.FirstOrDefault(x => x.TenantInstanceId == instance.TenantInstanceId && x.Activo);
            
            if (instanceExist == null)
                return null;

           return _context.Update(instance).Entity;

        }
    }
}