using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAll();
        Task Create(string name);
    }
}