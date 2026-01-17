using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Contracts.Repos
{
    public interface IGenericRepo<TEntity, TKey> where TEntity : class , IEntity<TKey>
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, TKey> specifications);
        public Task<TEntity> GetByIdAsync(TKey id);
        public Task<TEntity> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications);
        public void AddAsync(TEntity entity);
        public void Update(TEntity entity);

        public void Delete(TEntity entity);
        public void DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}
