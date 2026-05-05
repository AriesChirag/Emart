using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.FindByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
           
            return await _productRepository.SaveAsync(product);
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var existing = await _productRepository.FindByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Product not found");

            
            existing.ProductName = product.ProductName;
            existing.ProductImageUrl = product.ProductImageUrl;
            existing.NormalPrice = product.NormalPrice;
            existing.EcardPrice = product.EcardPrice;
            existing.AvailableQuantity = product.AvailableQuantity;
            existing.Description = product.Description;
            existing.StoreId = product.StoreId;
            existing.SubcategoryId = product.SubcategoryId;
            existing.DiscountPercent = product.DiscountPercent;
            
            return await _productRepository.SaveAsync(existing);
        }

        public async Task DeleteProductAsync(int id)
        {
            if (!await _productRepository.ExistsAsync(id))
                 throw new KeyNotFoundException("Product not found");

            await _productRepository.DeleteAsync(id);
        }

        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            return await _productRepository.SearchProductsAsync(keyword);
        }

        public async Task UploadProductsAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            using (var stream = new System.IO.StreamReader(file.OpenReadStream()))
            {
                // Skip Header
                string? header = await stream.ReadLineAsync();
                
                while (!stream.EndOfStream)
                {
                    string? line = await stream.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');
                    // Assuming CSV Format matching Java model or simplified
                    // Name,Desc,Price,Img,Qty,StoreId,SubCatId
                    if (values.Length < 5) continue; 

                    try 
                    {
                        var product = new Product
                        {
                            ProductName = values[0].Trim(),
                            Description = values[1].Trim(),
                            NormalPrice = decimal.Parse(values[2]),
                            ProductImageUrl = values[3].Trim(),
                            AvailableQuantity = int.Parse(values[4]),
                            // Optional fields or defaults
                            StoreId = values.Length > 5 ? int.Parse(values[5]) : null,
                            SubcategoryId = values.Length > 6 ? int.Parse(values[6]) : null
                            // EcardPrice, Discount logic... default 0 or calc
                        };
                        
                        // Save individually or batch
                        await _productRepository.SaveAsync(product);
                    }
                    catch
                    {
                        // Skip bad lines or log
                        continue;
                    }
                }
            }
        }
    }
}
