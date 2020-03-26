using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Data.Repository
{
    public class BaseEntityRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly AppDbContext _dbContext;

        public BaseEntityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<TEntity>> List(Expression<Func<TEntity, object>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> dbSet = _dbContext.Set<TEntity>();
            dbSet = includeProperties.Aggregate(dbSet, (current, includeProperty) => current.Include(includeProperty));

            return (orderBy == null ? dbSet : dbSet.OrderBy(orderBy)).ToListAsync();
        }

        public Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> dbSet = _dbContext.Set<TEntity>();
            dbSet = includeProperties.Aggregate(dbSet, (current, includeProperty) => current.Include(includeProperty));

            return (orderBy == null ? dbSet.Where(predicate) : dbSet.Where(predicate).OrderBy(orderBy)).ToListAsync();
        }

        public Task<TEntity> GetById(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> dbSet = _dbContext.Set<TEntity>();
            dbSet = includeProperties.Aggregate(dbSet, (current, includeProperty) => current.Include(includeProperty));
            return dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }


        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity ?? throw new ArgumentNullException(nameof(entity)));
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.FirstOrDefaultAsync(predicate);
        }
    }
}