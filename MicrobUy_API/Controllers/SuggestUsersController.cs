using MicrobUy_API.Data.Repositories;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuggestUsersController : ControllerBase
    {
        private readonly INeo4jUsersRepository _neo4jUsersRepository;

        public SuggestUsersController(INeo4jUsersRepository neo4jUsersRepository)
        {
            _neo4jUsersRepository = neo4jUsersRepository;
        }

        [HttpGet("CreateUser")]
        public Task<int> CreateUser(int UserId, int tenantId, string username, string occupation, string city)
        {
            return _neo4jUsersRepository.CreateUser(UserId, tenantId, username, occupation, city);
        }

        [HttpGet("CreatePost")]
        public Task<int> CreatePost(int UserId, int tenantId, int postId, string postCreated)
        {
            return _neo4jUsersRepository.CreatePost(UserId, tenantId, postId, postCreated);
        }

        [HttpGet("UpdateUser")]
        public Task<int> UpdateUser(int UserId, int tenantId, string username, string occupation, string city)
        {
            return _neo4jUsersRepository.UpdateUser(UserId, tenantId, username, occupation, city);
        }
    }
}
