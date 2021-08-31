using CRM.Business.Services;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepoMock;
        private AccountService _sut;

        [SetUp]
        public void SetUp()
        {
            _accountRepoMock = new Mock<IAccountRepository>();
            //_sut = new AccountService(_accountRepoMock,)
        }




        [Test]
        public void AddAccount()
        {

        }

        [Test]
        public void DeleteAccount()
        {

        }

        [Test]
        public void GetAccountWithTransactions()
        {

        }
    }
}
