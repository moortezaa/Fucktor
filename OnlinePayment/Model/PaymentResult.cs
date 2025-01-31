namespace OnlinePayment.Model
{
    public class PaymentResult
    {
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
        public string TrackingNumber { get; set; }
    }
}