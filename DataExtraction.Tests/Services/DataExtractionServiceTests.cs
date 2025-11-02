using DataExtraction.Application.Interfaces;
using DataExtraction.Application.Services;
using DataExtraction.Domain.Models;
using Moq;

namespace DataExtraction.Tests.Services
{
    public class DataExtractionServiceTests
    {

        [Fact]
        public async Task ExecuteAsync_ValidInput_ProcessesSuccessfully()
        {
            var parserMock = new Mock<IBankParser>();
            parserMock.Setup(p => p.BankName).Returns("BankA");
            parserMock.Setup(p => p.Parse(It.IsAny<IEnumerable<dynamic>>()))
                .Returns(new List<ParsedRecord> { new ParsedRecord { ISIN = "ABC" } });

            var repoMock = new Mock<IDataProcessorRepository>();
            repoMock.Setup(r => r.ReadDataAsync(It.IsAny<string>(), It.IsAny<IBankParser>()))
                .ReturnsAsync(new List<dynamic> { new { ISIN = "ABC", CFICode = "XYZ", Venue = "Test", AlgoParams = "PriceMultiplier:100|;" } });
            repoMock.Setup(r => r.WriteDataAsync(It.IsAny<IEnumerable<ParsedRecord>>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new DataExtractionService(new[] { parserMock.Object }, repoMock.Object);
            await service.ExecuteAsync("BankA", Path.GetTempFileName());

            repoMock.Verify(r => r.ReadDataAsync(It.IsAny<string>(), parserMock.Object), Times.Once);
            repoMock.Verify(r => r.WriteDataAsync(It.IsAny<IEnumerable<ParsedRecord>>(), It.IsAny<string>()), Times.Once);
        }
    }
}
