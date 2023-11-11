using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Data.Repositories
{
    public interface INeo4jUsersRepository
    {
        Task CreateUser(CreateUserNeo4jDto createUserNeo4JDto);
        Task<int> CreatePost(CrearPostNeo4jDto crearPostNeo4JDto);
        Task<int> UpdateUser(CreateUserNeo4jDto createUserNeo4JDto);
        Task<int> GiveLike(int userId, int tenantId, int postId);
        Task<int> TopHashtagByTenant(int tenantId, int topCant);
        Task<int> TopHashtagAllTenant(int topCant);
    }
}
