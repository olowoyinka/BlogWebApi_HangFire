using System;

namespace BlogWebApi.Contracts.DTOs.Responses
{
    public class LoginResponseDTO
    {
        public string token { get; set; }
        public string expiryTime { get; set; }
    }
}