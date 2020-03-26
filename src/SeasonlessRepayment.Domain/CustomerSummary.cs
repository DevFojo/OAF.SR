namespace SeasonlessRepayment.Domain
{
    public class CustomerSummary : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }
        public decimal TotalRepaid { get; set; }
        public decimal TotalCredit { get; set; }
    }
}