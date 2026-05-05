using AnalyticsEngineApi.Models;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace AnalyticsEngineApi.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
}
