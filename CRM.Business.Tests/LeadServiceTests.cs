using System.Threading.Tasks;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
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
        private readonly IAuthOptions _options;
        private LeadService _sut;

        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _leadValidationHelper = new LeadValidationHelper(_leadRepoMock.Object);
            _authenticationService = new AuthenticationService(_leadRepoMock.Object, _options);
            _sut = new LeadService(
                _leadRepoMock.Object,
                _accountRepoMock.Object,
                _authenticationService,
                _leadValidationHelper,
                _publishEndpointMock.Object);
        }

        [Test]
        public async Task AddLead()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var input = LeadData.GetInputLeadDto();
            var account = AccountData.GetRubAccountDto();
            account.LeadId = lead.Id;

            _leadRepoMock.Setup(x => x.AddLeadAsync(input)).ReturnsAsync(lead.Id);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            var actualLeadDto = await _sut.AddLeadAsync(input);

            //Then
            Assert.AreEqual(lead, actualLeadDto);
            _leadRepoMock.Verify(x => x.AddLeadAsync(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Once);
        }

        [Test]
        public async Task UpdateLead()
        {
            //Given
            const int callCount = 2;
            var input = LeadData.GetInputLeadDto();
            var expectedLeadDto = LeadData.GetLeadDto();

            _leadRepoMock.Setup(x => x.UpdateLeadAsync(input));
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(expectedLeadDto.Id)).ReturnsAsync(expectedLeadDto);

            //When
            var actualLeadDto = await _sut.UpdateLeadAsync(expectedLeadDto.Id, input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.UpdateLeadAsync(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(expectedLeadDto.Id), Times.Exactly(callCount));
        }

        [Test]
        public async Task UpdateLeadRoleAsync()
        {
            //Given
            const int callCount = 2;
            var lead = LeadData.GetLeadDto();
            lead.Role = Role.Vip;

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);
            _leadRepoMock.Setup(x => x.UpdateLeadRoleAsync(lead));

            //When
            var actualLeadDto = await _sut.UpdateLeadRoleAsync(lead.Id, Role.Vip);

            //Then
            Assert.AreEqual(lead, actualLeadDto);
            _leadRepoMock.Verify(x => x.UpdateLeadRoleAsync(lead), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Exactly(callCount));
        }

        [Test]
        public async Task GetAllLeads()
        {
            //Given
            var expectedList = LeadData.GetListLeadDto();
            _leadRepoMock.Setup(x => x.GetAllLeadsAsync()).ReturnsAsync(expectedList);

            //When
            var actualList = await _sut.GetAllLeadsAsync();

            //Then
            Assert.AreEqual(expectedList, actualList);
            _leadRepoMock.Verify(x => x.GetAllLeadsAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteLeadAsync()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);
            _leadRepoMock.Setup(x => x.DeleteLeadAsync(lead.Id));

            //When
            await _sut.DeleteLeadAsync(lead.Id);

            //Then
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Once);
            _leadRepoMock.Verify(x => x.DeleteLeadAsync(lead.Id), Times.Once);
        }

        [Test]
        public async Task GetLeadByIdAsync()
        {
            //Given
            var expectedLeadDto = LeadData.GetLeadDto();
            var leadRegular = LeadIdentityInfoData.GetRegularLeadIdentityInfo();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadRegular.LeadId)).ReturnsAsync(expectedLeadDto);

            //When
            var actualLeadDto = await _sut.GetLeadByIdAsync(leadRegular.LeadId, leadRegular);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public async Task GetLeadByIdAsync_DoesNotHaveLeadInDataBase_ReturnEntityNotFoundException()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var leadRegular = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var expected = string.Format(ServiceMessages.EntityNotFoundMessage, nameof(lead), lead.Id);

            //When
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.GetLeadByIdAsync(lead.Id, leadRegular));

            //Then
            Assert.AreEqual(expected, ex.Message);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Once);
        }

        [Test]
        public async Task GetLeadByIdAsync_LeadDoesNotHaveAccessToCurrentLead_ReturnEntityNotFoundException()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var leadRegular = LeadIdentityInfoData.GetRegularAnotherLeadIdentityInfo();
            var expected = string.Format(ServiceMessages.LeadHasNoAccessMessageToLead, leadRegular.LeadId);

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.GetLeadByIdAsync(lead.Id, leadRegular));

            //Then
            Assert.AreEqual(expected, ex.Message);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Once);
        }

        [Test]
        public async Task GetLeadByIdAsync_AdminLead_ReturnLeadDto()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var leadAdmin = LeadIdentityInfoData.GetAdminLeadIdentityInfo();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            var actual = await _sut.GetLeadByIdAsync(lead.Id, leadAdmin);

            //Then
            Assert.AreEqual(lead, actual);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(lead.Id), Times.Once);
        }
    }
}