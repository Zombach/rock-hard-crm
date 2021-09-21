using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class CityServiceTests
    {
        private Mock<ICityRepository> _mock;
        private CityService _sut;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<ICityRepository>();
            _sut = new CityService(_mock.Object);
        }

        [Test]
        public void GetAllCities_NoEntry_ListCityDto()
        {
            //Given
            var expectedCites = CityData.GetListCityDto();
            _mock.Setup(x => x.GetAllCitiesAsync()).ReturnsAsync(expectedCites);

            //When
            var actualCites = _sut.GetAllCities();

            //Then
            Assert.AreEqual(expectedCites, actualCites);
            _mock.Verify(x => x.GetAllCitiesAsync(), Times.Once);
        }
    }
}