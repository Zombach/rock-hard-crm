using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
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
        private AccountService _sut;
        private RequestHelper _requestHelper;

        [SetUp]
        public void SetUp()
        {
            _requestHelper = new RequestHelper();
            _leadRepoMock = new Mock<ILeadRepository>();
            _clientMock = new Mock<RestClient>();
            _mapperMock = new Mock<IMapper>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _accountValidationHelper = new AccountValidationHelper(_accountRepoMock.Object);
            _sut = new AccountService
            (
                _accountRepoMock.Object,
                _leadRepoMock.Object,
                _mapperMock.Object,
                _accountValidationHelper,
                _clientMock.Object,
                _requestHelper);
        }

        [Test]
        public void AddAccount()
        {
            //Given
            var lead = LeadData.GetLead();
            var expectedAccount = AccountData.GetAccountDto();
            _accountRepoMock.Setup(x => x.AddAccount(expectedAccount)).Returns(expectedAccount.Id);
            _leadRepoMock.Setup(x => x.GetLeadById(lead.Id)).Returns(lead);

            //When
            var actualId = _sut.AddAccount(expectedAccount, lead.Id);

            //Then
            Assert.AreEqual(expectedAccount.Id, actualId);
            _accountRepoMock.Verify(x => x.AddAccount(expectedAccount), Times.Once);
        }

        [Test]
        public void DeleteAccount()
        {
            //Given
            var account = AccountData.GetAccountDto();
            _accountRepoMock.Setup(x => x.DeleteAccount(account.Id));
            _accountRepoMock.Setup(x => x.GetAccountById(account.Id)).Returns(account);

            //When
            _sut.DeleteAccount(account.Id, account.LeadId);

            //Then
            _accountRepoMock.Verify(x => x.GetAccountById(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccount(account.Id), Times.Once);
        }

        [Test]
        public void DeleteAccount_AccountIdAndInvalidUserIdentityInfo_Exception()
        {
            //Given
            var account = AccountData.GetAccountDto();
            var accountAnother = AccountData.GetAnotherAccountDto();
            var expectedException = string.Format(ServiceMessages.LeadHasNoAccessMessage, account.LeadId);
            _accountRepoMock.Setup(x => x.GetAccountById(account.Id)).Returns(accountAnother);

            //When
            var ex = Assert.Throws<AuthorizationException>(
                () => _sut.DeleteAccount(account.Id, account.LeadId));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.GetAccountById(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccount(account.Id), Times.Never);
        }

        [Test]
        public void GetAccountWithTransactions()
        {
            var accountDto = AccountData.GetAccountDto();
            var expectedList = TransactionData.GetJSONstring();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();

            _accountRepoMock.Setup(x => x
                .GetAccountById(accountDto.Id))
                .Returns(accountDto);
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
            var actualList = _sut.GetAccountWithTransactions(accountDto.Id, accountDto.LeadId);

            //Then
            Assert.AreEqual(accountBusinessModel, actualList);
            _accountRepoMock.Verify(x => x.GetAccountById(accountDto.Id), Times.Once);
        }
    }
}