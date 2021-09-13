using CRM.Business.Options;
using CRM.Business.Services;
using CRM.DAL.Repositories;
using CRM.Business.ValidationHelpers;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class Tests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IAccountRepository> _accountRepoMock;

        private LeadService _sut;

        [SetUp]
        public void Setup()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _accountRepoMock = new Mock<IAccountRepository>();
            IAuthenticationService authenticationService = new AuthenticationService(_leadRepoMock.Object, new Mock<IAuthOptions>().Object);

            _sut = new LeadService(
                _leadRepoMock.Object, 
                _accountRepoMock.Object, 
                authenticationService, 
                new LeadValidationHelper(_leadRepoMock.Object));
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}