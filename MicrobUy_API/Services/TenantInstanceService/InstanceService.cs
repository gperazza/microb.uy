using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
            UserModel userExist = _context.User.Where(x=>x.UserName == userName).FirstOrDefault();

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
            var instances = _context.TenantInstances.Include(x => x.InstanceAdministrators).Where(x => x.Activo).ToList();
            return instances;
        }

        /// <summary>
        /// Obtiene la instancia que machea con el id
        /// </summary>
        /// <param name="instanceId">Id de la instancia</param>
        /// <returns>Si la instancia con el id existe la devuelve</returns>
        public async Task<TenantInstanceModel> GetInstance(int instanceId) 
        {
            var instance = _context.TenantInstances.Include(x => x.InstanceAdministrators).FirstOrDefault(x => x.TenantInstanceId == instanceId && x.Activo); 
            return instance;
        }

        /// <summary>
        /// Devuelve la instancia que coincida con el dominio dado
        /// </summary>
        /// <param name="domain">dominio de la instancia</param>
        /// <returns>retorna una instancia</returns>
        public async Task<TenantInstanceModel> GetInstanceByDomain(string domain)
        {
            var instance = _context.TenantInstances.Include(x => x.InstanceAdministrators).FirstOrDefault(x => x.Dominio == domain && x.Activo);
            return instance;
        }

        /// <summary>
        /// Modifica la Instancia 
        /// </summary>
        /// <param name="instance">Datos de la instancia modificada</param>
        /// <returns>Devuelve la instancia modificada</returns>
        public async Task<int> ModifyInstance(ModifyInstanceRequest instance)
        {
            TenantInstanceModel newInstance = _mapper.Map<TenantInstanceModel>(instance);
            newInstance.TenantInstanceId = _context._tenant;
            newInstance.Activo = true;

            _context.Update(newInstance);
           return _context.SaveChanges();


        }
    }
}