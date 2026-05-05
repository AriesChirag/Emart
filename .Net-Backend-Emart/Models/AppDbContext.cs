using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Emart_DotNet.Converters;

namespace Emart_DotNet.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<EmartCard> EmartCards { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.ToTable("address");

            entity.HasIndex(e => e.UserId, "FKiquku6nnetd0a05ek7whbltf0");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.HouseNumber)
                .HasMaxLength(255)
                .HasColumnName("house_number");
            entity.Property(e => e.Landmark)
                .HasMaxLength(255)
                .HasColumnName("landmark");
            entity.Property(e => e.Pincode)
                .HasMaxLength(255)
                .HasColumnName("pincode");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
            entity.Property(e => e.Town)
                .HasMaxLength(255)
                .HasColumnName("town");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FKiquku6nnetd0a05ek7whbltf0");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.HasIndex(e => e.Email, "UKc0r9atamxvbhjjvy5j8da1kam").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Active)
                .HasColumnType("bit(1)")
                .HasColumnName("active");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PRIMARY");

            entity.ToTable("cart");

            entity.HasIndex(e => e.UserId, "FKgol9os2lyo1m4bu3aa5rg7jyl");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CouponDiscount)
                .HasPrecision(38, 2)
                .HasColumnName("coupon_discount");
            entity.Property(e => e.EarnedEpoints).HasColumnName("earned_epoints");
            entity.Property(e => e.EpointDiscount)
                .HasPrecision(38, 2)
                .HasColumnName("epoint_discount");
            entity.Property(e => e.FinalPayableAmount)
                .HasPrecision(38, 2)
                .HasColumnName("final_payable_amount");
            entity.Property(e => e.PlatformFee)
                .HasPrecision(38, 2)
                .HasColumnName("platform_fee");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(38, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TotalMrp)
                .HasPrecision(38, 2)
                .HasColumnName("total_mrp");
            entity.Property(e => e.UsedEpoints).HasColumnName("used_epoints");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FKgol9os2lyo1m4bu3aa5rg7jyl");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PRIMARY");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.CartId, "FK99e0am9jpriwxcm6is7xfedy3");

            entity.HasIndex(e => e.ProductId, "FKl7je3auqyq1raj52qmwrgih8x");

            entity.Property(e => e.CartItemId).HasColumnName("cart_item_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.EpointsUsed).HasColumnName("epoints_used");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.PurchaseType)
                .HasMaxLength(255)
                .HasColumnName("purchase_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(38, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(38, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK99e0am9jpriwxcm6is7xfedy3");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FKl7je3auqyq1raj52qmwrgih8x");
        });



        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.CardHolderId, "FK7adfelh6bpnfyuojtya9ac5g6");

            entity.HasIndex(e => e.AddressId, "FKglkhkmh2vyn790ijs6hiqqpi");

            entity.HasIndex(e => e.Email, "UKdwk6cx0afu8bs9o4t536v1j5v").IsUnique();

            entity.HasIndex(e => e.MembershipNumber, "UKp7j34m98syxlm3689jhuqq1vk").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AuthProvider)
                 .HasConversion<string>()
                .HasColumnName("auth_provider");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CardHolderId).HasColumnName("card_holder_id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Epoints).HasColumnName("epoints");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Interests)
                .HasMaxLength(255)
                .HasColumnName("interests");
            entity.Property(e => e.MembershipNumber).HasColumnName("membership_number");
            entity.Property(e => e.Mobile)
                .HasMaxLength(255)
                .HasColumnName("mobile");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.ProfileCompleted)
                .HasColumnType("bit(1)")
                .HasColumnName("profile_completed");
            entity.Property(e => e.PromotionalEmail)
                .HasColumnType("bit(1)")
                .HasColumnName("promotional_email");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");

            entity.HasOne(d => d.Address).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FKglkhkmh2vyn790ijs6hiqqpi");

            entity.HasOne(d => d.CardHolder).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CardHolderId)
                .HasConstraintName("FK7adfelh6bpnfyuojtya9ac5g6");
        });



        modelBuilder.Entity<EmartCard>(entity =>
        {
            entity.HasKey(e => e.CardId).HasName("PRIMARY");

            entity.ToTable("emart_card");

            entity.HasIndex(e => e.UserId, "UKtegi3qhj6254t9jufxgstywck").IsUnique();

            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.AnnualIncome).HasColumnName("annual_income");
            entity.Property(e => e.BankDetails)
                .HasMaxLength(255)
                .HasColumnName("bank_details");
            entity.Property(e => e.EducationQualification)
                .HasMaxLength(255)
                .HasColumnName("education_qualification");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.Occupation)
                .HasMaxLength(255)
                .HasColumnName("occupation");
            entity.Property(e => e.PanCard)
                .HasMaxLength(255)
                .HasColumnName("pan_card");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TotalEpointsUsed).HasColumnName("total_epoints_used");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "FK4icpel3dfbpjoi1lom0dhatcg");

            entity.HasIndex(e => e.StoreId, "FK5n14sr4mswfdtaoiwj7rkt0mw");

            entity.HasIndex(e => e.AddressId, "FKf5464gxwc32ongdvka2rtvw96");

            entity.HasIndex(e => e.CartId, "FKtpihbdn6ws0hu56camb0bg2to");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.DeliveryType)
                 .HasConversion(new DeliveryTypeConverter())
                .HasColumnName("delivery_type");
            entity.Property(e => e.EpointsEarned).HasColumnName("epoints_earned");
            entity.Property(e => e.EpointsUsed).HasColumnName("epoints_used");
            entity.Property(e => e.OrderDate)
                .HasMaxLength(6)
                .HasColumnName("order_date");
            entity.Property(e => e.PaymentMethod)
                .HasConversion(new PaymentMethodConverter())
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                 .HasConversion(new PaymentStatusConverter())
                .HasColumnName("payment_status");
            entity.Property(e => e.Status)
                .HasConversion(new OrderStatusConverter())
                .HasColumnName("status");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(38, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FKf5464gxwc32ongdvka2rtvw96");

            entity.HasOne(d => d.Cart).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FKtpihbdn6ws0hu56camb0bg2to");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("FK5n14sr4mswfdtaoiwj7rkt0mw");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK4icpel3dfbpjoi1lom0dhatcg");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "FKbioxgbv59vetrxe0ejfubep1w");

            entity.HasIndex(e => e.ProductId, "FKlf6f9q956mt144wiv6p1yko16");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasPrecision(38, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(38, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKbioxgbv59vetrxe0ejfubep1w");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKlf6f9q956mt144wiv6p1yko16");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.HasIndex(e => e.OrderId, "FKlouu98csyullos9k25tbpk4va");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Mode)
                .HasConversion<string>()
                .HasColumnName("mode");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasMaxLength(6)
                .HasColumnName("payment_date");
            entity.Property(e => e.RazorpayOrderId)
                .HasMaxLength(255)
                .HasColumnName("razorpay_order_id");
            entity.Property(e => e.RazorpayPaymentId)
                .HasMaxLength(255)
                .HasColumnName("razorpay_payment_id");
            entity.Property(e => e.RazorpaySignature)
                .HasMaxLength(255)
                .HasColumnName("razorpay_signature");
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FKlouu98csyullos9k25tbpk4va");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PRIMARY");

            entity.ToTable("customer_invoice");

            entity.HasIndex(e => e.OrderId, "FK_Invoice_Order");
            entity.HasIndex(e => e.UserId, "FK_Invoice_User");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.BillingAddress).HasColumnName("billing_address");
            entity.Property(e => e.DeliveryType)
                 .HasConversion(new DeliveryTypeConverter())
                 .HasColumnName("delivery_type");
            entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount");
            entity.Property(e => e.EpointsBalance).HasColumnName("epoints_balance");
            entity.Property(e => e.EpointsEarned).HasColumnName("epoints_earned");
            entity.Property(e => e.EpointsUsed).HasColumnName("epoints_used");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");
            entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Invoice_Order");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Invoice_User");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.SubcategoryId, "FKniucpti15id7jc1gqsnlcpd0b");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.AvailableQuantity).HasColumnName("available_quantity");
            entity.Property(e => e.Description)
                .HasMaxLength(5000)
                .HasColumnName("description");
            entity.Property(e => e.DiscountPercent)
                .HasPrecision(5, 2)
                .HasColumnName("discount_percent");
            entity.Property(e => e.EcardPrice)
                .HasPrecision(38, 2)
                .HasColumnName("ecard_price");
            entity.Property(e => e.NormalPrice)
                .HasPrecision(38, 2)
                .HasColumnName("normal_price");
            entity.Property(e => e.ProductImageUrl)
                .HasMaxLength(1000)
                .HasColumnName("product_image_url");
            entity.Property(e => e.ProductName)
                .HasMaxLength(500)
                .HasColumnName("product_name");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .HasConstraintName("FKniucpti15id7jc1gqsnlcpd0b");
        });





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
