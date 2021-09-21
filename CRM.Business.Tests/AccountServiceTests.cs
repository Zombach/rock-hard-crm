using System.Threading.Tasks;
using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.Models;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Repositories;
using MassTransit;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace CRM.Business.Tests
{
    public class AccountServiceTests
    {
        private IAccountValidationHelper _accountValidationHelper;
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<RestClient> _clientMock;
        private Mock<IPublishEndpoint> _publishEndPointMock;
        private AccountService _sut;

        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _clientMock = new Mock<RestClient>();
            _mapperMock = new Mock<IMapper>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _publishEndPointMock = new Mock<IPublishEndpoint>();
            _accountValidationHelper = new AccountValidationHelper(_accountRepoMock.Object);
            _sut = new AccountService
            (
                _accountRepoMock.Object,
                _leadRepoMock.Object,
                _mapperMock.Object,
                _accountValidationHelper,
                _publishEndPointMock.Object,
                _clientMock.Object);
        }

        [Test]
        public void AddAccount()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var expectedAccount = AccountData.GetAccountDto();
            var lead = LeadData.GetLead();

            _accountRepoMock.Setup(x => x.AddAccountAsync(expectedAccount)).ReturnsAsync(expectedAccount.Id);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);

            //When
            var actualId = _sut.AddAccountAsync(expectedAccount, leadInfo);

            //Then
            Assert.AreEqual(expectedAccount.Id, actualId);
            _accountRepoMock.Verify(x => x.AddAccountAsync(expectedAccount), Times.Once);
        }

        [Test]
        public async Task DeleteAccount()
        {
            //Given
            var account = AccountData.GetAccountDto();
            _accountRepoMock.Setup(x => x.DeleteAccountAsync(account.Id));
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(account);

            //When
            await _sut.DeleteAccountAsync(account.Id, account.LeadId);

            //Then
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccountAsync(account.Id), Times.Once);
        }

        [Test]
        public void DeleteAccount_AccountIdAndInvalidUserIdentityInfo_Exception()
        {
            //Given
            var account = AccountData.GetAccountDto();
            var accountAnother = AccountData.GetAnotherAccountDto();
            var expectedException = string.Format(ServiceMessages.LeadHasNoAccessMessageToAccount, account.LeadId);

            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(accountAnother);

            //When
            var ex = Assert.ThrowsAsync<AuthorizationException>(
                () => _sut.DeleteAccountAsync(account.Id, account.LeadId));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccountAsync(account.Id), Times.Never);
        }

        [Test]
        public async Task GetAccountWithTransactions()
        {
            var accountDto = AccountData.GetAccountDto();
            var expectedList = TransactionData.GetJSONstring();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();

            _accountRepoMock.Setup(x => x
                .GetAccountByIdAsync(accountDto.Id))
                .ReturnsAsync(accountDto);
            _mapperMock.Setup(x => x
                .Map<AccountBusinessModel>(accountDto))
                .Returns(accountBusinessModel);
            _clientMock.Setup(x => x
                .Execute<string>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<string>
                {
                    Data = expectedList
                });

            //When
            var actualList = await _sut.GetAccountWithTransactionsAsync(accountDto.Id, leadInfo);

            //Then
            Assert.AreEqual(accountBusinessModel, actualList);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(accountDto.Id), Times.Once);
        }
    }
}