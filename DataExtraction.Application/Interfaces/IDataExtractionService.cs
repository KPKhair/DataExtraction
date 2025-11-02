namespace DataExtraction.Application.Interfaces
{
    public interface IDataExtractionService
    {
        //Initiate start of data extraction process
        Task ExecuteAsync(string bankName, string inputFile);
    }
}
