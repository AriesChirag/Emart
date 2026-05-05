using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emart_DotNet.Models;

[Table("store")]
public partial class Store
{
    [Key]
    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("store_name")]
    [StringLength(255)]
    public string StoreName { get; set; } = null!;

    [Column("availability")]
    public bool Availability { get; set; }

    [Column("city")]
    [StringLength(255)]
    public string City { get; set; } = null!;

    [Column("address")]
    [StringLength(255)]
    public string Address { get; set; } = null!;

    [Column("contact_number")]
    [StringLength(255)]
    public string ContactNumber { get; set; } = null!;

    [InverseProperty("Store")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
