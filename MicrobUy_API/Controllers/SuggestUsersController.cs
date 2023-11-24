﻿using MicrobUy_API.Data.Repositories;
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

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="createUserNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            await _neo4jUsersRepository.CreateUser(createUserNeo4JDto);
            return Ok();
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="crearPostNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("CreatePost")]
        public Task<int> CreatePost([FromBody] CrearPostNeo4jDto crearPostNeo4JDto)
        {
            return _neo4jUsersRepository.CreatePost(crearPostNeo4JDto);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="createUserNeo4JDto"></param>
        /// <returns></returns>
        [HttpPut("UpdateUser")]
        public Task<int> UpdateUser([FromBody] CreateUserNeo4jDto createUserNeo4JDto)
        {
            return _neo4jUsersRepository.UpdateUser(createUserNeo4JDto);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="giveLikeNeo4JDto"></param>
        /// <returns></returns>
        [HttpPost("GiveLike")]
        public Task<int> GiveLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.GiveLike(giveLikeNeo4JDto);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="giveLikeNeo4JDto"></param>
        /// <returns></returns>
        [HttpDelete("DeleteLike")]
        public Task<int> DeleteLike([FromBody] GiveLikeNeo4jDto giveLikeNeo4JDto)
        {
            return _neo4jUsersRepository.DeleteLike(giveLikeNeo4JDto);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("TopHashtagByTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagByTenant([Required]int tenantId, int topCant)
        {
            return _neo4jUsersRepository.TopHashtagByTenant(tenantId, topCant);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("TopHashtagAllTenant")]
        public Task<List<HashtagNeo4jDto>> TopHashtagAllTenant(int topCant)
        {
            return _neo4jUsersRepository.TopHashtagAllTenant(topCant);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("PostWhitMostLikeAllTenant")]
        public Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeAllTenant(int topCant)
        {
            return _neo4jUsersRepository.PostWhitMostLikeAllTenant(topCant);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("PostWhitMostLikeByTenant")]
        public Task<List<PostWhitMostLikeNeo4jDto>> PostWhitMostLikeByTenant([Required] int tenantId, int topCant)
        {
            return _neo4jUsersRepository.PostWhitMostLikeByTenant(tenantId, topCant);
        }

        /// <summary>
        /// Necesita un comentario
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="topCant"></param>
        /// <returns></returns>
        [HttpGet("SuggestUsersByTenant")]
        public Task<List<SuggestUserNeo4jDto>> SuggestUsersByTenant([Required] int tenantId, [Required] int userId, int topCant)
        {
            return _neo4jUsersRepository.SuggestUsersByTenant(tenantId, userId, topCant);
        }
    }
}
