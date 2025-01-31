namespace OnlinePayment.Model
{
    public class SepVerifyInfo
    {
        public string RRN { get; set; }
        public string RefNum { get; set; }
        public string MaskedPan { get; set; }
        public string HashedPan { get; set; }
        public int TerminalNumber { get; set; }
        public long OriginalAmount { get; set; }
        public long AffectiveAmount { get; set; }
        public string StraceDate { get; set; }
        public string StraceNo { get; set; }
    }
}