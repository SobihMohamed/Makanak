using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Makanak.Services.Specifications
{
    public class OrderExpressionInfo<TEntity>
    {
        public Expression<Func<TEntity, object>> Expression { get; set; }
        public bool IsDescending { get; set; }
    }
}
