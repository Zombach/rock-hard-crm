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

        public AuthenticationService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public string SignIn(LeadDto dto)
        {
            var identity = GetClaimsIdentity(dto.Email, dto.Password);
            if (identity == default)
            {
                return default;
            }

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: DateTime.UtcNow,
                claims: identity.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        private ClaimsIdentity GetClaimsIdentity(string email, string password)
        {
            var lead = _leadRepository.GetLeadByEmail(email);

            var claims = new List<Claim>();
            if (lead != default)
            {
                if (BCrypt.Net.BCrypt.EnhancedVerify(password, lead.Password, BCrypt.Net.HashType.SHA384))
                {
                    claims.Add(new Claim(JwtRegisteredClaimNames.NameId, lead.Id.ToString()));
                    claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, lead.Email));
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, lead.Role.ToString()));
                };

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return default;
        }
    }
}
