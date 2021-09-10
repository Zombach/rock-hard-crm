using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<LeadOutputModel> Register([FromBody] LeadInputModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);
            var createLeadDto = _leadService.AddLead(dto);
            var output = _mapper.Map<LeadOutputModel>(createLeadDto);
            return StatusCode(201, output);
        }

        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public string SignIn(LeadSignInModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);
            return _authService.SignIn(dto);
        }
    }
}