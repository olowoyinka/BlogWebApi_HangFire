using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Contracts.DTOs.Requests
{
    public class BlogUpdateDTO
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