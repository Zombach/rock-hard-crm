using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Repositories;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class LeadServiceTests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<IPublishEndpoint> _publishEndpointMock;
        private IAuthenticationService _authenticationService;
        private ILeadValidationHelper _leadValidationHelper;
        private readonly IAuthOptions _authOptions;
        private LeadService _sut;

        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _leadValidationHelper = new LeadValidationHelper(_leadRepoMock.Object);
            _authenticationService = new AuthenticationService(_leadRepoMock.Object, _authOptions);
            _sut = new LeadService(
                _leadRepoMock.Object,
                _accountRepoMock.Object,
                _authenticationService,
                _leadValidationHelper,
                _publishEndpointMock.Object);
        }

        [Test]
        public void AddLead()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            var input = LeadData.GetInputLeadDto();
            _leadRepoMock.Setup(x => x.AddLeadAsync(input)).ReturnsAsync(expectedLeadDto.Id);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(expectedLeadDto.Id)).ReturnsAsync(expectedLeadDto);

            //When
            var actualLeadDto = _sut.AddLeadAsync(input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.AddLeadAsync(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void UpdateLead()
        {
            //Given
            var callCount = 2;
            var input = LeadData.GetInputLeadDto();
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.UpdateLeadAsync(input));
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(expectedLeadDto.Id)).ReturnsAsync(expectedLeadDto);

            //When
            var actualLeadDto = _sut.UpdateLeadAsync(expectedLeadDto.Id, input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.UpdateLeadAsync(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(expectedLeadDto.Id), Times.Exactly(callCount));
        }

        [Test]
        public void GetAllLeads()
        {
            //Given
            var expectedList = LeadData.GetListLeadDto();
            _leadRepoMock.Setup(x => x.GetAllLeadsAsync()).ReturnsAsync(expectedList);

            //When
            var actualList = _sut.GetAllLeadsAsync();

            //Then
            Assert.AreEqual(expectedList, actualList);
            _leadRepoMock.Verify(x => x.GetAllLeadsAsync(), Times.Once);
        }

        [Test]
        public void GetLeadById()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            var leadRegular = LeadIdentityInfoData.GetRegularLeadIdentityInfo();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadRegular.LeadId)).ReturnsAsync(expectedLeadDto);

            //When
            var actualLeadDto = _sut.GetLeadByIdAsync(leadRegular.LeadId, leadRegular);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(expectedLeadDto.Id), Times.Once);
        }
    }
}