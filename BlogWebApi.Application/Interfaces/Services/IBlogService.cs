using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using System.Collections.Generic;

namespace BlogWebApi.Application.Interfaces.Services
{
    public interface IBlogService
    {
        (int totalCount, List<BlogResponseDTO> getAllBlog) GetAllBlog(int page, int size, string title, string email);

        BlogResponseDTO CreateBlog(string userId, BlogRequestDTO model);

        BlogResponseDTO GetBlog(int id);

        BlogResponseDTO UpdateBlog(string userId, int id, BlogUpdateDTO model);

        string DeleteBlog(string userId, int id);

        string CreateComment(string userId, int blogId, CommentRequestDTO model);

        string UpdateComment(string userId, int blogId, int commentId, CommentUpdateDTO model);

        string DeleteComment(string userId, int blogId, int commentId);
    }
}