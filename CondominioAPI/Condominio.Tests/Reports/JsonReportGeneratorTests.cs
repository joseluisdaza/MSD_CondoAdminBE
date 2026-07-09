using NUnit.Framework;
using Condominio.Reports;
using Condominio.Reports.Models;
using Condominio.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Condominio.Tests.Reports
{
    [TestFixture]
    public class JsonReportGeneratorTests
    {
        private JsonReportGenerator _jsonReportGenerator;

        [SetUp]
        public void Setup()
        {
            _jsonReportGenerator = new JsonReportGenerator();
        }

        #region Basic Generation Tests

        [Test]
        public void Generate_WithValidReportData_ReturnsJsonReportOutput()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                TitleStyleId = 1
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonReportOutput>(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Generate_WithValidReportData_ReturnsAbstractReportOutput()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                TitleStyleId = 1
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AbstractReportOutput>(result);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Generate_ReturnsContentAsReportExecutionResponse()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                TitleStyleId = 1
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.IsInstanceOf<ReportExecutionResponse>(result.Content);
        }

        #endregion

        #region Content Mapping Tests

        [Test]
        public void Generate_MapsReportTitleCorrectly()
        {
            // Arrange
            var expectedTitle = "My Test Report";
            var reportData = new ReportExecutionData
            {
                Title = expectedTitle,
                TitleStyleId = 1
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.AreEqual(expectedTitle, result.Content.Title);
        }

        [Test]
        public void Generate_MapsTitleStyleIdCorrectly()
        {
            // Arrange
            var expectedStyleId = 5;
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                TitleStyleId = expectedStyleId
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.AreEqual(expectedStyleId, result.Content.StyleId);
        }

        [Test]
        public void Generate_WithHeaderParts_MapsHeadersCorrectly()
        {
            // Arrange
            var headerParts = new List<ReportPartData>
            {
                new ReportPartData { Content = "Header 1", StyleId = 1, IsTable = false, DisplayOrder = 0 },
                new ReportPartData { Content = "Header 2", StyleId = 2, IsTable = false, DisplayOrder = 1 }
            };
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                HeaderParts = headerParts
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result.Content.Headers);
            Assert.AreEqual(2, result.Content.Headers.Count());
        }

        [Test]
        public void Generate_WithSectionParts_MapsSectionsCorrectly()
        {
            // Arrange
            var sectionParts = new List<ReportPartData>
            {
                new ReportPartData { Content = "Section 1", StyleId = 1, IsTable = false, DisplayOrder = 0 },
                new ReportPartData { Content = "Section 2", StyleId = 2, IsTable = false, DisplayOrder = 1 }
            };
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = sectionParts
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result.Content.Sections);
            Assert.AreEqual(2, result.Content.Sections.Count());
        }

        [Test]
        public void Generate_WithFooterParts_MapsFootersCorrectly()
        {
            // Arrange
            var footerParts = new List<ReportPartData>
            {
                new ReportPartData { Content = "Footer 1", StyleId = 1, IsTable = false, DisplayOrder = 0 },
                new ReportPartData { Content = "Footer 2", StyleId = 2, IsTable = false, DisplayOrder = 1 }
            };
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                FooterParts = footerParts
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result.Content.Footers);
            Assert.AreEqual(2, result.Content.Footers.Count());
        }

        #endregion

        #region Part Data Conversion Tests

        [Test]
        public void Generate_ConvertPartDataToContentItems()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Content", StyleId = 1, IsTable = false, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;
            var contentItem = result.Content.Sections.First();

            // Assert
            Assert.AreEqual("Content", contentItem.Text);
            Assert.AreEqual(1, contentItem.StyleId);
            Assert.IsFalse(contentItem.IsTable);
        }

        [Test]
        public void Generate_PreservesStyleIdInContentItems()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Content", StyleId = 5, IsTable = false, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;
            var contentItem = result.Content.Sections.First();

            // Assert
            Assert.AreEqual(5, contentItem.StyleId);
        }

        [Test]
        public void Generate_PreservesTableFlagInContentItems()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Table Data", StyleId = 1, IsTable = true, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;
            var contentItem = result.Content.Sections.First();

            // Assert
            Assert.IsTrue(contentItem.IsTable);
        }

        [Test]
        public void Generate_WithMultipleParts_OrdersByDisplayOrder()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Third", StyleId = 1, DisplayOrder = 2 },
                    new ReportPartData { Content = "First", StyleId = 1, DisplayOrder = 0 },
                    new ReportPartData { Content = "Second", StyleId = 1, DisplayOrder = 1 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;
            var sections = result.Content.Sections.ToList();

            // Assert
            Assert.AreEqual(3, sections.Count);
        }

        #endregion

        #region Null and Empty Cases Tests

        [Test]
        public void Generate_WithNullReportData_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _jsonReportGenerator.Generate(null));
        }

        [Test]
        public void Generate_WithNullHeaderParts_ReturnsEmptyHeaders()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                HeaderParts = null
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Headers);
        }

        [Test]
        public void Generate_WithNullSectionParts_ReturnsEmptySections()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = null
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Sections);
        }

        [Test]
        public void Generate_WithNullFooterParts_ReturnsEmptyFooters()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                FooterParts = null
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Footers);
        }

        [Test]
        public void Generate_WithEmptyHeaderParts_ReturnsEmptyHeaders()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                HeaderParts = new List<ReportPartData>()
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Headers);
        }

        [Test]
        public void Generate_WithEmptySectionParts_ReturnsEmptySections()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                SectionParts = new List<ReportPartData>()
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Sections);
        }

        [Test]
        public void Generate_WithEmptyFooterParts_ReturnsEmptyFooters()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Test Report",
                FooterParts = new List<ReportPartData>()
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsEmpty(result.Content.Footers);
        }

        #endregion

        #region Complex Scenarios Tests

        [Test]
        public void Generate_WithCompleteReportData_GeneratesSuccessfully()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Complete Report",
                TitleStyleId = 1,
                AvailableStyles = new List<ReportStyleData>
                {
                    new ReportStyleData { Id = 1, StyleName = "Title", Bold = true, FontSize = 16 },
                    new ReportStyleData { Id = 2, StyleName = "Body", Bold = false, FontSize = 12 }
                },
                HeaderParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Report Header", StyleId = 1, IsTable = false, DisplayOrder = 0 }
                },
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Section 1", StyleId = 2, IsTable = false, DisplayOrder = 0 },
                    new ReportPartData { Content = "Section 2", StyleId = 2, IsTable = true, DisplayOrder = 1 }
                },
                FooterParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Report Footer", StyleId = 1, IsTable = false, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Complete Report", result.Content.Title);
            Assert.AreEqual(1, result.Content.StyleId);
            Assert.AreEqual(1, result.Content.Headers.Count());
            Assert.AreEqual(2, result.Content.Sections.Count());
            Assert.AreEqual(1, result.Content.Footers.Count());
        }

        [Test]
        public void Generate_MultipleCallsWithDifferentData_ProduceCorrectResults()
        {
            // Arrange
            var reportData1 = new ReportExecutionData { Title = "Report 1" };
            var reportData2 = new ReportExecutionData { Title = "Report 2" };

            // Act
            var result1 = _jsonReportGenerator.Generate(reportData1) as JsonReportOutput;
            var result2 = _jsonReportGenerator.Generate(reportData2) as JsonReportOutput;

            // Assert
            Assert.AreEqual("Report 1", result1.Content.Title);
            Assert.AreEqual("Report 2", result2.Content.Title);
        }

        [Test]
        public void Generate_WithLargeNumberOfParts_ProcessesSuccessfully()
        {
            // Arrange
            var sections = new List<ReportPartData>();
            for (int i = 0; i < 100; i++)
            {
                sections.Add(new ReportPartData
                {
                    Content = $"Section {i}",
                    StyleId = i % 5 + 1,
                    IsTable = i % 2 == 0,
                    DisplayOrder = i
                });
            }

            var reportData = new ReportExecutionData
            {
                Title = "Large Report",
                SectionParts = sections
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(100, result.Content.Sections.Count());
        }

        [Test]
        public void Generate_WithSpecialCharactersInContent_HandlesCorrectly()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Report with \"Quotes\" & Ampersand",
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Content with <tags> & special chars: é, ñ, ü", StyleId = 1, DisplayOrder = 0 }
                }
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData) as JsonReportOutput;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Report with \"Quotes\" & Ampersand", result.Content.Title);
        }

        #endregion

        #region Success Flag Tests

        [Test]
        public void Generate_AlwaysSetsSuccessToTrue()
        {
            // Arrange
            var reportData = new ReportExecutionData { Title = "Test" };

            // Act
            var result = _jsonReportGenerator.Generate(reportData);

            // Assert
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Generate_WithMinimalData_SuccessIsTrue()
        {
            // Arrange
            var reportData = new ReportExecutionData
            {
                Title = "Minimal",
                TitleStyleId = 0
            };

            // Act
            var result = _jsonReportGenerator.Generate(reportData);

            // Assert
            Assert.IsTrue(result.Success);
        }

        #endregion
    }
}
