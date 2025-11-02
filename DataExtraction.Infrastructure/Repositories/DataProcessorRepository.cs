using DataExtraction.Application.Interfaces;
using DataExtraction.Domain.Models;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataExtraction.Infrastructure.Repositories
{
    public class DataProcessorRepository : IDataProcessorRepository
    {
        /// <summary>
        /// Cleanse and reads the CSV file, returning dynamic records.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<dynamic>> ReadDataAsync(string inputFile, IBankParser parser)
        {
            try
            {
                // Build configuration explicitly
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    IgnoreBlankLines = true
                };
                var cleanedFile = await PreprocessCsvAsync(inputFile, parser);

                using var reader = new StreamReader(cleanedFile);
                using var csv = new CsvReader(reader, config);

                return csv.GetRecords<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to read CSV for {parser.BankName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Preprocesses the CSV file to remove metadata lines based on parser rules.
        /// </summary>
        /// <param name="inputFile">Input CSV file</param>
        /// <param name="parser">Holds metadata information</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private async Task<string> PreprocessCsvAsync(string inputFile, IBankParser parser)
        {
            try
            {
                var tempFile = Path.GetTempFileName();

                using (var reader = new StreamReader(inputFile, Encoding.UTF8))
                await using (var writer = new StreamWriter(tempFile, false, Encoding.UTF8))
                {
                    string? line;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Skip metadatafor banks
                        if (parser.MetadataPrefixes.Any(p => line.StartsWith(p)))
                            continue;

                        await writer.WriteLineAsync(line);
                    }
                }

                return tempFile;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error preprocessing file {inputFile}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves the extracted data to a CSV file.
        /// </summary>
        /// <param name="records">Extracted data from CSV</param>
        /// <param name="outputFile">File location to save extracted data </param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task WriteDataAsync(IEnumerable<ParsedRecord> records, string outputFile)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim
                };

                await using var writer = new StreamWriter(outputFile);
                await using var csv = new CsvWriter(writer, config);
                await csv.WriteRecordsAsync(records);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to write output file '{outputFile}': {ex.Message}", ex);
            }
        }
    }
}
