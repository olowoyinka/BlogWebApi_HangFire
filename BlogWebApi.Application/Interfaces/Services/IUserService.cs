using BlogWebApi.Contracts.Commons;
using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using System.Threading.Tasks;

namespace BlogWebApi.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<DataResponseDTO<LoginResponseDTO>> Register(RegisterRequestDTO model);
        Task<DataResponseDTO<LoginResponseDTO>> Login(LoginRequestDTO model);
    }
}