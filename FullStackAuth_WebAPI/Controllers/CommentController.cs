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
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // get all comments by UserId in the profile

        [HttpGet("user/{userId}")]
        public IActionResult Get(string userId)
        {
            try
            {
                var comments = _context.Comments.Where(c => c.UserId == userId).ToList();

                
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Comment comment)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                comment.UserId = userId;

                _context.Comments.Add(comment);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                return StatusCode(201, comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                var comment = _context.Comments.FirstOrDefault(c => c.CommentId == id);
                if (comment is null)
                    return NotFound();

                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || comment.UserId != userId)
                    return Unauthorized();

                _context.Comments.Remove(comment);
                _context.SaveChanges();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Comment comment)
        {
            try
            {
                var existComment = _context.Comments.Include(o => o.User)
                                                    .Include(t => t.Topic)
                                                    .FirstOrDefault(f => f.CommentId == id);
                if (existComment is null)
                    return NotFound();

                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId) || existComment.UserId != userId)
                    return Unauthorized();

                existComment.Text = comment.Text;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                var existingCommentDto = new CommentsForDisplayDto
                {
                    CommentId = existComment.CommentId,
                    Text = existComment.Text,
                    TimePosted = existComment.TimePosted,
                    Likes = existComment.Likes,
                    User = new UserNameDto
                    {
                        UserName = existComment.User.UserName
                    }
                };

                return StatusCode(201, existingCommentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("like/{id}"), Authorize]
        public IActionResult LikeMessages(int id)
        {
            try
            {
                var message = _context.Comments.FirstOrDefault(c => c.CommentId == id);

                if (message is null) 
                    return NotFound(); 

                message.Likes++;

                _context.SaveChanges();

                return Ok(message.Likes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
