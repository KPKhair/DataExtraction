using DataExtraction.Domain.Models;

namespace DataExtraction.Application.Interfaces
{
    public interface IBankParser
    {
        // Bank Identifier
        string BankName { get; }

        // Metadata List
        string[] MetadataPrefixes { get; }

        // Parse dynamic records into strongly typed Records
        IEnumerable<ParsedRecord> Parse(IEnumerable<dynamic> rawRecords);
    }
}
