namespace Web.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string DisplayName { get; set; } = string.Empty;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
