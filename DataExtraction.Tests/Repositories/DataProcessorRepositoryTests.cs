using DataExtraction.Domain.Models;
using DataExtraction.Infrastructure.Parsers;
using DataExtraction.Infrastructure.Repositories;

namespace DataExtraction.Tests.Repositories
{
    public class DataProcessorRepositoryTests
    {

        private readonly DataProcessorRepository _repository = new DataProcessorRepository();
        private readonly BarclayParser _parser = new BarclayParser();

        private string CreateTempCsvFile(IEnumerable<string> lines)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, lines);
            return tempFile;
        }

        [Fact]
        public async Task ReadDataAsync_ShouldReturnRecords_AndSkipMetadataLines()
        {
            var lines = new[]
            {
                "TimeZone=UTC",
                "ISIN,CFICode,Venue,AlgoParams",
                "US123456,EQUI,NYSE,PriceMultiplier:10|;Other:5|",
                "US654321,BOND,NASDAQ,Other:5|"
            };
            var tempFile = CreateTempCsvFile(lines);

            var result = await _repository.ReadDataAsync(tempFile, _parser);

            Assert.Equal(2, result.Count()); // Metadata skipped
            Assert.Contains(result, r => r.ISIN == "US123456");
            Assert.Contains(result, r => r.ISIN == "US654321");

            File.Delete(tempFile);
        }

        [Fact]
        public async Task WriteDataAsync_ShouldCreateCsvFileWithRecords()
        {
            var outputFile = Path.Combine(Path.GetTempPath(), "test_output.csv");
            var records = new List<ParsedRecord>
            {
                new ParsedRecord { ISIN = "US123", CFICode = "EQUI", Venue = "NYSE", ContractSize = "10" },
                new ParsedRecord { ISIN = "US456", CFICode = "BOND", Venue = "NASDAQ", ContractSize = "5" }
            };

            await _repository.WriteDataAsync(records, outputFile);

            Assert.True(File.Exists(outputFile));
            var lines = await File.ReadAllLinesAsync(outputFile);
            Assert.Equal(3, lines.Length); // Header + 2 rows

            File.Delete(outputFile);
        }

    }
}
