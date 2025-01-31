namespace OnlinePayment.Model
{
    public class Invoice
    {
        public string MerchantID { get; set; }
        public long Amount { get; set; }
        public string Payload { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
}