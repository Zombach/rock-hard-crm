using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace CRM.Business.Tests
{
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<RestClient> _clientMock;
        private RequestHelper _requestHelper;
        private AccountService _sut;

        [SetUp]
        public void SetUp()
        {
            _clientMock = new Mock<RestClient>();
            _mapperMock = new Mock<IMapper>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _requestHelper = new RequestHelper();
            _sut = new AccountService(_accountRepoMock.Object, _clientMock.Object, _mapperMock.Object, _requestHelper);
        }

        [Test]
        public void AddAccount()
        {
            //Given
            var expectedAccount = AccountData.GetAccountDto();
            var userInfo =
                UserIdentityInfoData.GetUserIdentityInfo(expectedAccount.LeadId, new List<Role> { Role.Regular });
            _accountRepoMock.Setup(x => x.AddAccount(expectedAccount)).Returns(expectedAccount.Id);

            //When
            var actualId = _sut.AddAccount(expectedAccount, userInfo);

            //Then
            Assert.AreEqual(expectedAccount.Id, actualId);
            _accountRepoMock.Verify(x => x.AddAccount(expectedAccount), Times.Once);
        }

        [Test]
        public void DeleteAccount()
        {
            //Given
            var account = AccountData.GetAccountDto();
            var userInfo = UserIdentityInfoData.GetUserIdentityInfo(account.Id, new List<Role> { Role.Regular });
            _accountRepoMock.Setup(x => x.DeleteAccount(account.Id));
            _accountRepoMock.Setup(x => x.GetAccountById(account.Id)).Returns(account);

            //When
            _sut.DeleteAccount(account.Id, userInfo);

            //Then
            _accountRepoMock.Verify(x => x.DeleteAccount(account.Id), Times.Once);
        }

        [Test]
        public void DeleteAccount_AccountIdAndInvalidUserIdentityInfo_Exception()
        {
            //Given
            var account = AccountData.GetAccountDto();
            var accountAnother = AccountData.GetAnotherAccountDto();
            var userInfo = UserIdentityInfoData.GetUserIdentityInfo(account.Id, new List<Role> { Role.Regular });
            _accountRepoMock.Setup(x => x.GetAccountById(account.Id)).Returns(accountAnother);
            var expectedException = "It is not your account";

            //When
            var ex = Assert.Throws<Exception>(
                () => _sut.DeleteAccount(account.Id, userInfo));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.DeleteAccount(account.Id), Times.Never);
        }

        [Test]
        public void GetAccountWithTransactions()
        {
            var accountDto = AccountData.GetAccountDto();
            var expectedList = TransactionBusinessModelData.GetListTransactionBusinessModel();
            var accountBusinessModel = AccountBusinessModelData.GetAccountBusinessModel();
            var userInfo = UserIdentityInfoData.GetUserIdentityInfo(accountDto.LeadId, new List<Role> { Role.Regular });

            _accountRepoMock.Setup(x => x
                .GetAccountById(accountDto.Id))
                .Returns(accountDto);
            _mapperMock.Setup(x => x
                .Map<AccountBusinessModel>(accountDto))
                .Returns(accountBusinessModel);
            _clientMock.Setup(x => x
                .Execute<List<TransactionBusinessModel>>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<List<TransactionBusinessModel>>
                {
                    Data = expectedList,
                    StatusCode = HttpStatusCode.OK
                });

            //When
            var actualList = _sut.GetAccountWithTransactions(accountDto.Id, userInfo);

            //Then
            Assert.AreEqual(accountBusinessModel, actualList);
            _accountRepoMock.Verify(x => x.GetAccountById(accountDto.Id), Times.Once);
        }
    }
}