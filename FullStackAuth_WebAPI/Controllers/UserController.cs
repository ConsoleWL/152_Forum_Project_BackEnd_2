using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            try
            {
                var user = _context.Users.Include(t => t.Topics)
                                     .Include(c => c.Comments)
                                     .FirstOrDefault(u => u.Id == id);

                if (user is null)
                    return NotFound();

                var topics = _context.Topics.Include(u => u.User).Where(user => user.UserId == id).ToList();
                var comments = _context.Comments.Where(user => user.UserId == id).ToList();

                var userDto = new UserForDisplayDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Likes = user.Likes,
                    RegistrationData = user.RegistrationDate,
                    ProfilePicture = user.ProfilePicture,
                    Topics = topics.Select(t => new TopicsForDisplayDto
                    {
                        TopicId = t.TopicId,
                        Title = t.Title,
                        TimePosted = t.TimePosted,
                        Likes = t.Likes,
                        Text = t.Text
                    }).ToList(),
                    Comments = comments.Select(c => new CommentsForDisplayDto
                    {
                        CommentId = c.CommentId,
                        Text = c.Text,
                        TimePosted = c.TimePosted,
                        Likes = c.Likes,
                        User = new UserNameDto
                        {
                            UserName = c.User.UserName
                        },

                    }).ToList()
                };

                return Ok(userDto);

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(string id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(user => user.Id == id);
                if (user is null)
                    return NotFound();

                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                _context.Users.Remove(user);
                _context.SaveChanges();

                return StatusCode(204);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult Put(string id, [FromBody] User userToUpdate)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(user => user.Id == id);
                if (user is null)
                    return NotFound();

                var userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                user.UserName = userToUpdate.UserName;
                user.FirstName = userToUpdate.FirstName;
                user.LastName = userToUpdate.LastName;
                user.ProfilePicture = userToUpdate.ProfilePicture;
                user.Email = userToUpdate.Email;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                return StatusCode(201, user);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        

        //doesn't work

        [HttpGet("mess/{idfrom}"), Authorize]
        public IActionResult GetMessages(string idfrom)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var messages = _context.DirectMessages
                    .Where((m => m.UserIdToId == userId && m.UserIdFromId == idfrom)).ToList();

                return Ok(messages);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
