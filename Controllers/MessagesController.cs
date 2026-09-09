using DatingApp.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        // DỮ LIỆU GIẢ
        private static List<MessageDto> _fakeMessages =
            new List<MessageDto>
        {
            new MessageDto
            {
                Id = 1,
                ConversationId = 1,
                SenderId = 1,
                SenderName = "Nguyen Van A",
                Content = "Chao ban!",
                SentAt = DateTime.Now.AddMinutes(-10),
                IsRead = true
            },

            new MessageDto
            {
                Id = 2,
                ConversationId = 1,
                SenderId = 2,
                SenderName = "Nguyen Van B",
                Content = "Chao ban, rat vui duoc lam quen!",
                SentAt = DateTime.Now.AddMinutes(-5),
                IsRead = true
            },

            new MessageDto
            {
                Id = 3,
                ConversationId = 1,
                SenderId = 1,
                SenderName = "Nguyen Van A",
                Content = "Ban dang lam gi vay?",
                SentAt = DateTime.Now,
                IsRead = false
            }
        };

        // 1. GET ALL MESSAGE
        // GET: api/messages
        [HttpGet]
        public ActionResult<List<MessageDto>> GetAll()
        {
            return Ok(_fakeMessages);
        }

        // 2. GET MESSAGE BY ID
        // GET: api/messages/1
        [HttpGet("{id:int}")]
        public ActionResult<MessageDto> GetById(int id)
        {
            var message = _fakeMessages
                .FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay tin nhan co Id = {id}"
                });
            }

            return Ok(message);
        }

        // 3. GET MESSAGE BY CONVERSATION
        // GET: api/messages/conversation/1
        [HttpGet("conversation/{conversationId:int}")]
        public ActionResult<List<MessageDto>> GetByConversation(
            int conversationId)
        {
            var messages = _fakeMessages
                .Where(m => m.ConversationId == conversationId)
                .ToList();

            return Ok(messages);
        }

        // 4. CREATE MESSAGE
        // POST: api/messages
        [HttpPost]
        public ActionResult<MessageDto> Create(
            [FromBody] SendMessageDto request)
        {
            var message = new MessageDto
            {
                Id = _fakeMessages.Count == 0
                    ? 1
                    : _fakeMessages.Max(m => m.Id) + 1,

                ConversationId = request.ConversationId,

                // Tạm giả lập SenderId
                SenderId = 1,

                SenderName = "Nguyen Van A",

                Content = request.Content,

                SentAt = DateTime.Now,

                IsRead = false
            };

            _fakeMessages.Add(message);

            return CreatedAtAction(
                nameof(GetById),
                new { id = message.Id },
                message
            );
        }

        // 5. UPDATE MESSAGE
        // PUT: api/messages/1
        [HttpPut("{id:int}")]
        public ActionResult<MessageDto> Update(
            int id,
            [FromBody] MessageDto request)
        {
            var message = _fakeMessages
                .FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay tin nhan co Id = {id}"
                });
            }

            message.Content = request.Content;
            message.IsRead = request.IsRead;

            return Ok(message);
        }

        // 6. DELETE MESSAGE
        // DELETE: api/messages/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var message = _fakeMessages
                .FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay tin nhan co Id = {id}"
                });
            }

            _fakeMessages.Remove(message);

            return Ok(new
            {
                message = "Xoa tin nhan thanh cong."
            });
        }
    }
}