namespace Core
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MeasuringUnit { get; set; }

        public List<UserItem> Sellers { get; set; } = [];
    }
}