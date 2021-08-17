using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    public class LeadController
    {
        [Authorize]
        [ApiController]
        [Route("api/[controller]")]
        public class UserController : Controller
        {
            private readonly IMapper _mapper;
            private readonly IUserService _userService;

            public UserController(IMapper mapper, IUserService userService)
            {
                _mapper = mapper;
                _userService = userService;
            }
        }
}
