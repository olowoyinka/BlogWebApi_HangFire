using BlogWebApi.Contracts.Commons;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Contracts.DTOs.Requests
{
    public class RegisterRequestDTO
    {
        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [StringRange(AllowableValues = new[] { "creator", "reader" }, ErrorMessage = "role must be either 'creator' or 'reader'.")]
        public string role { get; set; }
    }
}