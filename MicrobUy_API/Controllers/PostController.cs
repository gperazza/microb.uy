using AutoMapper;
using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.PostService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private IValidator<CreatePostDto> _validator;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto post, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
           //FluentValidation.Results.ValidationResult res = await _validator.ValidateAsync(post);
            
            //if(res.IsValid) {
                var result = await _postService.CreatePost(post, userName);
                if (result == null)
                {
                    listOfErrors.Add("Error al crear el post");
                    errors = listOfErrors.Select(x => x);
                    return BadRequest(new UserPostResponseDto { Errors = errors });
                }
                return Ok(result);
            //}
            //return BadRequest(res.Errors);
        }

        [HttpPost("CreateComment")]
        public async Task<IActionResult> CommentAsync(int postId, [FromBody] CreatePostDto postComment, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            var result = await _postService.CreatePostComment(postId, postComment, userName);
            if (result == null)
            {
                listOfErrors.Add("Error al crear el comentario");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserPostResponseDto { Errors = errors });
            }
            return Ok(result);
        
        }

        [HttpGet("GetPostByUser")]
        public async Task<IActionResult> GetPostByUser(String userName)
        {
            var result = await _postService.GetPostByUser(userName);
            return Ok(result);
        }

        [HttpPost("AddLikeToPost")]
        public async Task<IActionResult> LikeAsync(int postId, string userName)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();

            var result = await _postService.LikeComment(postId, userName);
            if (result == null)
            {
                listOfErrors.Add("Error al likear el posteo");
                errors = listOfErrors.Select(x => x);
                return BadRequest(new UserPostResponseDto { Errors = errors });
            }
            return Ok(result);

        }

        [HttpGet("GetPostById")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var result = await _postService.GetPostById(postId);
            return Ok(result);
        }
    }
}
