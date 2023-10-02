using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectMessageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DirectMessageController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost, Authorize]
        public IActionResult Post([FromBody] DirectMessage directMessage)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                directMessage.UserIdFromId = userId;

                _context.DirectMessages.Add(directMessage);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                return StatusCode(201, directMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
