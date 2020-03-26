using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Data.Repository
{
    public class CustomerSummaryRepository : BaseEntityRepository<CustomerSummary>, ICustomerSummaryRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomerSummaryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<CustomerSummary>> GetByCustomerId(int customerId)
        {
            return Find(cs => cs.CustomerId == customerId,
                cs => cs.Season.StartDate,
                cs => cs.Customer, cs => cs.Season);
        }

        public Task<CustomerSummary> GetByCustomerIdAndSeasonId(int customerId, int seasonId)
        {
            return GetSingle(cs => cs.CustomerId == customerId && cs.SeasonId == seasonId,
                cs => cs.Season, cs => cs.Customer);
        }

        public Task<List<CustomerSummary>> GetOutstandingByCustomerId(int customerId)
        {
            return Find(cs => cs.CustomerId == customerId && cs.TotalCredit > cs.TotalRepaid,
                cs => cs.Season.StartDate,
                cs => cs.Customer, cs => cs.Season);
        }

        public Task<CustomerSummary> GetLastByCustomerId(int customerId)
        {
            return _dbContext.CustomerSummaries
                .OrderByDescending(cs => cs.Season.StartDate)
                .FirstOrDefaultAsync(cs => cs.CustomerId == customerId);
        }
    }
}