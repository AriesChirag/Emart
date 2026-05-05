namespace Emart_DotNet.DTOs
{
    public class EmartCardDTO
    {
        public int CardId { get; set; }
        
        public int UserId { get; set; }
        
        public string FullName { get; set; } = string.Empty;

        public DateOnly PurchaseDate { get; set; }
        
        public DateOnly ExpiryDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
