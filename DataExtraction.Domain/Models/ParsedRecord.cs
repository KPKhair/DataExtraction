namespace DataExtraction.Domain.Models
{
    public class ParsedRecord
    {
        public string? ISIN { get; set; }
        public string? CFICode { get; set; }
        public string? Venue { get; set; }
        public string? ContractSize { get; set; } // Extracted from AlgoParams
    }
}
