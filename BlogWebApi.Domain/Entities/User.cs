using Microsoft.AspNetCore.Identity;
using System;

namespace BlogWebApi.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateAt { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}