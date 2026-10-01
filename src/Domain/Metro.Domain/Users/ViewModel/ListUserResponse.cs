namespace Metro.Domain.Users.ViewModel
{
    public class ListUserResponse
    {
        public IEnumerable<ListUserViewModel?> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
