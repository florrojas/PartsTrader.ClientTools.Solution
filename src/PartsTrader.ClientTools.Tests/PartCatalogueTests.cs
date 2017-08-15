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
        public void PartsCatalogue_ValidatePartNumber_Succeds()
        {
            //Arrange
            string partNumber = "1234-abecA";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPartException))]
        public void PartsCatalogue_ValidatePartNumber_Failed()
        {
            //Arrange
            string partNumber = "12ae-abec3";

            //Act
            var result = _partsCatalogue.GetCompatibleParts(partNumber);

            //Assert
            Assert.IsNotNull(result);
        }


    }
}