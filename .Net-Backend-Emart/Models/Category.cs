using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Emart_DotNet.Models;

[Table("category")]
public partial class Category
{
    [Key]
    [Column("category_id")]
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [Column("category_name")]
    [StringLength(255)]
    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [Column("parent_category_id")]
    [JsonIgnore]
    public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    [InverseProperty("InverseParentCategory")]
    [JsonPropertyName("parentCategory")]
    public virtual Category? ParentCategory { get; set; }

    [InverseProperty("ParentCategory")]
    [JsonIgnore]
    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    [InverseProperty("Category")]
    [JsonIgnore]
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
