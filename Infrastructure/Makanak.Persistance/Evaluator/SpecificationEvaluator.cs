using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Makanak.Persistance.Evaluator
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GenerateQueery<TEntity, TKey>
            (IQueryable<TEntity> BaseQuery , ISpecifications<TEntity, TKey> Specifications) 
            where TEntity : class, IEntity<TKey>
        {
            var Query = BaseQuery;
            if(Specifications.Criteria is not null)
            {
                Query = Query.Where(Specifications.Criteria);
            }
            if (Specifications.Includes is not null && Specifications.Includes.Any())
            {
                Query = Specifications.Includes.Aggregate(Query, (CurrentQuery, Expression) => CurrentQuery.Include(Expression));
            }
            return Query;
        }
    }
}
