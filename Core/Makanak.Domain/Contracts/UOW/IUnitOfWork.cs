using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Domain.Contracts.UOW
{
    public interface IUnitOfWork
    {
        IGenericRepo<TEntity, TKey> GetRepo<TEntity, TKey>() where TEntity : class, IEntity<TKey>;
        public Task<int> SaveChangesAsync();
    }
}
