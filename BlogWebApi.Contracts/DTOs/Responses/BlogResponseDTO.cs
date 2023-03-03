using System;
using System.Collections.Generic;

namespace BlogWebApi.Contracts.DTOs.Responses
{
    public class BlogResponseDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set;  }
        public string imageUrl { get; set; }
        public string publishedBy { get; set; } 
        public DateTime createAt { get; set; }
        public DateTime? updateAt { get; set; }

        public TagResponseDTO tag { get; set; }

        public IEnumerable<CommentResponseDTO> comments { get; set; }
    }
}