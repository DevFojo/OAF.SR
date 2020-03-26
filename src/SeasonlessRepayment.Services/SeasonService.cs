using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Data;
using SeasonlessRepayment.Data.Repository;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly IBaseRepository<Season> _seasonRepository;
        private readonly AppDbContext _dbContext;

        public SeasonService(IBaseRepository<Season> seasonRepository, AppDbContext dbContext)
        {
            _seasonRepository = seasonRepository;
            _dbContext = dbContext;
        }

        public Task<List<Season>> GetAll()
        {
            return _seasonRepository.List();
        }

        public Task Create(string name)
        {
            _seasonRepository.Add(new Season
            {
                Name = name
            });
            return _dbContext.SaveChangesAsync();
        }
    }
}