using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Contracts.DTOs.Requests
{
    public class CommentUpdateDTO
    {
        [Required]
        public string message { get; set; }
    }
}