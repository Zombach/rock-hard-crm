﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using FluentAssertions;
using MassTransit;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace CRM.Business.Tests
{
    public class AccountServiceTests
    {
        private Mock<IEmailSenderService> _emailSenderServiceMock;
        private IAccountValidationHelper _accountValidationHelper;
        private ILeadValidationHelper _leadValidationHelper;
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IMapper> _mapperMock;
        private Mock<RestClient> _clientMock;
        private AccountService _sut;

        [SetUp]
        public void SetUp()
        {
            _emailSenderServiceMock = new Mock<IEmailSenderService>();
            _leadRepoMock = new Mock<ILeadRepository>();
            _clientMock = new Mock<RestClient>();
            _mapperMock = new Mock<IMapper>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _accountValidationHelper = new AccountValidationHelper(_accountRepoMock.Object);
            _leadValidationHelper = new LeadValidationHelper(_leadRepoMock.Object);
            _sut = new AccountService
            (
                _emailSenderServiceMock.Object,
                _accountRepoMock.Object,
                _mapperMock.Object,
                _accountValidationHelper,
                _clientMock.Object,
                _leadValidationHelper);
        }

        [Test]
        public void AddAccount_ValidAccountIdAndRegularIdentity_ReturnTaskInt()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var expectedAccount = new AccountDto { LeadId = leadInfo.LeadId, Currency = Currency.USD };
            var lead = LeadData.GetLeadDto();
            expectedAccount.LeadId = lead.Id;
            expectedAccount.Id = 1;

            _accountRepoMock.Setup(x => x.AddAccountAsync(It.Is(expectedAccount, new AccountDtoComparer()))).ReturnsAsync(expectedAccount.Id);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);

            //When
            var actualId = _sut.AddAccountAsync(expectedAccount.Currency, leadInfo);

            //Then
            Assert.AreEqual(expectedAccount.Id, actualId.Result);
            _accountRepoMock.Verify(x => x.AddAccountAsync(It.Is(expectedAccount, new AccountDtoComparer())), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
        }

        [Test]
        public void AddAccount_ValidAccountIdWithDuplicateCurrency_ReturnValidationException()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var expectedAccount = AccountData.GetRubAccountDto();
            var lead = LeadData.GetLeadDto();
            var expectedException = string.Format(ServiceMessages.LeadHasThisCurrencyMessage, lead.Id);

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);

            //When
            var ex = Assert.ThrowsAsync<ValidationException>(
                () => _sut.AddAccountAsync(expectedAccount.Currency, leadInfo));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.AddAccountAsync(expectedAccount), Times.Never);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
        }

        [Test]
        public void AddAccount_InvalidCurrencyWithoutVipStatus_ReturnAuthorizationException()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var expectedAccount = AccountData.GetEurAccountDto();
            var lead = LeadData.GetLeadDto();
            var expectedException = string.Format(ServiceMessages.LeadHasNoAccessMessageByRole, leadInfo.LeadId);

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);

            //When
            var ex = Assert.ThrowsAsync<AuthorizationException>(
                () => _sut.AddAccountAsync(expectedAccount.Currency, leadInfo));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.AddAccountAsync(expectedAccount), Times.Never);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
        }

        [Test]
        public async Task DeleteAccountAsync_ValidAccountIdAndLeadId_NoEntitiesReturns()
        {
            //Given
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();

            _accountRepoMock.Setup(x => x.DeleteAccountAsync(account.Id));
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(account);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            await _sut.DeleteAccountAsync(account.Id, lead.Id);

            //Then
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccountAsync(account.Id), Times.Once);
        }

        [Test]
        public void DeleteAccount_AccountIdAndInvalidUserIdentityInfo_Exception()
        {
            //Given
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var expectedException = string.Format(ServiceMessages.LeadHasNoAccessMessageToAccount, account.LeadId, accountAnother.Id);

            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(accountAnother);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            var ex = Assert.ThrowsAsync<AuthorizationException>(
                () => _sut.DeleteAccountAsync(account.Id, lead.Id));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccountAsync(account.Id), Times.Never);
        }

        [Test]
        public void DeleteAccount_AccountDoesNotHasInDataBase_ReturnEntityNotFoundException()
        {
            //Given
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();
            var expectedException = string.Format(ServiceMessages.EntityNotFoundMessage, nameof(account), account.Id);

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(lead.Id)).ReturnsAsync(lead);

            //When
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.DeleteAccountAsync(account.Id, lead.Id));

            //Then
            Assert.AreEqual(expectedException, ex.Message);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _accountRepoMock.Verify(x => x.DeleteAccountAsync(account.Id), Times.Never);
        }

        [Test]
        public async Task GetAccountWithTransactions()
        {
            //Given
            var accountDto = AccountData.GetUsdAccountDto();
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
                    Data = expectedList,
                });

            //When
            var actualList = await _sut.GetAccountWithTransactionsAsync(accountDto.Id, leadInfo);

            //Then
            Assert.AreEqual(accountBusinessModel, actualList);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(accountDto.Id), Times.Once);
        }

        [Test]
        public async Task RestoreAccountAsync_ValidAccountAndLead_ReturnCompletedTask()
        {
            //Given
            var leadDto = LeadData.GetLeadDto();
            var accountDto = AccountData.GetUsdAccountDto();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadDto.Id)).ReturnsAsync(leadDto);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(accountDto.Id)).ReturnsAsync(accountDto);
            _accountRepoMock.Setup(x => x.RestoreAccountAsync(accountDto.Id)).Returns(Task.CompletedTask);

            //When
            await _sut.RestoreAccountAsync(accountDto.Id, leadDto.Id);

            //Then
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(accountDto.Id), Times.Once);
            _accountRepoMock.Verify(x => x.RestoreAccountAsync(accountDto.Id), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadDto.Id), Times.Once);
        }

        [Test]
        public async Task GetTransactionsByPeriodAndPossiblyAccountIdAsync_NullContent_ReturnException()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var account = AccountData.GetUsdAccountDto();
            var excpected = "все упало";
            var model = new TimeBasedAcquisitionBusinessModel()
            {
                AccountId = account.Id,
                From = "01.09.2021 00:00",
                To = "02.09.2021 00:00"
            };

            account.LeadId = leadInfo.LeadId;

            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(account);
            _clientMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<string>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<string>, RestRequestAsyncHandle>>((request, callback) =>
                {
                    callback(new RestResponse<string>
                    {
                        Data = "",
                        StatusCode = HttpStatusCode.OK
                    }, null);
                });

            //When
            var ex = Assert.ThrowsAsync<Exception>(
                async () => await _sut.GetTransactionsByPeriodAndPossiblyAccountIdAsync(model, leadInfo));

            //Then
            Assert.AreEqual(excpected, ex.Message);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
        }

        [Test]
        public async Task GetTransactionsByPeriodAndPossiblyAccountIdAsync()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var account = AccountData.GetUsdAccountDto();
            var listAccountDtos = AccountData.GetListAccountDtos();
            var listAccountBusinessModels = AccountData.GetListAccountBusinessModels();
            var data = AccountData.GetData();
            var count = 0;
            var ids = new List<int> { 2, 652731, 1958795 };
            var excpected = AccountData.GetSthngStrange();
            var model = new TimeBasedAcquisitionBusinessModel()
            {
                AccountId = account.Id,
                From = "01.09.2021 00:00",
                To = "02.09.2021 00:00"
            };

            account.LeadId = leadInfo.LeadId;

            _mapperMock.Setup(x => x.Map<List<AccountBusinessModel>>(listAccountDtos)).Returns(listAccountBusinessModels);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(account);
            _accountRepoMock.Setup(x => x.GetAccountsByListIdAsync(ids)).ReturnsAsync(listAccountDtos);
            _clientMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<string>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<string>, RestRequestAsyncHandle>>((request, callback) =>
                {
                    callback(new RestResponse<string>
                    {
                        Data = count++ < 1 ? data : "[]",
                        StatusCode = HttpStatusCode.OK
                    }, null);
                });

            //When
            var actual = await _sut.GetTransactionsByPeriodAndPossiblyAccountIdAsync(model, leadInfo);

            //Then
            actual.Should().BeEquivalentTo(excpected, options => options
                .Excluding(obj => obj.CreatedOn));
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
        }

        [Test]
        public async Task GetTransactionsByPeriodAndPossiblyAccountIdAsync_NotAdminTryGetTransactions_ReturnException()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var excpected = $"{ServiceMessages.NoAdminRights}";
            var model = new TimeBasedAcquisitionBusinessModel()
            {
                AccountId = null,
                From = "01.09.2021 00:00",
                To = "02.09.2021 00:00"
            };

            _clientMock
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<string>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<string>, RestRequestAsyncHandle>>((request, callback) =>
                {
                    callback(new RestResponse<string>
                    {
                        Data = "",
                        StatusCode = HttpStatusCode.OK
                    }, null);
                });

            //When
            var ex = Assert.ThrowsAsync<AuthorizationException>(
                async () => await _sut.GetTransactionsByPeriodAndPossiblyAccountIdAsync(model, leadInfo));

            //Then
            Assert.AreEqual(excpected, ex.Message);
        }

        [Test]
        public async Task GetLeadBalanceAsync()
        {
            //Given
            var leadDto = LeadData.GetLeadDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var model = new RatesExchangeBusinessModel()
            {
                Updated = "account.Id,",
                BaseCurrency = "RUB",
                Rates = new Dictionary<string, decimal>()
            };

            var accountDto = AccountData.GetUsdAccountDto();
            var expectedList = TransactionData.GetJSONstring();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            var excpected = accountBusinessModel.Balance + 4; // 4 converted coins on all accounts

            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(accountDto.Id)).ReturnsAsync(accountDto);
            _mapperMock.Setup(x => x.Map<AccountBusinessModel>(accountDto)).Returns(accountBusinessModel);
            _clientMock.Setup(x => x.Execute<string>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<string>
                {
                    Data = expectedList
                });


            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(leadDto);
            _clientMock.Setup(x => x.Execute<RatesExchangeBusinessModel>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<RatesExchangeBusinessModel>
                {
                    Data = model,
                });

            //When
            var actual = await _sut.GetLeadBalanceAsync(leadInfo.LeadId, leadInfo);

            //Then
            Assert.AreEqual(excpected, actual);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);


        }
    }
}