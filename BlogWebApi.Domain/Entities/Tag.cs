using System.ComponentModel.DataAnnotations;

namespace BlogWebApi.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
    }
}