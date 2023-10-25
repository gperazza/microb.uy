using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicrobUy_API.Services.PostService
{
    public interface IPostService
    {
       Task<PostModel> CreatePost(CreatePostDto post, string userEmail);
       Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userEmail);
    }
}
