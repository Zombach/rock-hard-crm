using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace CRM.Business.Tests
{
    public class TransactionServiceTests
    {
        private Mock<RestClient> _clientMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<IAccountService> _accountServiceMock;
        private Mock<IEmailSenderService> _emailSenderServiceMock;
        private TransactionService _sut;
        private IAccountValidationHelper _accountValidationHelper;
        private ILeadValidationHelper _leadValidationHelper;
        private Mock<ICommissionFeeService> _commissionFeeServiceMock;
        private Mock<ILeadRepository> _leadRepoMock;

        [SetUp]
        public void SetUp()
        {
            _commissionFeeServiceMock = new Mock<ICommissionFeeService>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _accountServiceMock = new Mock<IAccountService>();
            _emailSenderServiceMock = new Mock<IEmailSenderService>();
            _leadRepoMock = new Mock<ILeadRepository>();
            _clientMock = new Mock<RestClient>();
            _accountValidationHelper = new AccountValidationHelper(_accountRepoMock.Object);
            _leadValidationHelper = new LeadValidationHelper(_leadRepoMock.Object);

            var optionsMock = new Mock<IOptions<CommissionSettings>>();
            optionsMock.Setup(x => x.Value).Returns(new CommissionSettings
            {
                Commission = 0.2m,
                CommissionModifier = 1.5m,
                VipCommission = 0.1m
            });
            var optionSettingsMock = new Mock<IOptions<ConnectionSettings>>();
            optionSettingsMock.Setup(x => x.Value).Returns(new ConnectionSettings()
            {
                TransactionStoreUrl = @"https://T"
            });

            _sut = new TransactionService(
                optionsMock.Object,
                optionSettingsMock.Object,
                _accountValidationHelper,
                _leadValidationHelper,
                _accountServiceMock.Object,
                _emailSenderServiceMock.Object,
                _commissionFeeServiceMock.Object,
                _leadRepoMock.Object);
        }

        [Test]
        public async Task AddDeposit()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            const long data = 21321L;
            var model = TransactionData.GeTransactionBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();
            var commission = new CommissionFeeDto
            {
                LeadId = leadInfo.LeadId,
                AccountId = model.AccountId,
                TransactionId = data,
                Role = leadInfo.Role,
                CommissionAmount = 0.2m,
                TransactionType = TransactionType.Deposit
            };
            account.LeadId = leadInfo.LeadId;

            _sut.SetClient(_clientMock.Object);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _commissionFeeServiceMock.Setup(x => x.AddCommissionFeeAsync(commission)).ReturnsAsync(commission.Id);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(account);
            _clientMock
                .Setup(x => x.Execute<long>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<long>
                {
                    Data = data
                });

            //When
            var actual = await _sut.AddDepositAsync(model, leadInfo);

            //Then
            actual.Should().BeEquivalentTo(commission);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
            _commissionFeeServiceMock.Verify(x => x.AddCommissionFeeAsync(commission), Times.Never);
            _clientMock.Verify(x=> x.Execute<long>(It.IsAny<IRestRequest>()),Times.Once);
        }

        [Test]
        public async Task AddWithdraw()
        {
            //Given
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            const long data = 21321L;
            var model = TransactionData.GeTransactionBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();
            var expectedList = TransactionData.GetJSONstring();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            var commission = new CommissionFeeDto
            {
                LeadId = leadInfo.LeadId,
                AccountId = model.AccountId,
                TransactionId = data,
                Role = leadInfo.Role,
                CommissionAmount = 0.2m,
                TransactionType = TransactionType.Withdraw
            };
            account.LeadId = leadInfo.LeadId;

            _sut.SetClient(_clientMock.Object);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(account.Id, leadInfo)).ReturnsAsync(accountBusinessModel);
            _commissionFeeServiceMock.Setup(x => x.AddCommissionFeeAsync(commission)).ReturnsAsync(commission.Id);
            _clientMock
                .Setup(x => x.Execute<string>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<string>
                {
                    Data = expectedList
                });
            _clientMock
                .Setup(x => x.Execute<long>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<long>
                {
                    Data = data
                });

            //When
            var actual = await _sut.AddWithdrawAsync(model, leadInfo);

            //Then
            actual.Should().BeEquivalentTo(commission);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
            _commissionFeeServiceMock.Verify(x => x.AddCommissionFeeAsync(commission), Times.Never);
        }

        [Test]
        public async Task AddTransferAsync()
        {
            //Given
            var lead = LeadData.GetLeadWithTwoAccountsDto();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            const long transactionId = 21321L;
            const decimal commissionAmount = 4.2m;
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var data = new List<long>
            {
                transactionId,
                213123121L
            };
            var commission = new CommissionFeeDto
            {
                LeadId = leadInfo.LeadId,
                AccountId = model.AccountId,
                TransactionId = transactionId,
                Role = leadInfo.Role,
                CommissionAmount = commissionAmount,
                TransactionType = TransactionType.Transfer
            };

            account.LeadId = leadInfo.LeadId;
            model.Amount = 21m;
            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;

            _sut.SetClient(_clientMock.Object);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(accountBusinessModel);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(model.RecipientAccountId)).ReturnsAsync(account);
            _clientMock
                .Setup(x => x.ExecuteAsync<List<long>>(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<List<long>>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<List<long>>, RestRequestAsyncHandle>>((request, callback) =>
                    {
                        callback(new RestResponse<List<long>>
                        {
                            Data = data,
                            StatusCode = HttpStatusCode.OK
                        }, null);
                    });

            //When
            var actual = await _sut.AddTransferAsync(model, leadInfo);

            //Then
            actual.Should().BeEquivalentTo(commission);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId),Times.Once);
        }

        [Test]
        public async Task AddTransferAsync_DoesNotHaveLongInDatabase_ReturnException()
        {
            //Given
            var lead = LeadData.GetLeadWithTwoAccountsDto();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var excpected = "tstore slomalsy";

            account.LeadId = leadInfo.LeadId;
            model.Amount = 21m;
            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(accountBusinessModel);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(model.RecipientAccountId)).ReturnsAsync(account);
            _clientMock
                .Setup(x => x.ExecuteAsync<List<long>>(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<List<long>>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<List<long>>, RestRequestAsyncHandle>>((request, callback) =>
                    {
                        callback(new RestResponse<List<long>>
                        {
                            StatusCode = HttpStatusCode.OK
                        }, null);
                    });

            //When
            var ex = Assert.ThrowsAsync<Exception>(
                () => _sut.AddTransferAsync(model, leadInfo));

            //Then
            Assert.AreEqual(excpected,ex.Message);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId),Times.Once);
        }

        [Test]
        public async Task AddTransferAsync_DoesNotHaveMoneyOnBalance_ReturnException()
        {
            //Given
            var lead = LeadData.GetLeadWithTwoAccountsDto();
            var accountBusinessModel = TransactionData.GetAccountBusinessModel();
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var excpected = string.Format(ServiceMessages.DoesNotHaveEnoughMoney, account.Id, accountBusinessModel.Balance);

            account.LeadId = leadInfo.LeadId;
            model.Amount = 10221m;
            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;
            accountBusinessModel.Balance = 1000;

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(accountBusinessModel);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(model.RecipientAccountId)).ReturnsAsync(account);

            //When
            var ex = Assert.ThrowsAsync<ValidationException>(
                () => _sut.AddTransferAsync(model, leadInfo));

            //Then
            Assert.AreEqual(excpected, ex.Message);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
        }

        [Test]
        public async Task AddTransferAsync_LeadNotVipTransferToInvalidCurrency_ReturnException()
        {
            //Given
            var lead = LeadData.GetLeadWithTwoAccountsDto();
            var accountBusinessModel = TransactionData.GetEurAccountBusinessModel();
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var excpected = $"{ServiceMessages.IncompleteTransfer}";

            account.LeadId = leadInfo.LeadId;
            model.Amount = 21m;
            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;
            accountBusinessModel.Balance = 22m;

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(accountBusinessModel);
            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(model.RecipientAccountId)).ReturnsAsync(account);

            //When
            var ex = Assert.ThrowsAsync<ValidationException>(
                () => _sut.AddTransferAsync(model, leadInfo));

            //Then
            Assert.AreEqual(excpected, ex.Message);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
            _accountServiceMock.Verify(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo), Times.Once);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(model.RecipientAccountId), Times.Once);
        }

        [Test]
        public async Task CheckDepositTransactionAndSendEmailAsync()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var account = TransactionData.GetEurAccountBusinessModel();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var model = TransactionData.GeTransactionBusinessModel();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(account);

            //When
            await _sut.CheckDepositTransactionAndSendEmailAsync(model,leadInfo);

            //Then
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId),Times.Once);
            _accountServiceMock.Verify(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo),Times.Once);
        }

        [Test]
        public async Task CheckWithdrawTransactionAndSendEmailAsync()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var account = TransactionData.GetEurAccountBusinessModel();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var model = TransactionData.GeTransactionBusinessModel();

            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(account);

            //When
            await _sut.CheckWithdrawTransactionAndSendEmailAsync(model,leadInfo);

            //Then
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId),Times.Once);
            _accountServiceMock.Verify(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo),Times.Once);
        }

        [Test]
        public async Task CheckTransferAndSendEmailAsync()
        {
            //Given
            var lead = LeadData.GetLeadDto();
            var accountBusinessModel = TransactionData.GetEurAccountBusinessModel();
            var account = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var model = TransactionData.GetTransferEurBusinessModel();
            account.LeadId = leadInfo.LeadId;

            _accountRepoMock.Setup(x => x.GetAccountByIdAsync(model.RecipientAccountId)).ReturnsAsync(account);
            _leadRepoMock.Setup(x => x.GetLeadByIdAsync(leadInfo.LeadId)).ReturnsAsync(lead);
            _accountServiceMock.Setup(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo)).ReturnsAsync(accountBusinessModel);

            //When
            await _sut.CheckTransferAndSendEmailAsync(model,leadInfo);

            //Then
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId),Times.Once);
            _accountServiceMock.Verify(x => x.GetAccountWithTransactionsAsync(model.AccountId, leadInfo),Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
        }
    }
}