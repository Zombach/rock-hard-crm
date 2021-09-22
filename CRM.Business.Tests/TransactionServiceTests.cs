using System.Threading.Tasks;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
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
        private TransactionService _sut;
        private IAccountValidationHelper _accountValidationHelper;
        private Mock<ICommissionFeeService> _commissionFeeServiceMock;
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IPublishEndpoint> _publishEndPointMock;

        [SetUp]
        public void SetUp()
        {
            _commissionFeeServiceMock = new Mock<ICommissionFeeService>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _accountServiceMock = new Mock<IAccountService>();
            _publishEndPointMock = new Mock<IPublishEndpoint>();
            _leadRepoMock = new Mock<ILeadRepository>();
            _clientMock = new Mock<RestClient>();
            _accountValidationHelper = new AccountValidationHelper(_accountRepoMock.Object);

            var optionsMock = new Mock<IOptions<CommissionSettings>>();
            optionsMock.Setup(x => x.Value).Returns(new CommissionSettings
            {
                Commission = 0.2m,
                CommissionModifier = 1.5m,
                VipCommission = 0.1m
            });

            _sut = new TransactionService(
                optionsMock.Object,
                _accountValidationHelper,
                _accountServiceMock.Object,
                _commissionFeeServiceMock.Object,
                _publishEndPointMock.Object,
                _leadRepoMock.Object,
                _clientMock.Object);
        }

        [Test]
        public async Task AddDeposit()
        {
            //Given
            const long data = 21321L;
            var model = TransactionData.GeTransactionBusinessModel();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();
            var account = AccountData.GetUsdAccountDto();
            var lead = LeadData.GetLeadDto();
            var commission = new CommissionFeeDto
            {
                Id = 123,
                LeadId = leadInfo.LeadId,
                AccountId = model.AccountId,
                TransactionId = data,
                Role = leadInfo.Role,
                CommissionAmount = 0.2m,
                TransactionType = TransactionType.Deposit
            };
            account.LeadId = leadInfo.LeadId;

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
            Assert.AreEqual(commission, actual);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadByIdAsync(leadInfo.LeadId), Times.Once);
            _commissionFeeServiceMock.Verify(x => x.AddCommissionFeeAsync(commission), Times.Once);

        }

        [Test]
        public void AddWithdraw()
        {
            //Given
            var expected = 2344243L;
            var model = TransactionData.GeTransactionBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();

            _accountRepoMock
                .Setup(x => x.GetAccountByIdAsync(account.Id))
                .ReturnsAsync(account);
            _clientMock
                .Setup(x => x.Execute<long>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<long>
                {
                    Data = expected
                });

            //When
            var actual = _sut.AddWithdrawAsync(model, leadInfo);

            //Then
            Assert.AreEqual(expected, actual);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
        }

        [Test]
        public void AddTransfer()
        {
            //Given
            var expected = "Transaction";
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetUsdAccountDto();
            var accountAnother = AccountData.GetEurAccountDto();
            var leadInfo = LeadIdentityInfoData.GetRegularLeadIdentityInfo();

            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;
            _accountRepoMock.Setup(x => x
                    .GetAccountByIdAsync(model.RecipientAccountId))
                .ReturnsAsync(account);
            _accountRepoMock.Setup(x => x
                    .GetAccountByIdAsync(model.AccountId))
                .ReturnsAsync(accountAnother);

            _clientMock.Setup(x => x
                      .Execute<string>(It.IsAny<IRestRequest>()))
                  .Returns(new RestResponse<string>
                  {
                      Data = expected
                  });

            //When
            var actual = _sut.AddTransferAsync(model, leadInfo);

            //Then
            Assert.AreEqual(expected, actual);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(model.AccountId), Times.Once);
            _accountRepoMock.Verify(x => x.GetAccountByIdAsync(model.RecipientAccountId), Times.Once);
        }
    }
}