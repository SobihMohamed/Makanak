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
        IGenericRepo<TEntity, Key> GetRepo<TEntity, Key>() where TEntity : BaseEntity<Key>;
        public Task<int> SaveChangesAsync();
    }
}
