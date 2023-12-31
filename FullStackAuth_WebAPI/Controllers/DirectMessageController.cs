﻿using FullStackAuth_WebAPI.Data;
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

        [HttpGet("messages/{userFromId}"), Authorize]
        public IActionResult GetMessagesOfUser(string userFromId)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();


                var myMessages = _context.DirectMessages.Where(ufrom => ufrom.UserIdFromId == userId && ufrom.UserIdToId == userFromId).ToList();

                var userMessages = _context.DirectMessages.Where(ufrom => ufrom.UserIdToId == userId && ufrom.UserIdFromId == userFromId).ToList();
                
                myMessages.AddRange(userMessages);

                myMessages.Sort(new DirectMessageDateComparer());

                var direcMessagesDto = myMessages.Select(m => new DirectMessageDto
                {
                    DirectMessageId = m.DirectMessageId,
                    Text = m.Text,
                    MessageTime = m.MessageTime,
                    UserIdFromId = m.UserIdFromId,
                    UserIdFromName = _context.Users.FirstOrDefault(u=>u.Id == m.UserIdFromId).UserName,
                    UserIdToId = m.UserIdToId,
                    UserIdToName = _context.Users.FirstOrDefault(u=>u.Id == m.UserIdToId).UserName

                }).ToList();

                return Ok(direcMessagesDto);
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
