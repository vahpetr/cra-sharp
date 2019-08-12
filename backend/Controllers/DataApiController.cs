using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Services.UserService;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers {
    [ApiController]
    [Route ("api/data")]
    public class DataApiController : ControllerBase {
        public DataApiController () { }

        [Authorize]
        [HttpGet]
        public string Get () {
            return "Secret service data";
        }
    }
}