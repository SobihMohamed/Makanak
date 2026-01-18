using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Makanak.Services.Specifications
{
    public abstract class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {

        #region Where
        public Expression<Func<TEntity, bool>> Criteria { get; private set; }
        public BaseSpecifications(Expression<Func<TEntity, bool>> _Criteria)
        {
            Criteria = _Criteria;
        }
        #endregion

        #region Includes (Join)
        public List<Expression<Func<TEntity, object>>> Includes { get; private set; } = [];
        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        #endregion

        #region OrderBy
        public Expression<Func<TEntity, object>> OrderBy { get; private set; }
        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        #endregion
    }
}
