using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

[Table("sub_category")]
public partial class SubCategory
{
    [Key]
    [Column("sub_category_id")]
    [JsonPropertyName("subCategoryId")]
    public int SubCategoryId { get; set; }

    [Column("brand")]
    [StringLength(255)]
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [Column("sponsors")]
    [JsonPropertyName("sponsors")]
    public bool Sponsors { get; set; }

    [Column("category_id")]
    [JsonIgnore]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("SubCategories")]
    [JsonPropertyName("category")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Subcategory")]
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
