using AutoMapper;
using MicrobUy_API.Data;
using MicrobUy_API.Models;

using Microsoft.EntityFrameworkCore;
using Azure.Core;


using MicrobUy_API.Services.AccountService;

using Microsoft.EntityFrameworkCore;
using Azure.Core;
using MicrobUy_API.Dtos.PostDto;
using MicrobUy_API.Dtos;


namespace MicrobUy_API.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly TenantAplicationDbContext _context;

        public PostService(IMapper mapper, TenantAplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PostModel> CreatePost(CreatePostDto post, string userName)
        {
            PostModel newPost = _mapper.Map<PostModel>(post);
            UserModel userExist = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (userExist == null) return null;

            newPost.UserOwner = userExist;
            newPost.Created = DateTime.Now;
            newPost.Active = true;
            await _context.AddAsync(newPost);
            _context.SaveChanges();

            return newPost;
        }

        public async Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userName)
        {
            CommentModel newPost = _mapper.Map<CommentModel>(postComment);
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            UserModel userExist = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (aux_post.Active == false) return null;
            if (userExist == null) return null;
            if (aux_post == null) return null;

            newPost.UserOwner = userExist;
            newPost.Created = DateTime.Now;

            aux_post.Comments.Add(newPost);
            _context.SaveChanges();

            return newPost;
        }

        public async Task<bool> DeletePostById(int postId)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            if (aux_post == null) return false;
            aux_post.Active = false;
            _context.SaveChanges();
            return true;
        }

        public async Task<PostDto> GetPostById(int postId)
        {
            var aux_post = _context.Post.Where(x => x.PostId == postId && (x.Active)).Include(x => x.Comments).Include(x => x.UserOwner)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X => X.Likes).FirstOrDefault();

            var postDto = _mapper.Map<PostModel, PostDto>(aux_post);
            return postDto;
        }

        public async Task<IEnumerable<PostDto>> GetPostByUser(string userName)
        {
            var aux_post = _context.Post.Where(x => x.UserOwner.UserName == userName && !(x is CommentModel) && (x.Active)).Include(x => x.Comments).Include(x => x.UserOwner)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X=> X.Likes).ToList();

            var postDto = _mapper.Map<List<PostModel>, List<PostDto>>(aux_post);
            return postDto;
        }

        public async Task<PostModel> LikeComment(int postId, string userName)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            UserModel aux_user = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (aux_post.Active == false) return null;
            if (aux_user == null) return null;
            if (aux_post == null) return null;

            aux_post.Likes.Add(aux_user);
            _context.SaveChanges();

            return aux_post;

        }
    }

}
