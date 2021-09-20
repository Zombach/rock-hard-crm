using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Repositories;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class Tests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private Mock<IPublishEndpoint> _publishEndpointMock;

        private LeadService _sut;

        [SetUp]
        public void Setup()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _accountRepoMock = new Mock<IAccountRepository>();
            var authenticationService = new AuthenticationService(_leadRepoMock.Object, new Mock<IAuthOptions>().Object);
            var validationHelper = new LeadValidationHelper(_leadRepoMock.Object);

            _sut = new LeadService(
                _leadRepoMock.Object,
                _accountRepoMock.Object,
                authenticationService,
                validationHelper,
                _publishEndpointMock.Object);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}