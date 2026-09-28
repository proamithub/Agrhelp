using HelpData;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HelpUI.Helpers;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using HelpRepo;

namespace HelpUI.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public CustomPrincipal CurrentUser
        {
            get
            {
                if (User.Claims.Any())
                {
                    string userData = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
                    var user = JsonConvert.DeserializeObject<CustomPrincipal>(userData);
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
        protected IUnitOfWork uow;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected ISession Session => httpContextAccessor.HttpContext.Session;
        IConfiguration config;
        public BaseApiController(IServiceProvider serviceProvider)
        {
            uow = serviceProvider.GetService<IUnitOfWork>();
            config = serviceProvider.GetService<IConfiguration>();
            httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        }
    }
}
