using System.Collections.Generic;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class LeadServiceTests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private Mock<IAccountRepository> _accountRepoMock;
        private IAuthenticationService _authenticationService;
        private IAuthOptions _authOptions;
        private LeadService _sut;


        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _accountRepoMock = new Mock<IAccountRepository>();
            _authenticationService = new AuthenticationService(_leadRepoMock.Object, _authOptions);
            _sut = new LeadService(_leadRepoMock.Object, _accountRepoMock.Object, _authenticationService);
        }

        [Test]
        public void AddLead()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            var input = LeadData.GetInputLeadDto();
            _leadRepoMock.Setup(x => x.AddLead(input)).Returns(expectedLeadDto.Id);
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.AddLead(input);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.AddLead(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void UpdateLead()
        {
            //Given
            var input = LeadData.GetInputLeadDto();
            var expectedLeadDto = LeadData.GetLead();
            var userInfo = UserIdentityInfoData.GetUserIdentityInfo(expectedLeadDto.Id, new List<Role> { expectedLeadDto.Role });
            _leadRepoMock.Setup(x => x.UpdateLead(input));
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.UpdateLead(expectedLeadDto.Id, input, userInfo);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.UpdateLead(input), Times.Once);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void GetAllLeads()
        {
            //Given
            var expectedList = LeadData.GetListLeadDto();
            _leadRepoMock.Setup(x => x.GetAllLeads()).Returns(expectedList);

            //When
            var actualList = _sut.GetAllLeads();

            //Then
            Assert.AreEqual(expectedList, actualList);
            _leadRepoMock.Verify(x => x.GetAllLeads(), Times.Once);
        }

        [Test]
        public void GetLeadById()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.GetLeadById(expectedLeadDto.Id)).Returns(expectedLeadDto);

            //When
            var actualLeadDto = _sut.GetLeadById(expectedLeadDto.Id);

            //Then
            Assert.AreEqual(expectedLeadDto, actualLeadDto);
            _leadRepoMock.Verify(x => x.GetLeadById(expectedLeadDto.Id), Times.Once);
        }

        [Test]
        public void DeleteLeadById()
        {
            //Given
            var expectedLeadDto = LeadData.GetLead();
            _leadRepoMock.Setup(x => x.DeleteLeadById(expectedLeadDto.Id));


            //When
            _sut.DeleteLeadById(expectedLeadDto.Id);

            //Then
            _leadRepoMock.Verify(x => x.DeleteLeadById(expectedLeadDto.Id), Times.Once);
        }
    }
}