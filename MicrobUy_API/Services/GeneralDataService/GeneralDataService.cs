using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.GeneralDataService
{
    public class GeneralDataService : IGeneralDataService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;

        public GeneralDataService(TenantAplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<TematicaModel> CreateTematica(CreateTematicaRequestDto tematica) {


            TematicaModel newTematica = _mapper.Map<TematicaModel>(tematica);
            TematicaModel tematicaExist = _context.Tematica.FirstOrDefault(x => x.Name == tematica.Name);

            if (tematicaExist != null)
                return null;
            
            await _context.AddAsync(newTematica);   
            _context.SaveChanges();

            return newTematica;
        }

        public async Task<IEnumerable<TematicaModel>> GetTematicas()
        {
            return _context.Tematica.ToList();
        }

        public async Task<TematicaModel> GetTematicaById(int TematicaId)
        {
            return _context.Tematica.FirstOrDefault(x => x.Id == TematicaId);
        }

        public async Task<IEnumerable<CityModel>> GetCities() 
        {
            return _context.City.ToList();  
        }
    }

}
