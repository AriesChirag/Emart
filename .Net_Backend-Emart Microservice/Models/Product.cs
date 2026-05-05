using System.ComponentModel.DataAnnotations.Schema;

namespace AnalyticsEngineApi.Models
{
    [Table("product")]
    public class Product
    {
        public int? ProductId { get; set; }
        public int? Available_Quantity { get; set; }
        public string Description { get; set; }
        public double? Ecard_Price { get; set; }
        public string? Product_Image_Url { get; set; }
        public string? Product_Name { get; set; }
        public double? Normal_Price { get; set; }
        public int? StoreId { get; set; }
        public int? SubcategoryId { get; set; }
        
        [Column("discount_percent")]
        public double? Discount_Percent { get; set; }
    }


}
