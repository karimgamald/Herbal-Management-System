using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PhytoIntellect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize(Roles = "User,Herbalist")]
        [HttpGet("users")]
        public IActionResult UserEndpoint()
        {
            return Ok("Users and Herbalist allowed");
        }

        // 🔒 Protected endpoint
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("Any authenticated user");
        }

    }
}
