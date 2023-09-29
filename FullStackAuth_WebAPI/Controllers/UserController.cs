using FullStackAuth_WebAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult Get(string id)
        {
            var user = _context.Users.Include(t => t.Topics)
                                     .Include(c => c.Comments)
                                     .FirstOrDefault(u => u.Id == id);

            if (user is null)
                return NotFound();

            var topics = _context.Topics.Include(u => u.User).Where(user => user.UserId == id).ToList();
            var comments = _context.Comments.Where(user => user.UserId == id).ToList();


        }
    }
}
