using System.Collections.Generic;

namespace BlogWebApi.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string ImageUrl { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}