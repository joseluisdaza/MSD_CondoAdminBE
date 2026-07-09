using NUnit.Framework;
using Condominio.Reports;
using Condominio.Reports.Models;
using Condominio.DTOs;
using System.Collections.Generic;

namespace Condominio.Tests.Reports
{
    [TestFixture]
    public class ReportModelsTests
    {
        #region ReportExecutionData Tests

        [Test]
        public void ReportExecutionData_HasDefaultTitle()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.IsNull(reportData.Title);
        }

        [Test]
        public void ReportExecutionData_HasDefaultTitleStyleId()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.AreEqual(0, reportData.TitleStyleId);
        }

        [Test]
        public void ReportExecutionData_HasDefaultAvailableStyles()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.IsNotNull(reportData.AvailableStyles);
            Assert.IsEmpty(reportData.AvailableStyles);
        }

        [Test]
        public void ReportExecutionData_HasDefaultHeaderParts()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.IsNotNull(reportData.HeaderParts);
            Assert.IsEmpty(reportData.HeaderParts);
        }

        [Test]
        public void ReportExecutionData_HasDefaultSectionParts()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.IsNotNull(reportData.SectionParts);
            Assert.IsEmpty(reportData.SectionParts);
        }

        [Test]
        public void ReportExecutionData_HasDefaultFooterParts()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData();

            // Assert
            Assert.IsNotNull(reportData.FooterParts);
            Assert.IsEmpty(reportData.FooterParts);
        }

        [Test]
        public void ReportExecutionData_CanSetTitle()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var expectedTitle = "Test Report";

            // Act
            reportData.Title = expectedTitle;

            // Assert
            Assert.AreEqual(expectedTitle, reportData.Title);
        }

        [Test]
        public void ReportExecutionData_CanSetTitleStyleId()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var expectedStyleId = 5;

            // Act
            reportData.TitleStyleId = expectedStyleId;

            // Assert
            Assert.AreEqual(expectedStyleId, reportData.TitleStyleId);
        }

        [Test]
        public void ReportExecutionData_CanSetAvailableStyles()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var styles = new List<ReportStyleData> 
            { 
                new ReportStyleData { Id = 1, StyleName = "Style1" }
            };

            // Act
            reportData.AvailableStyles = styles;

            // Assert
            Assert.AreEqual(styles, reportData.AvailableStyles);
        }

        [Test]
        public void ReportExecutionData_CanSetHeaderParts()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var headerParts = new List<ReportPartData> 
            { 
                new ReportPartData { Content = "Header" }
            };

            // Act
            reportData.HeaderParts = headerParts;

            // Assert
            Assert.AreEqual(headerParts, reportData.HeaderParts);
        }

        [Test]
        public void ReportExecutionData_CanSetSectionParts()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var sectionParts = new List<ReportPartData> 
            { 
                new ReportPartData { Content = "Section" }
            };

            // Act
            reportData.SectionParts = sectionParts;

            // Assert
            Assert.AreEqual(sectionParts, reportData.SectionParts);
        }

        [Test]
        public void ReportExecutionData_CanSetFooterParts()
        {
            // Arrange
            var reportData = new ReportExecutionData();
            var footerParts = new List<ReportPartData> 
            { 
                new ReportPartData { Content = "Footer" }
            };

            // Act
            reportData.FooterParts = footerParts;

            // Assert
            Assert.AreEqual(footerParts, reportData.FooterParts);
        }

        #endregion

        #region ReportPartData Tests

        [Test]
        public void ReportPartData_HasDefaultContent()
        {
            // Arrange & Act
            var reportPart = new ReportPartData();

            // Assert
            Assert.IsNull(reportPart.Content);
        }

        [Test]
        public void ReportPartData_HasDefaultStyleId()
        {
            // Arrange & Act
            var reportPart = new ReportPartData();

            // Assert
            Assert.AreEqual(0, reportPart.StyleId);
        }

        [Test]
        public void ReportPartData_HasDefaultIsTable()
        {
            // Arrange & Act
            var reportPart = new ReportPartData();

            // Assert
            Assert.IsFalse(reportPart.IsTable);
        }

        [Test]
        public void ReportPartData_HasDefaultDisplayOrder()
        {
            // Arrange & Act
            var reportPart = new ReportPartData();

            // Assert
            Assert.AreEqual(0, reportPart.DisplayOrder);
        }

        [Test]
        public void ReportPartData_CanSetContent()
        {
            // Arrange
            var reportPart = new ReportPartData();
            var expectedContent = "Test Content";

            // Act
            reportPart.Content = expectedContent;

            // Assert
            Assert.AreEqual(expectedContent, reportPart.Content);
        }

        [Test]
        public void ReportPartData_CanSetStyleId()
        {
            // Arrange
            var reportPart = new ReportPartData();
            var expectedStyleId = 3;

            // Act
            reportPart.StyleId = expectedStyleId;

            // Assert
            Assert.AreEqual(expectedStyleId, reportPart.StyleId);
        }

        [Test]
        public void ReportPartData_CanSetIsTable()
        {
            // Arrange
            var reportPart = new ReportPartData();

            // Act
            reportPart.IsTable = true;

            // Assert
            Assert.IsTrue(reportPart.IsTable);
        }

        [Test]
        public void ReportPartData_CanSetDisplayOrder()
        {
            // Arrange
            var reportPart = new ReportPartData();
            var expectedDisplayOrder = 5;

            // Act
            reportPart.DisplayOrder = expectedDisplayOrder;

            // Assert
            Assert.AreEqual(expectedDisplayOrder, reportPart.DisplayOrder);
        }

        [Test]
        public void ReportPartData_CanSetContentToNull()
        {
            // Arrange
            var reportPart = new ReportPartData { Content = "Some Content" };

            // Act
            reportPart.Content = null;

            // Assert
            Assert.IsNull(reportPart.Content);
        }

        [Test]
        public void ReportPartData_CanSetContentToObject()
        {
            // Arrange
            var reportPart = new ReportPartData();
            var expectedContent = new { Name = "Test" };

            // Act
            reportPart.Content = expectedContent;

            // Assert
            Assert.AreEqual(expectedContent, reportPart.Content);
        }

        #endregion

        #region ReportStyleData Tests

        [Test]
        public void ReportStyleData_HasDefaultId()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.AreEqual(0, reportStyle.Id);
        }

        [Test]
        public void ReportStyleData_HasDefaultStyleName()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsNull(reportStyle.StyleName);
        }

        [Test]
        public void ReportStyleData_HasDefaultBold()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsFalse(reportStyle.Bold);
        }

        [Test]
        public void ReportStyleData_HasDefaultItalic()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsFalse(reportStyle.Italic);
        }

        [Test]
        public void ReportStyleData_HasDefaultUnderline()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsFalse(reportStyle.Underline);
        }

        [Test]
        public void ReportStyleData_HasDefaultFontSize()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.AreEqual(0, reportStyle.FontSize);
        }

        [Test]
        public void ReportStyleData_HasDefaultFontColor()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsNull(reportStyle.FontColor);
        }

        [Test]
        public void ReportStyleData_HasDefaultBackgroundColor()
        {
            // Arrange & Act
            var reportStyle = new ReportStyleData();

            // Assert
            Assert.IsNull(reportStyle.BackgroundColor);
        }

        [Test]
        public void ReportStyleData_CanSetId()
        {
            // Arrange
            var reportStyle = new ReportStyleData();
            var expectedId = 10;

            // Act
            reportStyle.Id = expectedId;

            // Assert
            Assert.AreEqual(expectedId, reportStyle.Id);
        }

        [Test]
        public void ReportStyleData_CanSetStyleName()
        {
            // Arrange
            var reportStyle = new ReportStyleData();
            var expectedName = "Title Style";

            // Act
            reportStyle.StyleName = expectedName;

            // Assert
            Assert.AreEqual(expectedName, reportStyle.StyleName);
        }

        [Test]
        public void ReportStyleData_CanSetBold()
        {
            // Arrange
            var reportStyle = new ReportStyleData();

            // Act
            reportStyle.Bold = true;

            // Assert
            Assert.IsTrue(reportStyle.Bold);
        }

        [Test]
        public void ReportStyleData_CanSetItalic()
        {
            // Arrange
            var reportStyle = new ReportStyleData();

            // Act
            reportStyle.Italic = true;

            // Assert
            Assert.IsTrue(reportStyle.Italic);
        }

        [Test]
        public void ReportStyleData_CanSetUnderline()
        {
            // Arrange
            var reportStyle = new ReportStyleData();

            // Act
            reportStyle.Underline = true;

            // Assert
            Assert.IsTrue(reportStyle.Underline);
        }

        [Test]
        public void ReportStyleData_CanSetFontSize()
        {
            // Arrange
            var reportStyle = new ReportStyleData();
            var expectedFontSize = 14;

            // Act
            reportStyle.FontSize = expectedFontSize;

            // Assert
            Assert.AreEqual(expectedFontSize, reportStyle.FontSize);
        }

        [Test]
        public void ReportStyleData_CanSetFontColor()
        {
            // Arrange
            var reportStyle = new ReportStyleData();
            var expectedFontColor = "#000000";

            // Act
            reportStyle.FontColor = expectedFontColor;

            // Assert
            Assert.AreEqual(expectedFontColor, reportStyle.FontColor);
        }

        [Test]
        public void ReportStyleData_CanSetBackgroundColor()
        {
            // Arrange
            var reportStyle = new ReportStyleData();
            var expectedBackgroundColor = "#FFFFFF";

            // Act
            reportStyle.BackgroundColor = expectedBackgroundColor;

            // Assert
            Assert.AreEqual(expectedBackgroundColor, reportStyle.BackgroundColor);
        }

        [Test]
        public void ReportStyleData_CanSetMultipleProperties()
        {
            // Arrange
            var reportStyle = new ReportStyleData();

            // Act
            reportStyle.Id = 1;
            reportStyle.StyleName = "Header";
            reportStyle.Bold = true;
            reportStyle.FontSize = 16;
            reportStyle.FontColor = "#333333";

            // Assert
            Assert.AreEqual(1, reportStyle.Id);
            Assert.AreEqual("Header", reportStyle.StyleName);
            Assert.IsTrue(reportStyle.Bold);
            Assert.AreEqual(16, reportStyle.FontSize);
            Assert.AreEqual("#333333", reportStyle.FontColor);
        }

        #endregion

        #region AbstractReportOutput Tests

        [Test]
        public void AbstractReportOutput_HasSuccessProperty()
        {
            // Arrange
            var jsonOutput = new JsonReportOutput();

            // Act & Assert
            Assert.That(jsonOutput, Has.Property("Success"));
        }

        [Test]
        public void AbstractReportOutput_CanSetSuccess()
        {
            // Arrange
            var jsonOutput = new JsonReportOutput();

            // Act
            jsonOutput.Success = true;

            // Assert
            Assert.IsTrue(jsonOutput.Success);
        }

        [Test]
        public void AbstractReportOutput_SuccessDefaultValue()
        {
            // Arrange & Act
            var jsonOutput = new JsonReportOutput();

            // Assert
            Assert.IsFalse(jsonOutput.Success);
        }

        #endregion

        #region JsonReportOutput Tests

        [Test]
        public void JsonReportOutput_HasContentProperty()
        {
            // Arrange & Act
            var jsonOutput = new JsonReportOutput();

            // Assert
            Assert.That(jsonOutput, Has.Property("Content"));
        }

        [Test]
        public void JsonReportOutput_CanSetContent()
        {
            // Arrange
            var jsonOutput = new JsonReportOutput();
            var reportResponse = new ReportExecutionResponse { Title = "Test" };

            // Act
            jsonOutput.Content = reportResponse;

            // Assert
            Assert.AreEqual(reportResponse, jsonOutput.Content);
        }

        [Test]
        public void JsonReportOutput_InheritsFromAbstractReportOutput()
        {
            // Arrange & Act
            var jsonOutput = new JsonReportOutput();

            // Assert
            Assert.IsInstanceOf<AbstractReportOutput>(jsonOutput);
        }

        #endregion

        #region FileReportOutput Tests

        [Test]
        public void FileReportOutput_HasFilePathProperty()
        {
            // Arrange & Act
            var fileOutput = new FileReportOutput();

            // Assert
            Assert.That(fileOutput, Has.Property("FilePath"));
        }

        [Test]
        public void FileReportOutput_HasFileNameProperty()
        {
            // Arrange & Act
            var fileOutput = new FileReportOutput();

            // Assert
            Assert.That(fileOutput, Has.Property("FileName"));
        }

        [Test]
        public void FileReportOutput_CanSetFilePath()
        {
            // Arrange
            var fileOutput = new FileReportOutput();
            var expectedPath = "/reports/report123.pdf";

            // Act
            fileOutput.FilePath = expectedPath;

            // Assert
            Assert.AreEqual(expectedPath, fileOutput.FilePath);
        }

        [Test]
        public void FileReportOutput_CanSetFileName()
        {
            // Arrange
            var fileOutput = new FileReportOutput();
            var expectedFileName = "report123.pdf";

            // Act
            fileOutput.FileName = expectedFileName;

            // Assert
            Assert.AreEqual(expectedFileName, fileOutput.FileName);
        }

        [Test]
        public void FileReportOutput_InheritsFromAbstractReportOutput()
        {
            // Arrange & Act
            var fileOutput = new FileReportOutput();

            // Assert
            Assert.IsInstanceOf<AbstractReportOutput>(fileOutput);
        }

        [Test]
        public void FileReportOutput_CanSetSuccessAndFilePath()
        {
            // Arrange
            var fileOutput = new FileReportOutput();

            // Act
            fileOutput.Success = true;
            fileOutput.FilePath = "/path/to/report.pdf";
            fileOutput.FileName = "report.pdf";

            // Assert
            Assert.IsTrue(fileOutput.Success);
            Assert.AreEqual("/path/to/report.pdf", fileOutput.FilePath);
            Assert.AreEqual("report.pdf", fileOutput.FileName);
        }

        #endregion

        #region Complex Model Tests

        [Test]
        public void ReportExecutionData_FullInitialization()
        {
            // Arrange & Act
            var reportData = new ReportExecutionData
            {
                Title = "Monthly Report",
                TitleStyleId = 1,
                AvailableStyles = new List<ReportStyleData>
                {
                    new ReportStyleData { Id = 1, StyleName = "Title", Bold = true, FontSize = 18 },
                    new ReportStyleData { Id = 2, StyleName = "Body", Bold = false, FontSize = 12 }
                },
                HeaderParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Header", StyleId = 1, DisplayOrder = 0 }
                },
                SectionParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Content", StyleId = 2, DisplayOrder = 0 }
                },
                FooterParts = new List<ReportPartData>
                {
                    new ReportPartData { Content = "Footer", StyleId = 1, DisplayOrder = 0 }
                }
            };

            // Assert
            Assert.AreEqual("Monthly Report", reportData.Title);
            Assert.AreEqual(1, reportData.TitleStyleId);
            Assert.AreEqual(2, reportData.AvailableStyles.Count());
            Assert.AreEqual(1, reportData.HeaderParts.Count());
            Assert.AreEqual(1, reportData.SectionParts.Count());
            Assert.AreEqual(1, reportData.FooterParts.Count());
        }

        #endregion
    }
}
