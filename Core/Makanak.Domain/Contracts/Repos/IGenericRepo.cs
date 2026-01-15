using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Contracts.Repos
{
    public interface IGenericRepo<TEntity, Key> where TEntity : BaseEntity<Key>
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, Key> specifications);
        public Task<TEntity> GetByIdAsync(Key id);
        public Task<TEntity> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, Key> specifications);
        public void AddAsync(TEntity entity);
        public void Update(TEntity entity);

        public void Delete(TEntity entity);
        public void DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}
