using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public interface ISeasonService
    {
        Task<List<Season>> GetAll();
        Task Create(string name);
    }
}