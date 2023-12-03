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
            newPost.isSanctioned = false;
            newPost.PendingToReview = false;
            newPost.alreadyModerated = false;
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
            newPost.isSanctioned = false;
            newPost.PendingToReview = false;
            newPost.Active = true;
            newPost.alreadyModerated = false;

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
            var aux_post = _context.Post.Where(x => x.PostId == postId && (x.Active == true) && (x.isSanctioned == false)).Include(x => x.Comments).Include(x => x.UserOwner)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X => X.Likes).FirstOrDefault();

            var postDto = _mapper.Map<PostModel, PostDto>(aux_post);
            return postDto;
        }

        public async Task<IEnumerable<PostDto>> GetPostByUser(string userName)
        {
            var aux_post = _context.Post.Include(x => x.UserOwner).Include(x => x.Comments)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X => X.Likes).Where(x => x.UserOwner.UserName == userName && !(x is CommentModel) && (x.Active) && (!x.isSanctioned)).ToList();

            var postDto = _mapper.Map<List<PostModel>, List<PostDto>>(aux_post);
            return postDto;
        }

        public async Task<PostModel> LikeComment(int postId, string userName)
        {
            PostModel aux_post = _context.Post.Include(x => x.Likes).FirstOrDefault(x => x.PostId == postId);
            UserModel aux_user = _context.User.Where(x => x.UserName == userName).FirstOrDefault();

            if (aux_post.Active == false) return null;
            if (aux_user == null) return null;
            if (aux_post == null) return null;

            if (!aux_post.Likes.Select(x => x.UserId).ToList().Contains(aux_user.UserId))
            {
                aux_post.Likes.Add(aux_user);
                _context.SaveChanges();

                return aux_post;
            }

            return aux_post;
        }

        public async Task<IEnumerable<PostDto>> GetAllPost()
        {
            var aux_post = _context.Post.Where(x => x.Active == true && !(x is CommentModel) && (x.isSanctioned == false)).Include(x => x.Comments).Include(x => x.UserOwner)
               .Include(x => x.Likes).Include(x => x.Hashtag).Include(X => X.Likes).ToList();

            var postDto = _mapper.Map<List<PostModel>, List<PostDto>>(aux_post);
            return postDto;
        }
        // ------------------------- FUNCIONALIDADES PARA REPORTAR Y MODERAR REPORTES ------------------------- //
        public async Task<bool> ReportPostById(int postId, string userName)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            UserModel aux_user = _context.User.Where(x => x.UserName == userName).FirstOrDefault();
            if (aux_post == null || aux_user == null) return false;
            aux_post.Reporters.Add(aux_user);
            if (aux_post.alreadyModerated == false) aux_post.PendingToReview = true;
            _context.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<PostDto>> GetReportedPosts()
        {
            var posts = _context.Post.Where(x => x.PendingToReview == true && (x.Active == true) && (x.isSanctioned == false) && (x.alreadyModerated == false)).ToList();
            var postDto = _mapper.Map<List<PostModel>, List<PostDto>>(posts);
            return postDto;
        }

        public async Task<bool> PunishPost(int postId)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            if (aux_post == null) return false;
            aux_post.isSanctioned = true;
            aux_post.alreadyModerated = true;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DismissReportedPost(int postId)
        {
            PostModel aux_post = _context.Post.FirstOrDefault(x => x.PostId == postId);
            if (aux_post == null) return false;
            aux_post.isSanctioned = false;
            aux_post.PendingToReview = false;
            aux_post.alreadyModerated = true;
            _context.SaveChanges();
            return true;
        }
        


        // --------------------------------- FUNCIONALIDADES PARA ESTADISTICAS --------------------------------- //
        public async Task<CountPostDto> GetCountPost()
        {
            var totalposts = _context.Post.Where(x => !(x is CommentModel)).ToList();
            int cantPost = totalposts.Count;

            var totalSancionPosts = _context.Post.Where(x => x.PendingToReview == true && (x.Active == true) && (x.isSanctioned == true)).ToList();
            int cantPostSancionados = totalSancionPosts.Count;

            CountPostDto aux_cant = new CountPostDto();
            aux_cant.total = cantPost;
            aux_cant.sancionados = cantPostSancionados;
            return aux_cant;
        }

       
    }

}


