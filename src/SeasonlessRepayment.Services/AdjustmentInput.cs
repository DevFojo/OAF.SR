using System;

namespace SeasonlessRepayment.Services
{
    public class AdjustmentInput
    {
        public AdjustmentInput(decimal amount, decimal? outstandingDebt, int customerId, DateTime date,
            int seasonId, int? parentId)
        {
            OutstandingDebt = outstandingDebt ?? 0;
            CustomerId = customerId;
            Date = date;
            SeasonId = seasonId;
            ParentId = parentId;
            Amount = amount;
        }

        public decimal OutstandingDebt { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public int SeasonId { get; set; }
        public int? ParentId { get; set; }
    }
}