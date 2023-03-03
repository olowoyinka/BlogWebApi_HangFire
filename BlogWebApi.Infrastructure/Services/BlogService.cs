using BlogWebApi.Application.Interfaces.Persistence;
using BlogWebApi.Application.Interfaces.Services;
using BlogWebApi.Contracts.Commons;
using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using BlogWebApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BlogWebApi.Infrastructure.Services
{
    public class BlogService : IBlogService
    {
        private readonly IGenericRepository<Blog> BlogRepository;
        private readonly IGenericRepository<Tag> TagRepository;
        private readonly IGenericRepository<Comment> CommentRepository;

        public BlogService(IGenericRepository<Blog> blogRepository,
                            IGenericRepository<Tag> tagRepository,
                            IGenericRepository<Comment> commentRepository)
        {
            this.BlogRepository = blogRepository;
            this.TagRepository = tagRepository;
            this.CommentRepository = commentRepository;
        }


        public (int totalCount, List<BlogResponseDTO> getAllBlog) GetAllBlog (int page, int size, string title, string email)
        {
            if (page < 1)
                page = 1;

            if (size < 0)
                size = 10;

            int skip = size * (page - 1);
            
            var getAllBlog = BlogRepository.Get(s => s.Title.Contains(title ?? "") &&
                                                    s.User.Email.Contains(email ?? ""),
                                                includeProperties: "User," +
                                                                   "Tag," +
                                                                   "Comments.User")
                                                .Skip(skip)
                                                .Take(size)
                                                .OrderByDescending(s => s.CreateAt)
                                    .Select(s => new BlogResponseDTO
                                    {
                                        id = s.Id,
                                        title = s.Title,
                                        body = s.Body,
                                        imageUrl = s.ImageUrl,
                                        createAt = s.CreateAt,
                                        updateAt = s.UpdateAt,
                                        publishedBy = $"{s.User.FirstName} {s.User.LastName}",
                                        tag = new TagResponseDTO
                                        {
                                            name = s.Tag.Name
                                        },
                                        comments = s.Comments.Select(d => new CommentResponseDTO
                                        {
                                            id = d.Id,
                                            email = d.User.Email,
                                            message = d.Message,
                                            createAt = d.CreateAt,
                                            updateAt = d.UpdateAt
                                        }).ToList()
                                    })
                                    .ToList();

            var blogCount = BlogRepository.Get(s => s.Title.Contains(title ?? "") &&
                                                    s.User.Email.Contains(email ?? ""))
                            .Count();

            return (blogCount, getAllBlog);
        }


        public BlogResponseDTO CreateBlog (string userId, BlogRequestDTO model)
        {
            BlogExist(userId, model.title);

            var tagId = AddTag(model.tagName);

            var newBlog = new Blog
            {
                TagId = tagId,
                Title = model.title,
                Body = model.body,
                CreateAt = DateTime.UtcNow,
                ImageUrl = model.imageUrl,
                UserId = userId
            };

            var blogId = BlogRepository.Insert(newBlog);

            return GetBlog(blogId);
        }


        public BlogResponseDTO GetBlog(int id)
        {
            var blogExist = BlogRepository.Get(s => s.Id.Equals(id),
                                                includeProperties: "User," +
                                                                   "Tag," +
                                                                   "Comments.User")
                                    .Select(s => new BlogResponseDTO
                                    {
                                        id = s.Id,
                                        title = s.Title,
                                        body = s.Body,
                                        imageUrl = s.ImageUrl,
                                        createAt = s.CreateAt,
                                        updateAt = s.UpdateAt,
                                        publishedBy = $"{s.User.FirstName} {s.User.LastName}",
                                        tag = new TagResponseDTO
                                        {
                                            name = s.Tag.Name
                                        },
                                        comments = s.Comments.Select(d => new CommentResponseDTO
                                        {
                                            id = d.Id,
                                            email = d.User.Email,
                                            message = d.Message,
                                            createAt = d.CreateAt,
                                            updateAt = d.UpdateAt
                                        }).ToList()
                                    })
                                    .FirstOrDefault();

            if (blogExist == null)
                throw new NotFoundException($"Blog id {id} not found", HttpStatusCode.NotFound);

            return blogExist;
        }
        
        
        public BlogResponseDTO UpdateBlog(string userId, int id, BlogUpdateDTO model)
        {
            var blogUserExist = BlogUserExist(userId, id);

            BlogTitleExist(userId, model.title, blogUserExist);

            var tagId = AddTag(model.tagName);

            blogUserExist.TagId = tagId;
            blogUserExist.Title = model.title;
            blogUserExist.Body = model.body;
            blogUserExist.UpdateAt = DateTime.UtcNow;
            blogUserExist.ImageUrl = model.imageUrl;

            BlogRepository.Update(blogUserExist);

            return GetBlog(id);
        }

        
        public string DeleteBlog(string userId, int id)
        {
            var blogUserExist = BlogUserExist(userId, id);

            BlogRepository.Delete(blogUserExist);

            return "deleted successfully";
        }

        
        public string CreateComment(string  userId, int blogId, CommentRequestDTO model)
        {
            var getBlog = BlogExistById(blogId);

            var newComment = new Comment()
            {
                BlogId = blogId,
                CreateAt = DateTime.UtcNow,
                Message = model.message,
                UserId = userId
            };

            this.CommentRepository.Insert(newComment);

            return "commented successfully";
        }


        public string UpdateComment(string userId, int blogId, int commentId, CommentUpdateDTO model)
        {
            var getComment = CommentExist(userId, blogId, commentId);

            getComment.Message = model.message;
            getComment.UpdateAt = DateTime.UtcNow;

            this.CommentRepository.Update(getComment);

            return "comment updated successfully";
        }


        public string DeleteComment(string userId, int blogId, int commentId)
        {
            var getComment = CommentExist(userId, blogId, commentId);

            this.CommentRepository.Delete(getComment);

            return "comment deleted successfully";
        }


        private Blog BlogExist (string userId, string title)
        {
            var blogExist = BlogRepository.Get(s => s.UserId.Equals(userId) &&
                                                    s.Title.ToLower().Equals(title.ToLower()))
                                    .FirstOrDefault();

            if (blogExist != null)
                throw new BadRequestException("Blog Title exist", HttpStatusCode.BadRequest);

            return blogExist;
        }
        
        
        private void BlogTitleExist(string userId, string title, Blog blog)
        {
            var blogExist = BlogRepository.Get(s => s.UserId.Equals(userId) &&
                                                    s.Title.ToLower().Equals(title.ToLower()))
                                    .FirstOrDefault();

            if (blogExist != null)
            {
                if (!blogExist.Title.Equals(blog.Title))
                {
                    throw new BadRequestException("Blog Title exist", HttpStatusCode.BadRequest);
                }
            }
        }


        private Blog BlogUserExist(string userId, int id)
        {
            var blogExist = BlogRepository.Get(s => s.UserId.Equals(userId) &&
                                                    s.Id.Equals(id))
                                    .FirstOrDefault();

            if (blogExist == null)
                throw new NotFoundException($"Blog id {id} not found", HttpStatusCode.BadRequest);

            return blogExist;
        }


        private Blog BlogExistById(int id)
        {
            var blogExist = BlogRepository.Get(s => s.Id.Equals(id))
                                    .FirstOrDefault();

            if (blogExist == null)
                throw new NotFoundException($"Blog id {id} not found", HttpStatusCode.BadRequest);

            return blogExist;
        }


        private int AddTag(string tagName)
        {
            var tagExist = TagRepository.Get(s => s.Name.ToLower().Equals(tagName.ToLower())).FirstOrDefault();

            if (tagExist != null)
                return tagExist.Id;

            var newTag = new Tag()
            {
                Name = tagName,
                CreateAt = DateTime.UtcNow
            };

            return TagRepository.Insert(newTag);
        }

        
        private Comment CommentExist(string userId, int blogId, int commentId)
        {
            var commentExist = CommentRepository.Get(s => s.UserId.Equals(userId) &&
                                                    s.Id.Equals(commentId) &&
                                                    s.BlogId.Equals(blogId))
                                    .FirstOrDefault();

            if (commentExist == null)
                throw new NotFoundException($"comment not found", HttpStatusCode.BadRequest);

            return commentExist;
        }
    }
}