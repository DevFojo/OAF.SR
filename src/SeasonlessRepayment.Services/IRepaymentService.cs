using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public interface IRepaymentService
    {
        Task<List<Repayment>> ProcessUploads(IEnumerable<RepaymentUpload> repaymentUploads);
        Task<List<Repayment>> ProcessUpload(RepaymentUpload repaymentUpload);
        Task<List<Repayment>> List();
    }
}