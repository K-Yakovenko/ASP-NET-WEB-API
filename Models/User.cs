using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITask.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Range(0, 150)]
        public required int Age { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        public List<Role> Roles { get; } = new();
    }
}