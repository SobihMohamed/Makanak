using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Contracts.InitializerDB
{
    public interface IDbInitializer
    {
        Task DataSeedAsync();
    }
}
