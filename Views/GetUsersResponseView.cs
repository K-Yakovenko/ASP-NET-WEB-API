using APITask.Models;

namespace APITask.Views
{
    public class GetUsersResponseView
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<UsersView> Users { get; set; }
    }
}