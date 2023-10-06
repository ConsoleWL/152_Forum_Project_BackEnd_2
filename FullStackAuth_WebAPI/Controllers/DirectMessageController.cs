using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
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

        [HttpGet, Authorize]
        public IActionResult GetAllChats()
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var usersWithMessages = _context.DirectMessages.Where(m => m.UserIdToId == userId).Select(u=>u.UserIdFromId).Distinct().ToList();
                var users = _context.Users.Where(m => usersWithMessages.Any(n => n == m.Id));

                var userswithMessgesDTO = users.Select(u => new UserForDisplayDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    RegistrationData = u.RegistrationDate,
                    ProfilePicture = u.ProfilePicture,
                    Likes = u.Likes
                }).ToList();

                return Ok(userswithMessgesDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
                directMessage.MessageTime = DateTime.Now;
                

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
