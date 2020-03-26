using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Data.Repository
{
    public interface ICustomerSummaryRepository : IBaseRepository<CustomerSummary>
    {
        Task<List<CustomerSummary>> GetByCustomerId(int customerId);
        Task<CustomerSummary> GetByCustomerIdAndSeasonId(int customerId, int seasonId);
        Task<List<CustomerSummary>> GetOutstandingByCustomerId(int customerId);
        Task<CustomerSummary> GetLastByCustomerId(int customerId);
    }
}