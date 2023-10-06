using AutoMapper.Configuration.Conventions;
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
    public class TopicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TopicController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("user/{userId}")]
        public IActionResult GetTopicsByUserId(string userId)
        {
            try
            {
                List<Topic> topics = _context.Topics.Include(u => u.User)
                                                    .Include(c => c.Comments)
                                                    .ThenInclude(c => c.User)
                                                    .Where(u=>u.UserId == userId)
                                                    .ToList();


                var topicsDto = topics.Select(t => new TopicsForDisplayDto
                {
                    TopicId = t.TopicId,
                    Title = t.Title,
                    TimePosted = t.TimePosted,
                    Likes = t.Likes,
                    Text = t.Text,
                    User = new UserNameDto
                    {
                        UserName = t.User.UserName
                    },
                    Comments = t.Comments.Select(c => new CommentsForDisplayDto
                    {
                        CommentId = c.CommentId,
                        Likes = c.Likes,
                        Text = c.Text,
                        TimePosted = c.TimePosted,
                        User = new UserNameDto
                        {
                            UserName = c.User.UserName
                        }
                    }).ToList()

                }).ToList();

                return Ok(topicsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Topic> topics = _context.Topics.Include(u=>u.User)
                                                    .Include(c=>c.Comments)
                                                    .ThenInclude(c=>c.User)
                                                    .ToList();
                

                var topicsDto = topics.Select(t => new TopicsForDisplayDto
                {
                    TopicId = t.TopicId,
                    Title = t.Title,
                    TimePosted = t.TimePosted,
                    Likes = t.Likes,
                    Text = t.Text,
                    User = new UserNameDto
                    {
                        UserName = t.User.UserName
                    },
                    Comments = t.Comments.Select(c => new  CommentsForDisplayDto{
                        CommentId = c.CommentId,
                        Likes = c.Likes,
                        Text = c.Text,
                        TimePosted = c.TimePosted,
                        User = new UserNameDto
                        {
                            UserName = c.User.UserName
                        }
                    }).ToList()
                    
                }).ToList();



                return Ok(topicsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var topic = _context.Topics.Include(u => u.User)
                                           .Include(c => c.Comments)
                                           .ThenInclude(c => c.User)
                                           .FirstOrDefault(topic => topic.TopicId == id);

                var user = _context.Users.FirstOrDefault(u => u.Id == topic.UserId);
                var comments = _context.Comments.Where(c => c.TopicId == id).ToList();

                if (topic is null)
                    return NotFound();

                var userDto = new UserNameDto
                {
                    UserName = user.UserName
                };

                var topicDto = new TopicsForDisplayDto
                {
                    TopicId = topic.TopicId,
                    Title = topic.Title,
                    Text = topic.Text,
                    TimePosted = topic.TimePosted,
                    Likes = topic.Likes,
                    User = userDto,
                    Comments = comments.Select(c => new CommentsForDisplayDto
                    {
                        CommentId = c.CommentId,
                        Text = c.Text,
                        TimePosted = c.TimePosted,
                        Likes = c.Likes,
                        User = new UserNameDto
                        {
                            UserName = c.User.UserName
                        }

                    }).ToList()
                };

                return Ok(topicDto);
                

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        

        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Topic topic)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                topic.UserId = userId;
                topic.TimePosted = DateTime.Now;

                _context.Topics.Add(topic);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                return StatusCode(201, topic);

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
                var topic = _context.Topics.FirstOrDefault(topic => topic.TopicId == id);
                if (topic is null)
                    return NotFound();

                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || topic.UserId != userId)
                    return Unauthorized();

                _context.Topics.Remove(topic);
                _context.SaveChanges();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Topic topic)
        {
            try
            {
                var existTopic = _context.Topics.Include(u => u.User)
                                                .FirstOrDefault(topic => topic.TopicId == id);
                if (existTopic is null)
                    return NotFound();

                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId) || existTopic.UserId != userId)
                    return Unauthorized();

                existTopic.Title = topic.Title;
                existTopic.Text = topic.Text;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.SaveChanges();

                var userForUpdateDto = new UserNameDto
                {
                    UserName = existTopic.User.UserName
                };

                var topicforUpdateDto = new TopicsForDisplayDto
                {
                    Title = existTopic.Title,
                    Text = existTopic.Text,
                    User = userForUpdateDto
                };

                return StatusCode(201, topicforUpdateDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("like/{id}"), Authorize]
        public IActionResult Like(int id)
        {
            try
            {
                var topic = _context.Topics.FirstOrDefault(t => t.TopicId == id);
                if (topic is null)
                    return NotFound();

                topic.Likes++;

                _context.SaveChanges();

                return Ok(topic.Likes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
    }
}
