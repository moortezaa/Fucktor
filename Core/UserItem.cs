namespace Core
{
    public class UserItem
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public decimal DefaultUnitPrice { get; set; }
        public int StorageAmount { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        public Guid UserId { get; set; }
        public AppUser? User { get; set; }
    }
}