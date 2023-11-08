using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Services.GeneralDataService
{
    public interface IGeneralDataService
    {
        Task<TematicaModel> CreateTematica(CreateTematicaRequestDto tematica);

        Task<IEnumerable<TematicaModel>> GetTematicas();

        Task<TematicaModel> GetTematicaById(int TematicaId);

        Task<IEnumerable<CityModel>> GetCities();

    }
}
