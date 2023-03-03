using System;

namespace BlogWebApi.Contracts.DTOs.Responses
{
    public class CommentResponseDTO
    {
        public int id { get; set; }

        public string email { get; set; }

        public string message { get; set; }

        public DateTime createAt { get; set; }

        public DateTime? updateAt { get; set; }
    }
}