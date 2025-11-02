using System.Dynamic;
using DataExtraction.Infrastructure.Parsers;

namespace DataExtraction.Tests.BankParsers
{
    public class BarclayParserTests
    {
        [Theory]
        [InlineData("US123", "CFI1", "NYSE", "PriceMultiplier:10|;", "10")]
        [InlineData("GB456", "CFI2", "LSE", "PriceMultiplier:20|;", "20.0")]
        [InlineData("JP789", "CFI3", "TSE", null, null)] // test null AlgoParams
        [InlineData("FR000", "CFI4", "Euronext", "", null)] // test empty AlgoParams
        [InlineData("FR000", "CFI4", null, "", null)] // test empty venue
        public void Parse_ShouldReturnCorrectRecords(string? isin, string? cficode, string? venue, string? algoParams, string? expectedContractSize)
        {
            var parser = new BarclayParser();

            var rawRecords = new List<dynamic>();
            dynamic record = new ExpandoObject();
            record.ISIN = isin;
            record.CFICode = cficode;
            record.Venue = venue;
            record.AlgoParams = algoParams;
            rawRecords.Add(record);

            var result = parser.Parse(rawRecords).ToList();

            Assert.Single(result);
            Assert.Equal(isin, result[0].ISIN);
            Assert.Equal(expectedContractSize, result[0].ContractSize);
        }
    }
}
