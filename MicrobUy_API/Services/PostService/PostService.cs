using AutoMapper;
using Azure.Core;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.Models;
using System.Threading.Tasks;

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
            newPost.Created =  DateTime.Now;

            await _context.AddAsync(newPost);
            _context.SaveChanges();

            return newPost;
        }

        public Task<PostModel> CreatePostComment(PostModel post, CreatePostDto postComment, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> CreatePostComment(int postId, CreatePostDto postComment, string userName)
        {
            throw new NotImplementedException();
        }

        public async Task <IEnumerable<PostModel>> GetPostByUser(string userName)
        {
            var post = _context.Post.Where(x => x.UserOwner.UserName == userName).ToList();
            return post;
        }

        
    }
}
