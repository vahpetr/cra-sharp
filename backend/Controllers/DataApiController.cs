using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    [ApiController]
    [Route ("api/data")]
    [Produces ("application/json")]
    public class DataApiController : ControllerBase {
        public DataApiController () { }

        [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public string Get () {
            return "Перед собой вы видите текст с сервера доступный только авторизованному пользователю и полученный через защищённый метод API.";
        }
    }
}