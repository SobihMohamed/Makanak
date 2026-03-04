using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Contracts
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
