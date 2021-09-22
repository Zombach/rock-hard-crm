using System;
using System.Threading.Tasks;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class CommissionFeeServiceTests
    {
        private Mock<ICommissionFeeRepository> _commissionFeeRepoMock;
        private CommissionFeeService _sut;

        [SetUp]
        public void SetUp()
        {
            _commissionFeeRepoMock = new Mock<ICommissionFeeRepository>();
            _sut = new CommissionFeeService(_commissionFeeRepoMock.Object);
        }

        [Test]
        public async Task AddCommissionFeeAsync()
        {
            //Given
            var dto = CommissionFeeData.GetCommissionFeeDto();
            _commissionFeeRepoMock.Setup(x => x.AddCommissionFeeAsync(dto)).ReturnsAsync(dto.Id);

            //When
            var actual = await _sut.AddCommissionFeeAsync(dto);

            //Then
            Assert.AreEqual(dto.Id, actual);
            _commissionFeeRepoMock.Verify(x => x.AddCommissionFeeAsync(dto), Times.Once);
        }

        [Test]
        public async Task GetCommissionFeesByAccountIdAsync()
        {
            //Given
            var list = CommissionFeeData.GetListCommissionFeeDto();
            var account = AccountData.GetRubAccountDto();
            _commissionFeeRepoMock.Setup(x => x.GetCommissionFeesByAccountIdAsync(account.LeadId)).ReturnsAsync(list);

            //When
            var actual = await _sut.GetCommissionFeesByAccountIdAsync(account.Id);

            //Then
            //Assert.AreEqual(account.Id, actual);

            //actual.Should().BeEquivalentTo(account.Id);
            _commissionFeeRepoMock.Verify(x => x.GetCommissionFeesByAccountIdAsync(account.LeadId), Times.Once);
        }

        [Test]
        public async Task GetAllCommissionFeesAsync()
        {
            //Given
            var list = CommissionFeeData.GetListCommissionFeeDto();
            _commissionFeeRepoMock.Setup(x => x.GetAllCommissionFeesAsync()).ReturnsAsync(list);

            //When
            var actual = await _sut.GetAllCommissionFeesAsync();

            //Then
            Assert.AreEqual(list, actual);
            _commissionFeeRepoMock.Verify(x => x.GetAllCommissionFeesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCommissionFeesByLeadIdAsync()
        {
            //Given
            var list = CommissionFeeData.GetListCommissionFeeDto();
            var lead = LeadData.GetLeadDto();
            _commissionFeeRepoMock.Setup(x => x.GetCommissionFeesByLeadIdAsync(lead.Id)).ReturnsAsync(list);

            //When
            var actual = await _sut.GetCommissionFeesByLeadIdAsync(lead.Id);

            //Then
            Assert.AreEqual(list, actual);
            _commissionFeeRepoMock.Verify(x => x.GetCommissionFeesByLeadIdAsync(lead.Id), Times.Once);
        }

        [Test]
        public async Task GetCommissionFeesByRoleAsync()
        {
            //Given
            var list = CommissionFeeData.GetListCommissionFeeDto();
            var role = Role.Regular;
            _commissionFeeRepoMock.Setup(x => x.GetCommissionFeesByRoleAsync((int)role)).ReturnsAsync(list);

            //When
            var actual = await _sut.GetCommissionFeesByRoleAsync(role);

            //Then
            Assert.AreEqual(list, actual);
            _commissionFeeRepoMock.Verify(x => x.GetCommissionFeesByRoleAsync((int)role), Times.Once);
        }

        [Test]
        public async Task SearchingCommissionFeesForThePeriodAsync()
        {
            //Given
            var list = CommissionFeeData.GetListCommissionFeeDto();
            var dto = new TimeBasedAcquisitionDto
            {
                From = DateTime.Now,
                To = DateTime.Now,
                Role = Role.Regular
            };
            _commissionFeeRepoMock.Setup(x => x.SearchingCommissionFeesForThePeriodAsync(dto)).ReturnsAsync(list);

            //When
            var actual = await _sut.SearchingCommissionFeesForThePeriodAsync(dto);

            //Then
            Assert.AreEqual(list, actual);
            _commissionFeeRepoMock.Verify(x => x.SearchingCommissionFeesForThePeriodAsync(dto), Times.Once);
        }

    }
}
