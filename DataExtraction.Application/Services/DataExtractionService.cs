using DataExtraction.Application.Interfaces;

namespace DataExtraction.Application.Services
{
    public class DataExtractionService : IDataExtractionService
    {
        private readonly IEnumerable<IBankParser> _parsers;
        private readonly IDataProcessorRepository _repository;

        /// <summary>
        /// ProcessBankCsvUseCase constructor
        /// </summary>
        /// <param name="parsers"></param>
        /// <param name="repository"></param>
        public DataExtractionService(IEnumerable<IBankParser> parsers, IDataProcessorRepository repository)
        {
            // Inject all parsers and repository
            _parsers = parsers;
            _repository = repository;
        }

        /// <summary>
        /// Starts the CSV processing for the specified bank and input file.
        /// </summary>
        /// <param name="bankName">Bank name input from cmd</param>
        /// <param name="inputFile">Input file name and location</param>
        /// <returns></returns>
        public async Task ExecuteAsync(string bankName, string inputFile)
        {
            try
            {
                //Input Validation
                if (string.IsNullOrWhiteSpace(bankName))
                    throw new ArgumentException("Bank name cannot be empty.");

                if (!File.Exists(inputFile))
                    throw new FileNotFoundException($"Input file not found: {inputFile}");

                var parser = _parsers.FirstOrDefault(p => p.BankName.Equals(bankName, StringComparison.OrdinalIgnoreCase));
                if (parser == null) throw new Exception($"Parser not found for bank: {bankName}");

                //Read and parse records
                var rawRecords = await _repository.ReadDataAsync(inputFile, parser);
                var parsedRecords = parser.Parse(rawRecords);

                //Set output file path
                var outputFile = GenerateOutputFileName(inputFile);

                //Write parsed records to output file
                await _repository.WriteDataAsync(parsedRecords, outputFile);
                Console.WriteLine($"Extraction complete. Output written to: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($" Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the output file name based on input file and location
        /// </summary>
        /// <param name="inputFile">Input file loaction</param>
        /// <returns></returns>
        private string GenerateOutputFileName(string inputFile)
        {
            var directory = Path.GetDirectoryName(inputFile);
            var filenameWithoutExt = Path.GetFileNameWithoutExtension(inputFile);
            var extension = Path.GetExtension(inputFile);

            // Example: DataExtractor_Example_Input.csv -> DataExtractor_Example_Output.csv
            var newFileName = $"{filenameWithoutExt.Replace("_Input", "_Output")}{extension}";
            return Path.Combine(directory ?? ".", newFileName);
        }
    }
}
