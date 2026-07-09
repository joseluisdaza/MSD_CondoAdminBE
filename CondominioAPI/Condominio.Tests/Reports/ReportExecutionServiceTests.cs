using NUnit.Framework;
using Moq;
using Condominio.Reports;
using Condominio.Reports.Models;
using System.Collections.Generic;
using System.Linq;

namespace Condominio.Tests.Reports
{
    [TestFixture]
    public class ReportExecutionServiceTests
    {
        private ReportExecutionService _reportExecutionService;

        [SetUp]
        public void Setup()
        {
            _reportExecutionService = new ReportExecutionService();
        }

        #region Constructor and Initialization Tests

        [Test]
        public void Constructor_InitializesWithDefaultJsonGenerator()
        {
            // Arrange & Act
            var service = new ReportExecutionService();

            // Assert
            var supportedFormats = service.GetSupportedFormats();
            Assert.Contains("json", supportedFormats.ToList());
        }

        [Test]
        public void GetSupportedFormats_ReturnsJsonByDefault()
        {
            // Arrange & Act
            var formats = _reportExecutionService.GetSupportedFormats();

            // Assert
            Assert.IsNotNull(formats);
            Assert.Contains("json", formats.ToList());
        }

        #endregion

        #region RegisterGenerator Tests

        [Test]
        public void RegisterGenerator_WithValidFormatAndGenerator_RegistersSuccessfully()
        {
            // Arrange
            var format = "custom";
            var mockGenerator = new Mock<IReportGenerator>();

            // Act
            _reportExecutionService.RegisterGenerator(format, mockGenerator.Object);

            // Assert
            var supportedFormats = _reportExecutionService.GetSupportedFormats();
            Assert.Contains("custom", supportedFormats.ToList());
        }

        [Test]
        public void RegisterGenerator_WithCaseInsensitiveFormat_StoresLowercase()
        {
            // Arrange
            var format = "PDF";
            var mockGenerator = new Mock<IReportGenerator>();

            // Act
            _reportExecutionService.RegisterGenerator(format, mockGenerator.Object);

            // Assert
            var supportedFormats = _reportExecutionService.GetSupportedFormats();
            Assert.Contains("pdf", supportedFormats.ToList());
            Assert.IsFalse(supportedFormats.Contains("PDF"));
        }

        [Test]
        public void RegisterGenerator_WithNullFormat_ThrowsArgumentException()
        {
            // Arrange
            var mockGenerator = new Mock<IReportGenerator>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _reportExecutionService.RegisterGenerator(null, mockGenerator.Object));
        }

        [Test]
        public void RegisterGenerator_WithEmptyFormat_ThrowsArgumentException()
        {
            // Arrange
            var mockGenerator = new Mock<IReportGenerator>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _reportExecutionService.RegisterGenerator("", mockGenerator.Object));
        }

        [Test]
        public void RegisterGenerator_WithWhitespaceFormat_ThrowsArgumentException()
        {
            // Arrange
            var mockGenerator = new Mock<IReportGenerator>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _reportExecutionService.RegisterGenerator("   ", mockGenerator.Object));
        }

        [Test]
        public void RegisterGenerator_WithNullGenerator_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _reportExecutionService.RegisterGenerator("custom", null));
        }

        [Test]
        public void RegisterGenerator_OverridesExistingGenerator()
        {
            // Arrange
            var format = "json";
            var mockGenerator1 = new Mock<IReportGenerator>();
            var mockGenerator2 = new Mock<IReportGenerator>();
            var mockOutput = new Mock<AbstractReportOutput>();
            mockOutput.Object.Success = true;
            mockGenerator2.Setup(g => g.Generate(It.IsAny<ReportExecutionData>())).Returns(mockOutput.Object);

            // Act
            _reportExecutionService.RegisterGenerator(format, mockGenerator1.Object);
            _reportExecutionService.RegisterGenerator(format, mockGenerator2.Object);
            var reportData = new ReportExecutionData { Title = "Test Report" };
            var result = _reportExecutionService.ExecuteReport(reportData, format);

            // Assert - Verify the second generator was used
            Assert.IsNotNull(result);
            mockGenerator2.Verify(g => g.Generate(reportData), Times.Once);
        }

        #endregion

        #region ExecuteReport Tests

        [Test]
        public void ExecuteReport_WithValidData_ReturnsReportOutput()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                TitleStyleId = 1,
                HeaderParts = new List<ReportPartData>(),
                SectionParts = new List<ReportPartData>(),
                FooterParts = new List<ReportPartData>()
            };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, "json");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithNullReportData_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _reportExecutionService.ExecuteReport(null, "json"));
        }

        [Test]
        public void ExecuteReport_WithNullFormat_UsesDefaultJsonFormat()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithEmptyFormat_UsesDefaultJsonFormat()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, "");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithWhitespaceFormat_UsesDefaultJsonFormat()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, "   ");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithUnsupportedFormat_ThrowsInvalidOperationException()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };
            var unsupportedFormat = "unsupported";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _reportExecutionService.ExecuteReport(reportData, unsupportedFormat));

            Assert.That(exception.Message, Does.Contain("No generator found for format"));
            Assert.That(exception.Message, Does.Contain(unsupportedFormat));
        }

        [Test]
        public void ExecuteReport_WithCaseInsensitiveFormat_FindsGenerator()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, "JSON");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithComplexReportData_GeneratesSuccessfully()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Complex Report",
                TitleStyleId = 1,
                AvailableStyles = new List<ReportStyleData>
                {
                    new ReportStyleData { Id = 1, StyleName = "Title", Bold = true, FontSize = 16 },
                    new ReportStyleData { Id = 2, StyleName = "Body", Bold = false, FontSize = 12 }
                },
                HeaderParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Header", StyleId = 1, IsTable = false, DisplayOrder = 0 }
                },
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Section 1", StyleId = 2, IsTable = false, DisplayOrder = 0 },
                    new ReportPartData { Content = "Section 2", StyleId = 2, IsTable = false, DisplayOrder = 1 }
                },
                FooterParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Footer", StyleId = 1, IsTable = false, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData, "json");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_WithDefaultFormat_UsesJsonGenerator()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ExecuteReport_MultipleCallsWithDifferentFormats_ReturnCorrectGenerators()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test Report" };
            var mockCustomGenerator = new Mock<IReportGenerator>();
            var mockOutput = new Mock<AbstractReportOutput>();
            mockOutput.Object.Success = true;
            mockCustomGenerator.Setup(g => g.Generate(It.IsAny<ReportExecutionData>())).Returns(mockOutput.Object);

            // Act
            _reportExecutionService.RegisterGenerator("custom", mockCustomGenerator.Object);
            var jsonResult = _reportExecutionService.ExecuteReport(reportData, "json");
            var customResult = _reportExecutionService.ExecuteReport(reportData, "custom");

            // Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsNotNull(customResult);
            mockCustomGenerator.Verify(g => g.Generate(reportData), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Test]
        public void ExecuteReport_WithEmptyReportParts_ReturnsSuccessfully()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Empty Report",
                HeaderParts = new List<ReportPartData>(),
                SectionParts = new List<ReportPartData>(),
                FooterParts = new List<ReportPartData>()
            };

            // Act
            var result = _reportExecutionService.ExecuteReport(reportData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void GetSupportedFormats_ReturnsEnumerable()
        {
            // Act
            var formats = _reportExecutionService.GetSupportedFormats();

            // Assert
            Assert.IsNotNull(formats);
            Assert.IsInstanceOf<IEnumerable<string>>(formats);
        }

        [Test]
        public void GetSupportedFormats_CanBeCalledMultipleTimes()
        {
            // Act
            var formats1 = _reportExecutionService.GetSupportedFormats();
            var formats2 = _reportExecutionService.GetSupportedFormats();

            // Assert
            Assert.AreEqual(formats1.Count(), formats2.Count());
            Assert.That(formats1, Is.EquivalentTo(formats2));
        }

        #endregion
    }
}
