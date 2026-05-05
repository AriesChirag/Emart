using AnalyticsEngineApi.Data;
using AnalyticsEngineApi.DTOs;
using System.Globalization;

namespace AnalyticsEngineApi.Services;

public class AnalyticsService
{
    private readonly AnalyticsDbContext _db;

    public AnalyticsService(AnalyticsDbContext db)
    {
        _db = db;
    }

    public List<ProductOfferInventoryDTO> GetProductOffersInventory()
    {
        return _db.Products
            .Select(p => new ProductOfferInventoryDTO
            {
                Product_Name = p.Product_Name,

                Discount_Offer =
                    p.Discount_Percent != null && p.Discount_Percent > 0
                        ? p.Discount_Percent + "% OFF"
                        : "No discount available",

                Quantity = p.Available_Quantity ?? 0
            })
            .ToList();
    }

}
