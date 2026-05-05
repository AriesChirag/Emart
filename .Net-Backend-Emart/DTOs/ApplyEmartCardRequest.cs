namespace Emart_DotNet.DTOs
{
    public class ApplyEmartCardRequest
    {
        public int UserId { get; set; }

        public decimal AnnualIncome { get; set; }

        public string PanCard { get; set; } = string.Empty;

        public string BankDetails { get; set; } = string.Empty;

        public string Occupation { get; set; } = string.Empty;

        public string EducationQualification { get; set; } = string.Empty;

        // Optional fields
        public DateOnly? BirthDate { get; set; }

        public string? Interests { get; set; }
    }
}
