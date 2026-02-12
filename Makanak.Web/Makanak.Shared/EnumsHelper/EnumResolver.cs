using Makanak.Shared.Dto_s.Lookup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.EnumsHelper
{
    public static class EnumResolver
    {
        public static IEnumerable<EnumLookupDto> GetEnumList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new EnumLookupDto
                       {
                           Id = Convert.ToInt32(e),
                           Name = e.ToString()
                       });
        }
    }
}
