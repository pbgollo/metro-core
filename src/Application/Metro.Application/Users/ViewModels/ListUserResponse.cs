namespace Metro.Application.Users.ViewModels
{
    public class ListUserResponse
    {
        public IEnumerable<ListUserViewModel?> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
