using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Models;
using Makanak.Persistance.Contexts;
using Makanak.Persistance.Implements.ReposImplement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Implements.UOW
{
    public class UnitOfWork(MakanakDbContext _context) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repos = [];
        public IGenericRepo<TEntity, Key> GetRepo<TEntity, Key>() where TEntity : class , IEntity<Key>
        {
            var RepoType = typeof(TEntity).Name;
            if (_repos.ContainsKey(RepoType))
                return (IGenericRepo<TEntity, Key>) _repos[RepoType] ;
            var NewRepo = new GenericRepo<TEntity, Key>(_context);
            _repos.Add(RepoType, NewRepo);
            return NewRepo;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
