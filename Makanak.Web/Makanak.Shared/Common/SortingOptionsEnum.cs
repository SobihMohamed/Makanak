using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Makanak.Shared.Common
{
    public enum SortingOptionsEnum
    {
        [Description("Name (A-Z)")]
        NameAsc = 1,

        [Description("Name (Z-A)")]
        NameDesc = 2,

        [Description("Oldest First")]
        DateCreatedAsc = 3,

        [Description("Newest First")]
        DateCreatedDesc = 4,

        [Description("Price (Low to High)")]
        PriceAsc = 5,

        [Description("Price (High to Low)")]
        PriceDesc = 6
    }
}
