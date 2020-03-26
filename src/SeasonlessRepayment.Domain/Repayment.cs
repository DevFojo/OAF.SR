using System;

namespace SeasonlessRepayment.Domain
{
    public class Repayment : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? SeasonId { get; set; }
        public Season Season { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public int? ParentId { get; set; }
        public Repayment Parent { get; set; }
    }
}