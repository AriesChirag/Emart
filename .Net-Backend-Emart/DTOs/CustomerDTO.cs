namespace Emart_DotNet.DTOs
{
    public class CustomerDTO
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public int Epoints { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string? Interests { get; set; }

        // Emart Card summary (optional)
        public EmartCardDTO? EmartCard { get; set; }
    }
}
