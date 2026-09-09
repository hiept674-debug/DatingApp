using DatingApp.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        // DỮ LIỆU GIẢ
        private static List<NotificationDto> _fakeNotifications =
            new List<NotificationDto>
        {
            new NotificationDto
            {
                Id = 1,
                Title = "Tin nhan moi",
                Content = "Ban co mot tin nhan moi tu Nguyen Van B.",
                Type = "MESSAGE",
                IsRead = false,
                CreatedAt = DateTime.Now
            },

            new NotificationDto
            {
                Id = 2,
                Title = "Match moi",
                Content = "Ban va Tran Thi C da match voi nhau!",
                Type = "MATCH",
                IsRead = true,
                CreatedAt = DateTime.Now.AddHours(-1)
            },

            new NotificationDto
            {
                Id = 3,
                Title = "Like moi",
                Content = "Co nguoi vua thich ban.",
                Type = "LIKE",
                IsRead = false,
                CreatedAt = DateTime.Now.AddHours(-2)
            }
        };

        // 1. GET ALL
        // GET: api/notifications
        [HttpGet]
        public ActionResult<List<NotificationDto>> GetAll()
        {
            return Ok(_fakeNotifications);
        }

        // 2. GET BY ID
        // GET: api/notifications/1
        [HttpGet("{id:int}")]
        public ActionResult<NotificationDto> GetById(int id)
        {
            var notification = _fakeNotifications
                .FirstOrDefault(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay thong bao co Id = {id}"
                });
            }

            return Ok(notification);
        }

        // 3. CREATE
        // POST: api/notifications
        [HttpPost]
        public ActionResult<NotificationDto> Create(
            [FromBody] NotificationDto request)
        {
            request.Id = _fakeNotifications.Count == 0
                ? 1
                : _fakeNotifications.Max(n => n.Id) + 1;

            request.CreatedAt = DateTime.Now;
            request.IsRead = false;

            _fakeNotifications.Add(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = request.Id },
                request
            );
        }

        // 4. UPDATE
        // PUT: api/notifications/1
        [HttpPut("{id:int}")]
        public ActionResult<NotificationDto> Update(
            int id,
            [FromBody] NotificationDto request)
        {
            var notification = _fakeNotifications
                .FirstOrDefault(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay thong bao co Id = {id}"
                });
            }

            notification.Title = request.Title;
            notification.Content = request.Content;
            notification.Type = request.Type;
            notification.IsRead = request.IsRead;

            return Ok(notification);
        }

        // 5. DELETE
        // DELETE: api/notifications/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var notification = _fakeNotifications
                .FirstOrDefault(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay thong bao co Id = {id}"
                });
            }

            _fakeNotifications.Remove(notification);

            return Ok(new
            {
                message = "Xoa thong bao thanh cong."
            });
        }

        // 6. MARK AS READ
        // PUT: api/notifications/1/read
        [HttpPut("{id:int}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _fakeNotifications
                .FirstOrDefault(n => n.Id == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = $"Khong tim thay thong bao co Id = {id}"
                });
            }

            notification.IsRead = true;

            return Ok(notification);
        }
    }
}