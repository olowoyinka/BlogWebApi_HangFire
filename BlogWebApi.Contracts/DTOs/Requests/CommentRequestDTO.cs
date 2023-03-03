using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Contracts.DTOs.Requests
{
    public class CommentRequestDTO
    {
        [Required]
        public string message { get; set; }
    }
}