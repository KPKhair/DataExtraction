using DataExtraction.Domain.Models;

namespace DataExtraction.Application.Interfaces
{
    public interface IDataProcessorRepository
    {
        // Reads data from the input file using the specified bank parser
        Task<IEnumerable<dynamic>> ReadDataAsync(string inputFile, IBankParser parser);

        // Writes the processed data to the output file
        Task WriteDataAsync(IEnumerable<ParsedRecord> records, string outputFile);
    }
}
