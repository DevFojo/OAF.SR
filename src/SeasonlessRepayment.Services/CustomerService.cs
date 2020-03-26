using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Data;
using SeasonlessRepayment.Data.Repository;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly AppDbContext _dbContext;

        public CustomerService(IBaseRepository<Customer> customerRepository, AppDbContext dbContext)
        {
            _customerRepository = customerRepository;
            _dbContext = dbContext;
        }

        public Task<List<Customer>> GetAll()
        {
            return _customerRepository.List();
        }

        public Task Create(string name)
        {
            _customerRepository.Add(new Customer
            {
                Name = name
            });
            return _dbContext.SaveChangesAsync();
        }
    }
}