using Moq;
using PartsTrader.ClientTools.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PartsTrader.ClientTools.Api.Data;
using PartsTrader.ClientTools.Api;

namespace PartsTrader.ClientTools.Tests
{
    /// <summary>
    /// Tests for <see cref="PartCatalogue" />.
    /// </summary>
    [TestClass]
    public class PartCatalogueTests
    {
        //TODO: include your unit tests here
        Mock<IPartsTraderPartsService> _partsTraderPartsServiceMock;
        PartCatalogue _partsCatalogue;

        [TestInitialize]
        public void Inializite()
        {
            _partsTraderPartsServiceMock = new Mock<IPartsTraderPartsService>();
            _partsCatalogue = new PartCatalogue(_partsTraderPartsServiceMock.Object);

            _partsTraderPartsServiceMock.Setup(x => x.FindAllCompatibleParts(It.IsAny<string>())).Returns(new List<PartSummary>
            { new PartSummary { PartNumber = "1234-abcd", Description = "Part a"},
              new PartSummary { PartNumber = "1111-abcd", Description = "Part b"}
            });
        }

        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_PartsFound()
        {
            //Arrange
            string partNumber = "1234-Test";

            //Act
            List<PartSummary> result = (List<PartSummary>)_partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsTrue(result.Count > 0);
            _partsTraderPartsServiceMock.Verify(x => x.FindAllCompatibleParts(partNumber), Times.Once);
        }

        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_PartsNotFound()
        {
            //Arrange
            string partNumber = "1234-Test";
            _partsTraderPartsServiceMock.Setup(x => x.FindAllCompatibleParts(partNumber)).Returns(new List<PartSummary>());
            //Act
            List<PartSummary> result = (List<PartSummary>)_partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsTrue(result.Count == 0);
            _partsTraderPartsServiceMock.Verify(x => x.FindAllCompatibleParts(partNumber), Times.Once);
        }

        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_ValidatePartNumber_Success()
        {
            //Arrange
            string partNumber = "1234-aBc1AbC";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPartException))]
        public void PartsCatalogue_GetCompatibleParts_ValidatePartNumber_FailedWrongFormat_1()
        {
            //Arrange
            string partNumber = "a234-abcd";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPartException))]
        public void PartsCatalogue_GetCompatibleParts_ValidatePartNumber_FailedWrongFormat_2()
        {
            //Arrange
            string partNumber = "123-abcd";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPartException))]
        public void PartsCatalogue_GetCompatibleParts_ValidatePartNumber_FailedWrongFormat_3()
        {
            //Arrange
            string partNumber = "123-abcd-123";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);
        }

        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_CheckExclusions_NotExclusionFound()
        {
            //Arrange
            string partNumber = "1234-NoExclusion";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_CheckExclusions_ExclusionFound()
        {
            //Arrange
            string partNumber = "1111-Invoice";

            //Act
            List<PartSummary> result = (List<PartSummary>)_partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.AreEqual(result.Count, 0);
            _partsTraderPartsServiceMock.Verify(x => x.FindAllCompatibleParts(It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public void PartsCatalogue_GetCompatibleParts_CheckExclusions_ExclusionFound_CheckInsensitiveCase()
        {
            //Arrange
            string partNumber = "1111-INVOICE";

            //Act
            List<PartSummary> result = (List<PartSummary>)_partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.AreEqual(result.Count, 0);
            _partsTraderPartsServiceMock.Verify(x => x.FindAllCompatibleParts(It.IsAny<string>()), Times.Never);
        }
    }
}