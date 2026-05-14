using System.ComponentModel.DataAnnotations;

namespace Web.Domain.Entities
{
    public class Roles
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public required string DisplayName { get; set; }
    }
}
