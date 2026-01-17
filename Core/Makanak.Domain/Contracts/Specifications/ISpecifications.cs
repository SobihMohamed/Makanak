using Makanak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Makanak.Domain.Contracts.Specifications
{
    public interface ISpecifications<TEntity, TKey> where TEntity : class , IEntity<TKey>
    {
        Expression<Func<TEntity,bool>> Criteria { get; } // RETURN T OR F 
        List<Expression<Func<TEntity,object>>> Includes { get; }

    }
}
