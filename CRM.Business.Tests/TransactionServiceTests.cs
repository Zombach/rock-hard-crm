using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace CRM.Business.Tests
{
    public class TransactionServiceTests
    {
        private Mock<RestClient> _clientMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private TransactionService _sut;


        [SetUp]
        public void SetUp()
        {
            _accountRepoMock = new Mock<IAccountRepository>();
            _clientMock = new Mock<RestClient>();
            _sut = new TransactionService(_accountRepoMock.Object, _clientMock.Object);
        }

        [Test]
        public void AddDeposit()
        {
            //Given
            var expected = 234324243L;
            var model = TransactionData.GeTransactionBusinessModel();
            var account = AccountData.GetAccountDto();

            _accountRepoMock.Setup(x => x
                .GetAccountById(account.Id))
                .Returns(account);
            _clientMock.Setup(x => x
                .Execute<long>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<long>
                {
                    Data = expected
                });

            //When
            var actual = _sut.AddDeposit(model.Id, model);

            //Then
            Assert.AreEqual(expected, actual);
            _accountRepoMock.Verify(x => x.GetAccountById(account.Id), Times.Once);
        }

        [Test]
        public void AddWithdraw()
        {
            //Given
            var expected = 2344243L;
            var model = TransactionData.GeTransactionBusinessModel();
            var account = AccountData.GetAccountDto();

            _accountRepoMock.Setup(x => x
                    .GetAccountById(account.Id))
                .Returns(account);
            _clientMock.Setup(x => x
                    .Execute<long>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<long>
                {
                    Data = expected
                });

            //When
            var actual = _sut.AddWithdraw(model.Id, model);

            //Then
            Assert.AreEqual(expected, actual);
            _accountRepoMock.Verify(x => x.GetAccountById(account.Id), Times.Once);
        }

        [Test]
        public void AddTransfer()
        {
            //Given
            var expected = "Transaction";
            var model = TransactionData.GetTransferBusinessModel();
            var account = AccountData.GetAccountDto();
            var accountAnother = AccountData.GetAnotherAccountDto();

            model.Currency = account.Currency;
            model.RecipientCurrency = accountAnother.Currency;
            _accountRepoMock.Setup(x => x
                    .GetAccountById(model.RecipientAccountId))
                .Returns(account);
            _accountRepoMock.Setup(x => x
                    .GetAccountById(model.AccountId))
                .Returns(accountAnother);
            
          _clientMock.Setup(x => x
                    .Execute<string>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<string>
                {
                    Data = expected
                });

            //When
            var actual = _sut.AddTransfer(model);

            //Then
            Assert.AreEqual(expected, actual);
            _accountRepoMock.Verify(x => x.GetAccountById(model.AccountId), Times.Once);
            _accountRepoMock.Verify(x => x.GetAccountById(model.RecipientAccountId), Times.Once);
        }
    }
}