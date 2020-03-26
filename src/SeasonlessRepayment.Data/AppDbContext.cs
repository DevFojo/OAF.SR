using Microsoft.EntityFrameworkCore;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<CustomerSummary> CustomerSummaries { get; set; }
        public DbSet<Repayment> Repayments { get; set; }
        public DbSet<RepaymentUpload> RepaymentUploads { get; set; }
    }
}