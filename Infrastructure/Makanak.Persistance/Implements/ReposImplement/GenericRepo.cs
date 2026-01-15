using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using Makanak.Persistance.Contexts;
using Makanak.Persistance.Evaluator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Implements.ReposImplement
{
    public class GenericRepo<TEntity, Key>(MakanakDbContext _context)
        : IGenericRepo<TEntity, Key> where TEntity : BaseEntity<Key>
    {

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _context.Set<TEntity>().ToListAsync();

        public async Task<TEntity> GetByIdAsync(Key id) => await _context.Set<TEntity>().FindAsync(id);
        public void AddAsync(TEntity entity) => _context.Set<TEntity>().AddAsync(entity);
        public void Update(TEntity entity) => _context.Set<TEntity>().Update(entity);
        public void Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, Key> specifications)
        {
            var BaseQuery = _context.Set<TEntity>();
            
            var Query = SpecificationEvaluator.GenerateQueery(BaseQuery, specifications);
 
            return await Query.ToListAsync();
        }

        public async Task<TEntity> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, Key> specifications)
        {

            var BaseQuery = _context.Set<TEntity>();
            
            var Query = SpecificationEvaluator.GenerateQueery(BaseQuery, specifications);
                
            return await Query.FirstOrDefaultAsync();
        }

        public void DeleteRangeAsync(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);


    }
}
