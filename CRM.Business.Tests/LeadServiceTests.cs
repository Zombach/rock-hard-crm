using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class LeadServiceTests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private IAuthenticationService _authenticationService;
        private ILeadValidationHelper _leadValidationHelper;
        private IAuthOptions _authOptions;
        private LeadService _sut;


        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _leadValidationHelper = new LeadValidationHelper(_leadRepoMock.Object);
            _authenticationService = new AuthenticationService(_leadRepoMock.Object, _authOptions);
            _sut = new LeadService(_leadRepoMock.Object, _accountRepoMock.Object, _authenticationService, _leadValidationHelper);
        }

        [Test]
        public void AddLead()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            var input = LeadData.GetInputLeadDto();
            _leadRepoMock.Setup(x => x.AddLead(input)).Returns(expectedLeadDto.Id);
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.AddLead(input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.AddLead(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void UpdateLead()
        {
            //Given
            var callCount = 2;
            var input = LeadData.GetInputLeadDto();
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.UpdateLead(input));
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.UpdateLead(expectedLeadDto.Id, input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.UpdateLead(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Exactly(callCount));
        }

        [Test]
        public void GetAllLeads()
        {
            //Given
            var expectedList = LeadData.GetListLeadDto();
            _leadRepoMock.Setup(x => x.GetAllLeads()).Returns(expectedList);

            //When
            var actualList = _sut.GetAllLeads();

            //Then
            Assert.AreEqual(expectedList, actualList);
            _leadRepoMock.Verify(x => x.GetAllLeads(), Times.Once);
        }

        [Test]
        public void GetLeadById()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.GetLeadById(expectedLeadDto.Id);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void DeleteLeadById()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.DeleteLeadById(expectedLeadDto.Id));
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            _sut.DeleteLeadById(expectedLeadDto.Id);

            //Then
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
            _leadRepoMock.Verify(x => x.DeleteLeadById(expectedLeadDto.Id), Times.Once);
        }
    }
}