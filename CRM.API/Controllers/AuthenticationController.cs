using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IAuthenticationService _authService;
        private readonly IMapper _mapper;

        public AuthenticationController
        (
            IMapper mapper,
            IAuthenticationService authService,
            ILeadService leadService
        )
        {
            _authService = authService;
            _leadService = leadService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status201Created)]
        public async Task<ActionResult<LeadOutputModel>> RegisterAsync([FromBody] LeadInputModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);
            var createLeadDto = await _leadService.AddLeadAsync(dto);
            var output = _mapper.Map<LeadOutputModel>(createLeadDto);
            return StatusCode(201, output);
        }

        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<string> SignInAsync(LeadSignInModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);

            return await _authService.SignIn(dto);
        }
    }
}