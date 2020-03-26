using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public static class Extensions
    {
        public static Repayment ToRepayment(this RepaymentUpload upload)
        {
            return new Repayment
            {
                Amount = upload.Amount,
                CustomerId = upload.CustomerId,
                SeasonId = upload.SeasonId,
                Date = upload.Date
            };
        }

        public static Repayment ToRepaymentWithSeasonId(this RepaymentUpload upload, int seasonId)
        {
            return new Repayment
            {
                Amount = upload.Amount,
                CustomerId = upload.CustomerId,
                SeasonId = seasonId,
                Date = upload.Date
            };
        }
    }
}