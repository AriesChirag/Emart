using System;

namespace Emart_DotNet.Models;

public partial class Product
{
    public decimal GetEffectivePrice()
    {
        decimal normal = NormalPrice;
        decimal discount = DiscountPercent ?? 0m;

        if (discount > 0)
        {
            return normal - (normal * discount / 100m);
        }
        return normal;
    }
}
