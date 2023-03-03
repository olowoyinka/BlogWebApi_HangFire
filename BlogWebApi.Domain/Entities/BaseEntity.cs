using System;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateAt { get; set; }
        
        public DateTime? DeleteAt { get; set; }
    }
}