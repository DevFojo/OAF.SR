using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Data.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<List<TEntity>> List(Expression<Func<TEntity, object>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> GetById(int id, params Expression<Func<TEntity, object>>[] includeProperties);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Add(IEnumerable<TEntity> getRange);

        Task<TEntity> GetSingle(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties);
    }
}