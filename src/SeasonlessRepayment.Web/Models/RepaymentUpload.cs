namespace SeasonlessRepayment.Web.Models
{
    public class RepaymentUpload
    {
        public int CustomerId { get; set; }
        public int? SeasonId { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
    }
}