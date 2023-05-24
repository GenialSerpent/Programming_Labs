using System;
using DemoWebApp.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DemoWebApp.Controllers
{
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        public ChatsController()
        {
        }

        [HttpGet("api/chats")]
        public IActionResult Read(
            [FromQuery] string orderBy = "Id",
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 25)
        {
            return Ok();
        }

        [HttpPost("api/chats")]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpDelete("api/chats/{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpGet("api/chats/{chatId}/messages")]
        public IActionResult ReadMessages(
            int chatId,
            [FromQuery] string orderBy = "Id",
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 25)
        {
            return Ok();
        }
    }
}