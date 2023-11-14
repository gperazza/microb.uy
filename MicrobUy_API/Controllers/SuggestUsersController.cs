using MicrobUy_API.Data.Repositories;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;

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
        
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            await _neo4jUsersRepository.CreateUser(createUserNeo4JDto);
            return Ok();
        }

        [HttpPost("CreatePost")]
        public Task<int> CreatePost([FromBody] CrearPostNeo4jDto crearPostNeo4JDto)
        {
            return _neo4jUsersRepository.CreatePost(crearPostNeo4JDto);
        }

        [HttpPut("UpdateUser")]
        public Task<int> UpdateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            return _neo4jUsersRepository.UpdateUser(createUserNeo4JDto);
        }

        [HttpPost("GiveLike")]
        public Task<int> GiveLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.GiveLike(giveLikeNeo4JDto);
        }

        [HttpDelete("DeleteLike")]
        public Task<int> DeleteLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.DeleteLike(giveLikeNeo4JDto);
        }

        [HttpGet("TopHashtagByTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagByTenant([Required]int tenantId, int topCant)
        {
            return _neo4jUsersRepository.TopHashtagByTenant(tenantId, topCant);
        }

        [HttpGet("TopHashtagAllTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagAllTenant(int topCant)
        {
            return _neo4jUsersRepository.TopHashtagAllTenant(topCant);
        }

        [HttpGet("PostWhitMostLikeAllTenant")]
        public Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeAllTenant(int topCant)
        {
            return _neo4jUsersRepository.PostWhitMostLikeAllTenant(topCant);
        }
    }
}
