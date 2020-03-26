using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public class AdjustmentOutput
    {
        public decimal RemainingAmount { get; set; }
        public Repayment Repayment { get; set; }
    }
}