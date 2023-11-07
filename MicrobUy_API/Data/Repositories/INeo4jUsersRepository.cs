using MicrobUy_API.Dtos;
using MicrobUy_API.Models;

namespace MicrobUy_API.Data.Repositories
{
    public interface INeo4jUsersRepository
    {
        Task CreateUser(int userId, int tenantId, string username, string occupation, string city);
        Task<int> CreatePost(int userId, int tenantId, int postId, string postCreated/*, List<String> hashtag*/);
        Task<int> UpdateUser(int userId, int tenantId, string username, string occupation, string city);
    }
}
