using AutoMapper;
using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.PostService;
using MicrobUy_API.Services.TenantInstanceService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto post, string userEmail)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
           //FluentValidation.Results.ValidationResult res = await _validator.ValidateAsync(post);
            
            //if(res.IsValid) {
                var result = await _postService.CreatePost(post, userEmail);
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
        public async Task<IActionResult> CommentAsync(int postId, [FromBody] CreatePostDto postComment, string userEmail)
        {
            IEnumerable<string> errors;
            List<string> listOfErrors = new List<string>();
            FluentValidation.Results.ValidationResult res = await _validator.ValidateAsync(postComment);

            if (res.IsValid)
            {
                var result = await _postService.CreatePostComment(postId, postComment, userEmail);
                if (result == null)
                {
                    listOfErrors.Add("Error al crear el comentario");
                    errors = listOfErrors.Select(x => x);
                    return BadRequest(new UserPostResponseDto { Errors = errors });
                }
                return Ok(result);
            }
            return BadRequest(res.Errors);
        }


    }
}
