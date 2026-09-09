using DatingApp.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        // DỮ LIỆU GIẢ
        private static List<ConversationDto> _fakeConversations =
            new List<ConversationDto>
        {
            new ConversationDto
            {
                Id = 1,
                MatchId = 1,
                OtherUserId = 2,
                OtherUserName = "Nguyen Van B",
                OtherUserAvatar = "avatar2.jpg",
                LastMessage = "Chao ban!",
                LastMessageAt = DateTime.Now
            },

            new ConversationDto
            {
                Id = 2,
                MatchId = 2,
                OtherUserId = 3,
                OtherUserName = "Tran Thi C",
                OtherUserAvatar = "avatar3.jpg",
                LastMessage = "Hom nay ban the nao?",
                LastMessageAt = DateTime.Now
            }
        };

        // 1. GET ALL
        // GET: api/conversations
        [HttpGet]
        public ActionResult<List<ConversationDto>> GetAll()
        {
            return Ok(_fakeConversations);
        }

        // 2. GET BY ID
        // GET: api/conversations/1
        [HttpGet("{id:int}")]
        public ActionResult<ConversationDto> GetById(int id)
        {
            var conversation = _fakeConversations
                .FirstOrDefault(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay cuoc tro chuyen co Id = {id}"
                });
            }

            return Ok(conversation);
        }

        // 3. CREATE
        // POST: api/conversations
        [HttpPost]
        public ActionResult<ConversationDto> Create(
            [FromBody] ConversationDto request)
        {
            request.Id = _fakeConversations.Count == 0
                ? 1
                : _fakeConversations.Max(c => c.Id) + 1;

            request.LastMessageAt = DateTime.Now;

            _fakeConversations.Add(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = request.Id },
                request
            );
        }

        // 4. UPDATE
        // PUT: api/conversations/1
        [HttpPut("{id:int}")]
        public ActionResult<ConversationDto> Update(
            int id,
            [FromBody] ConversationDto request)
        {
            var conversation = _fakeConversations
                .FirstOrDefault(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay cuoc tro chuyen co Id = {id}"
                });
            }

            conversation.MatchId = request.MatchId;
            conversation.OtherUserId = request.OtherUserId;
            conversation.OtherUserName = request.OtherUserName;
            conversation.OtherUserAvatar = request.OtherUserAvatar;
            conversation.LastMessage = request.LastMessage;
            conversation.LastMessageAt = request.LastMessageAt;

            return Ok(conversation);
        }

        // 5. DELETE
        // DELETE: api/conversations/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var conversation = _fakeConversations
                .FirstOrDefault(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay cuoc tro chuyen co Id = {id}"
                });
            }

            _fakeConversations.Remove(conversation);

            return Ok(new
            {
                message = "Xoa cuoc tro chuyen thanh cong."
            });
        }
    }
}