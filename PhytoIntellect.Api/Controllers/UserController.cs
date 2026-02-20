using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Herbal_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [Authorize(Roles = "User,Admin")]
        [HttpGet("users")]
        public IActionResult UserEndpoint()
        {
            return Ok("Users and admins allowed");
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
