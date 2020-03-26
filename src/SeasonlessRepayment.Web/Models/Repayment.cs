using System;

namespace SeasonlessRepayment.Web.Models
{
    public class Repayment
    {
        public int RepaymentId { get; }
        public int CustomerId { get; }
        public int? SeasonId { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public int? ParentId { get; }
    }
}