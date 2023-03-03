namespace BlogWebApi.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Message { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}