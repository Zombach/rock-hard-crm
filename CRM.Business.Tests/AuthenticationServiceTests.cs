using System;
using System.Threading.Tasks;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.Tests.TestsDataHelpers;
using CRM.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace CRM.Business.Tests
{
    public class AuthenticationServiceTests
    {
        private Mock<ILeadRepository> _leadRepoMock;
        private readonly IAuthOptions _options;
        private AuthenticationService _sut;

        [SetUp]
        public void SetUp()
        {
            _leadRepoMock = new Mock<ILeadRepository>();
            _sut = new AuthenticationService(_leadRepoMock.Object, _options);
        }

        [Test]
        public void HashPassword_PasswordAndSalt_ReturnSalt()
        {
            //Given 
            var password = "password";
            var salt = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var expected = "AAECAwQFBgcICQoLDA0ODyT2cCjwnE2JIl0Ka2bvFeMtEwX+";

            //When
            var actual = _sut.HashPassword(password, salt);

            //Then
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void HashPassword_WrongSalt_ReturnError()
        {
            //Given 
            var password = "password";
            var salt = new byte[] { 0, 1, 2, 3, 4, 5, 6};

            //When
            Assert.Throws<ArgumentException>(() => _sut.HashPassword(password, salt));
        }

        [Test]
        public void Verify_CorrectPassword_GetTrue()
        {
            //Given 
            var expected = true;
            var hashedPassword = "AAECAwQFBgcICQoLDA0ODyT2cCjwnE2JIl0Ka2bvFeMtEwX+";
            var userPassword = "password";

            //When
            var actual = _sut.Verify(hashedPassword, userPassword);

            //Than
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Verify_IncorrectPassword_GetException()
        {
            //Given 
            var hashedPassword = "AAECAwQFBgcICQoLDA0ODyT2cCjwnE";
            var userPassword = "password";

            //When
            Assert.Throws<FormatException>(() => _sut.Verify(hashedPassword, userPassword));
        }
    }
}