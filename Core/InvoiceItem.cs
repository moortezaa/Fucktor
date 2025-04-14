namespace Core
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public float Off { get; set; }

        public Guid ItemId { get; set; }
        public Item Item { get; set; }

        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}