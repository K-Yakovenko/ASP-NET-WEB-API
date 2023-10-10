using APITask.Models;

namespace APITask.Views
{
    public class UsersView
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public List<Role> Roles { get; set; }
    }
}