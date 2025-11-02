using DataExtraction.Infrastructure.Helpers;
using DataExtraction.Application.Interfaces;
using DataExtraction.Domain.Models;

namespace DataExtraction.Infrastructure.Parsers
{
    public class BarclayParser : IBankParser
    {
        public string BankName => "Barclay";
        public string[] MetadataPrefixes => ["TimeZone="];

        /// <summary>
        /// Parses raw dynamic records into strongly typed Records.
        /// </summary>
        /// <param name="records"> Dynamic records from csv processing </param>
        /// <returns></returns>
        public IEnumerable<ParsedRecord> Parse(IEnumerable<dynamic> records)
        {
            foreach (var r in records)
            {
                yield return new ParsedRecord
                {
                    ISIN = r.ISIN,
                    CFICode = r.CFICode,
                    Venue = r.Venue,
                    ContractSize = ExtractPriceMultiplier(r.AlgoParams)
                };
            }
        }

        /// <summary>
        /// Gets the PriceMultiplier value from the AlgoParams field.
        /// </summary>
        /// <param name="algoParams">Get value from nested format</param>
        private string? ExtractPriceMultiplier(string algoParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(algoParams))
                    return null;

                var priceMultiplier = ParserHelper.GetValueFromPipeSeparatedString(algoParams, "PriceMultiplier", "|;", ":");
                return priceMultiplier;
            }
            catch
            {
                return null;
            }
        }


    }
}
