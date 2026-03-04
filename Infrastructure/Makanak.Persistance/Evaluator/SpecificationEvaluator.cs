using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            if (Specifications.OrderExpressions != null && Specifications.OrderExpressions.Count > 0)
            {
                // 1. التعامل مع أول ترتيب (Primary Sort)
                // لازم يتحول لـ IOrderedQueryable عشان يقبل ThenBy بعد كدة
                var firstSort = Specifications.OrderExpressions[0];

                IOrderedQueryable<TEntity> orderedQuery = firstSort.IsDescending
                    ? Query.OrderByDescending(firstSort.Expression)
                    : Query.OrderBy(firstSort.Expression);

                // 2. التعامل مع الترتيبات التالية (Secondary Sorts)
                for (int i = 1; i < Specifications.OrderExpressions.Count; i++)
                {
                    var nextSort = Specifications.OrderExpressions[i];

                    orderedQuery = nextSort.IsDescending
                        ? orderedQuery.ThenByDescending(nextSort.Expression)
                        : orderedQuery.ThenBy(nextSort.Expression);
                }

                // تحديث الكويري النهائي
                Query = orderedQuery;
            }

            if (Specifications.IsPagingEnabled)
            {
                Query = Query.Skip(Specifications.Skip).Take(Specifications.Take);
            }


            return Query;
        }
    }
}
