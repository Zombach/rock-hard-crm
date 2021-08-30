using CRM.Business.Options;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CRM.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAuthOptions _options;

        public AuthenticationService(ILeadRepository leadRepository, IAuthOptions authOptions)
        {
            _leadRepository = leadRepository;
            _options = authOptions;
        }
        public string HashPassword(string pass, byte[] salt = default)
        {
            if (salt == default)
            {
                salt = GetSalt();
            }
            var pkbdf2 = new Rfc2898DeriveBytes(pass, salt, 10000, HashAlgorithmName.SHA384);
            byte[] hash = pkbdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }

        public string SignIn(LeadDto dto)
        {
            var identity = GetIdentity(dto.Email, dto.Password);
            if (identity == default)
            {
                return default;
            }

            var jwt = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                notBefore: DateTime.UtcNow,
                claims: identity.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_options.Lifetime)),
                signingCredentials: new SigningCredentials(_options.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public byte[] GetSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            return salt;
        }

        public bool Verify(string hashedPassword, string userPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            string result = HashPassword(userPassword, salt);
            return result == hashedPassword;
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var lead = _leadRepository.GetLeadByEmail(email);

            var claims = new List<Claim>();
            if (lead != default && Verify(lead.Password, password))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.NameId, lead.Id.ToString()));
                claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, lead.Email));
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, lead.Role.ToString()));

                ClaimsIdentity claimsIdentity =
                    new(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return default;
        }
    }
}
