using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Contracts.DTOs.Requests
{
    public class BlogRequestDTO
    {
        [Required]
        public string title { get; set; }

        [Required]
        public string body { get; set; }

        public string imageUrl { get; set; }

        [Required]
        public string tagName { get; set; }
    }
}