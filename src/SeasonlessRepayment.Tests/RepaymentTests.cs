using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeasonlessRepayment.Data;
using SeasonlessRepayment.Data.Repository;
using SeasonlessRepayment.Domain;
using SeasonlessRepayment.Services;
using Xunit;

namespace SeasonlessRepayment.Tests
{
    public class RepaymentTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IRepaymentService _repaymentService;

        public RepaymentTests()
        {
            var mockSeasons = new List<Season>
            {
                new Season {Id = 1, Name = "2017 Rainy", StartDate = new DateTime(2017, 01, 01)},
                new Season {Id = 2, Name = "2018 Rainy", StartDate = new DateTime(2018, 01, 01)}
            };

            var mockCustomers = new List<Customer>
            {
                new Customer {Id = 1, Name = "Kit Kat"},
                new Customer {Id = 2, Name = "Ping Pong"},
                new Customer {Id = 3, Name = "Yin Yang"}
            };

            var mockCustomerSummaries = new List<CustomerSummary>
            {
                new CustomerSummary {CustomerId = 1, SeasonId = 1, TotalRepaid = 200, TotalCredit = 1000},
                new CustomerSummary {CustomerId = 1, SeasonId = 2, TotalRepaid = 700, TotalCredit = 1000},
                new CustomerSummary {CustomerId = 2, SeasonId = 1, TotalRepaid = 1200, TotalCredit = 1000},
                new CustomerSummary {CustomerId = 3, SeasonId = 1, TotalRepaid = 1000, TotalCredit = 1200},
                new CustomerSummary {CustomerId = 3, SeasonId = 2, TotalRepaid = 500, TotalCredit = 500},
                new CustomerSummary {CustomerId = 4, SeasonId = 1, TotalRepaid = 1000, TotalCredit = 1000},
                new CustomerSummary {CustomerId = 4, SeasonId = 2, TotalRepaid = 300, TotalCredit = 100}
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("SeasonServiceTestDb")
                .Options;

            _dbContext = new AppDbContext(options);

            _repaymentService = new RepaymentService(_dbContext,
                new BaseEntityRepository<Repayment>(_dbContext),
                new CustomerSummaryRepository(_dbContext),
                new BaseEntityRepository<RepaymentUpload>(_dbContext));

            _dbContext.Customers.AddRange(mockCustomers);
            _dbContext.Seasons.AddRange(mockSeasons);
            _dbContext.CustomerSummaries.AddRange(mockCustomerSummaries);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task UploadWithSeasonIdAmountLessThanDebt()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 1,
                SeasonId = 2,
                Amount = 100,
                Date = DateTime.Now
            };


            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Single(repayments);

            var repayment = repayments.First();
            Assert.Equal(mockRepaymentUpload.Amount, repayment.Amount);
            Assert.Equal(mockRepaymentUpload.CustomerId, repayment.CustomerId);
            Assert.Equal(mockRepaymentUpload.SeasonId, repayment.SeasonId);

            var summaryAfterUpload = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs =>
                    cs.CustomerId == mockRepaymentUpload.CustomerId && cs.SeasonId == mockRepaymentUpload.SeasonId);


            Assert.Equal(800, summaryAfterUpload.TotalRepaid);
        }

        [Fact]
        public async Task UploadWithSeasonIdAmountGreaterThanDebt()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 2,
                SeasonId = 1,
                Amount = 1000,
                Date = DateTime.Now
            };

            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Single(repayments);

            var repayment = repayments.First();
            Assert.Equal(mockRepaymentUpload.Amount, repayment.Amount);
            Assert.Equal(mockRepaymentUpload.CustomerId, repayment.CustomerId);
            Assert.Equal(mockRepaymentUpload.SeasonId, repayment.SeasonId);

            var summaryAfterUpload = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs =>
                    cs.CustomerId == mockRepaymentUpload.CustomerId && cs.SeasonId == mockRepaymentUpload.SeasonId);


            Assert.Equal(2200, summaryAfterUpload.TotalRepaid);
        }

        [Fact]
        public async Task UploadWithoutSeasonIdAmountLessThanDebt()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 1,
                SeasonId = 1,
                Amount = 100,
                Date = DateTime.Now
            };

            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Single(repayments);

            var repayment = repayments.First();
            Assert.Equal(mockRepaymentUpload.Amount, repayment.Amount);
            Assert.Equal(mockRepaymentUpload.CustomerId, repayment.CustomerId);
            Assert.Equal(1, repayment.SeasonId);

            var summaryAfterUpload = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs =>
                    cs.CustomerId == mockRepaymentUpload.CustomerId && cs.SeasonId == repayment.SeasonId);

            Assert.Equal(300, summaryAfterUpload.TotalRepaid);
        }

        [Fact]
        public async Task UploadWithoutSeasonIdAmountGreaterThanDebt()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 1,
                Amount = 2000,
                Date = DateTime.Now
            };

            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Equal(3, repayments.Count);


            Assert.Equal(2000, repayments[0].Amount);
            Assert.Null(repayments[0].ParentId);
            Assert.Equal(1, repayments[0].SeasonId);

            Assert.Equal(-1200, repayments[1].Amount);
            Assert.Equal(repayments[0].Id, repayments[1].ParentId);
            Assert.Equal(1, repayments[1].SeasonId);

            Assert.Equal(1200, repayments[2].Amount);
            Assert.Equal(repayments[0].Id, repayments[2].ParentId);
            Assert.Equal(2, repayments[2].SeasonId);

            var summaryAfterUploads = await _dbContext.CustomerSummaries
                .Where(cs => cs.CustomerId == 1)
                .OrderBy(cs => cs.Season.StartDate)
                .ToListAsync();

            Assert.Equal(2, summaryAfterUploads.Count);
            Assert.Equal(1000, summaryAfterUploads[0].TotalRepaid);
            Assert.Equal(1900, summaryAfterUploads[1].TotalRepaid);
        }

        [Fact]
        public async Task UploadWithoutSeasonIdAmountGreaterThanDebtExpectedOverflow()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 3,
                Amount = 500,
                Date = DateTime.Now
            };

            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Single(repayments);


            Assert.Equal(500, repayments[0].Amount);
            Assert.Null(repayments[0].ParentId);
            Assert.Equal(1, repayments[0].SeasonId);

            var summaryAfterUpload = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs => cs.CustomerId == 3 && cs.SeasonId == 1);

            Assert.Equal(1500, summaryAfterUpload.TotalRepaid);
        }

        [Fact]
        public async Task UploadWithoutSeasonIdWhenNoOutstandingDebt()
        {
            var mockRepaymentUpload = new RepaymentUpload
            {
                CustomerId = 4,
                Amount = 500,
                Date = DateTime.Now
            };

            var repayments = await _repaymentService.ProcessUpload(mockRepaymentUpload);

            Assert.NotNull(repayments);
            Assert.Single(repayments);


            Assert.Equal(500, repayments[0].Amount);
            Assert.Null(repayments[0].ParentId);
            Assert.Equal(2, repayments[0].SeasonId);

            var summaryAfterUpload = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs => cs.CustomerId == 4 && cs.SeasonId == 2);

            Assert.Equal(800, summaryAfterUpload.TotalRepaid);
        }

        [Fact]
        public async Task UploadMultiple()
        {
            var mockRepaymentUploads = new List<RepaymentUpload>
            {
                new RepaymentUpload {CustomerId = 1, Amount = 1500, Date = DateTime.Now},
                new RepaymentUpload {CustomerId = 3, Amount = 300, SeasonId = 2, Date = DateTime.Now}
            };

            var repayments = await _repaymentService.ProcessUploads(mockRepaymentUploads);

            Assert.NotNull(repayments);
            Assert.Equal(4, repayments.Count);

            Assert.Equal(1500, repayments[0].Amount);
            Assert.Null(repayments[0].ParentId);
            Assert.Equal(1, repayments[0].SeasonId);

            Assert.Equal(-700, repayments[1].Amount);
            Assert.Equal(1, repayments[1].SeasonId);
            Assert.Equal(repayments[0].Id, repayments[1].ParentId);

            Assert.Equal(700, repayments[2].Amount);
            Assert.Equal(2, repayments[2].SeasonId);
            Assert.Equal(repayments[0].Id, repayments[2].ParentId);

            Assert.Equal(300, repayments[3].Amount);
            Assert.Null(repayments[3].ParentId);
            Assert.Equal(2, repayments[3].SeasonId);

            var summaryAfterUploadsForCustomer1 = await _dbContext.CustomerSummaries
                .Where(cs => cs.CustomerId == 1)
                .OrderBy(cs => cs.Season.StartDate)
                .ToListAsync();

            Assert.Equal(2, summaryAfterUploadsForCustomer1.Count);
            Assert.Equal(1000, summaryAfterUploadsForCustomer1[0].TotalRepaid);
            Assert.Equal(1400, summaryAfterUploadsForCustomer1[1].TotalRepaid);

            var summaryAfterUploadsForCustomer3 = await _dbContext.CustomerSummaries
                .FirstOrDefaultAsync(cs => cs.CustomerId == 3 && cs.SeasonId == 2);

            Assert.Equal(800, summaryAfterUploadsForCustomer3.TotalRepaid);
        }


        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}