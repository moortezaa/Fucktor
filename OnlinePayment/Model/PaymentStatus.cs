namespace OnlinePayment.Model
{
    public enum PaymentStatus
    {
        Pending = 0,
        PendingVerify = 1,
        Payed = 2,
        Cancelled = 3,
        Failed = 4,
    }
}