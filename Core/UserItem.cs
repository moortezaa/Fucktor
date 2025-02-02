namespace Core
{
    public class UserItem
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public decimal DefaultUnitPrice { get; set; }
        public int StorageAmount { get; set; }
    }
}