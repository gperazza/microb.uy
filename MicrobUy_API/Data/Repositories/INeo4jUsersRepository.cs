using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Data.Repositories
{
    public interface INeo4jUsersRepository
    {
        Task CreateUser(CreateUserNeo4jDto createUserNeo4JDto);
        Task<int> CreatePost(CrearPostNeo4jDto crearPostNeo4JDto);
        Task<int> UpdateUser(CreateUserNeo4jDto createUserNeo4JDto);
        Task<int> GiveLike(GiveLikeNeo4jDto giveLikeNeo4JDto);
        Task<List<HashtagNeo4jDto>> TopHashtagByTenant(int tenantId, int topCant);
        Task<List<HashtagNeo4jDto>> TopHashtagAllTenant(int topCant);
        Task<int> DeleteLike(GiveLikeNeo4jDto giveLikeNeo4JDto);
    }
}
