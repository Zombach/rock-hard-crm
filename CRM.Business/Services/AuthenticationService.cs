using CRM.Business.Options;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var lead = _leadRepository.GetLeadByEmail(email);

            var claims = new List<Claim>();
            if (lead != default && BCrypt.Net.BCrypt.EnhancedVerify(password, lead.Password))
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
