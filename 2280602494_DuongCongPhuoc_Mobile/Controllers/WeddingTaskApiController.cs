using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeddingTaskApiController : ControllerBase
    {
        private readonly IWeddingTaskRepository _taskRepository;

        public WeddingTaskApiController(IWeddingTaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: api/WeddingTaskApi/event/5
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<WeddingTask>>> GetTasksByEventId(int eventId)
        {
            var tasks = await _taskRepository.GetTasksByEventIdAsync(eventId);
            return Ok(tasks);
        }

        // GET: api/WeddingTaskApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WeddingTask>> GetTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        // POST: api/WeddingTaskApi
        [HttpPost]
        public async Task<ActionResult<WeddingTask>> PostTask(WeddingTask task)
        {
            await _taskRepository.AddTaskAsync(task);
            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        // PUT: api/WeddingTaskApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, WeddingTask task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            try
            {
                await _taskRepository.UpdateTaskAsync(task);
            }
            catch (Exception)
            {
                if (await _taskRepository.GetTaskByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/WeddingTaskApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }

        // POST: api/WeddingTaskApi/template/5
        [HttpPost("template/{eventId}")]
        public async Task<ActionResult<IEnumerable<WeddingTask>>> GenerateTemplateTasks(int eventId)
        {
            // Check if tasks already exist (optional, but good for UX)
            // For now, we append. User can delete duplicates.

            var templateTasks = new List<WeddingTask>
            {
                // 12 Months Before
                new WeddingTask { Title = "Xác định ngân sách dự kiến", Category = "Chung", Priority = "High", DueDate = DateTime.Now.AddDays(7), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Lên danh sách khách mời sơ bộ", Category = "Khách mời", Priority = "High", DueDate = DateTime.Now.AddDays(14), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Chọn ngày cưới", Category = "Chung", Priority = "High", DueDate = DateTime.Now.AddDays(14), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Tìm và đặt nhà hàng/địa điểm", Category = "Địa điểm", Priority = "High", DueDate = DateTime.Now.AddMonths(1), Status = "Pending", EventId = eventId },

                // 9 Months Before
                new WeddingTask { Title = "Chọn váy cưới và vest", Category = "Trang phục", Priority = "Normal", DueDate = DateTime.Now.AddMonths(2), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Đặt dịch vụ quay phim/chụp ảnh", Category = "Ảnh/Video", Priority = "Normal", DueDate = DateTime.Now.AddMonths(2), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Lên ý tưởng trang trí", Category = "Trang trí", Priority = "Normal", DueDate = DateTime.Now.AddMonths(3), Status = "Pending", EventId = eventId },

                // 6 Months Before
                new WeddingTask { Title = "Đặt in thiệp cưới", Category = "Thiệp cưới", Priority = "Normal", DueDate = DateTime.Now.AddMonths(4), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Mua nhẫn cưới", Category = "Trang phục", Priority = "High", DueDate = DateTime.Now.AddMonths(5), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Đặt xe hoa và xe đưa đón", Category = "Hậu cần", Priority = "Normal", DueDate = DateTime.Now.AddMonths(5), Status = "Pending", EventId = eventId },

                // 3 Months Before
                new WeddingTask { Title = "Gửi thiệp mời", Category = "Thiệp cưới", Priority = "High", DueDate = DateTime.Now.AddMonths(6), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Thử món ăn tại nhà hàng", Category = "Ăn uống", Priority = "Normal", DueDate = DateTime.Now.AddMonths(6), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Đặt hoa cầm tay và hoa cài áo", Category = "Hoa", Priority = "Normal", DueDate = DateTime.Now.AddMonths(7), Status = "Pending", EventId = eventId },

                // 1 Month Before
                new WeddingTask { Title = "Chốt danh sách khách mời cuối cùng", Category = "Khách mời", Priority = "High", DueDate = DateTime.Now.AddMonths(8), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Thử váy/vest lần cuối", Category = "Trang phục", Priority = "High", DueDate = DateTime.Now.AddMonths(8), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Phân công công việc ngày cưới", Category = "Hậu cần", Priority = "High", DueDate = DateTime.Now.AddMonths(8), Status = "Pending", EventId = eventId },

                // 1 Week Before
                new WeddingTask { Title = "Kiểm tra lại toàn bộ dịch vụ", Category = "Chung", Priority = "High", DueDate = DateTime.Now.AddDays(260), Status = "Pending", EventId = eventId },
                new WeddingTask { Title = "Chuẩn bị tiền lì xì/tip", Category = "Hậu cần", Priority = "Normal", DueDate = DateTime.Now.AddDays(265), Status = "Pending", EventId = eventId },
            };

            foreach (var task in templateTasks)
            {
                await _taskRepository.AddTaskAsync(task);
            }

            return Ok(templateTasks);
        }

        // GET: api/WeddingTaskApi/stats/5
        [HttpGet("stats/{eventId}")]
        public async Task<ActionResult<object>> GetTaskStats(int eventId)
        {
            var (total, completed) = await _taskRepository.GetTaskStatsAsync(eventId);
            double percentage = total == 0 ? 0 : (double)completed / total * 100;
            
            return Ok(new 
            { 
                TotalTasks = total, 
                CompletedTasks = completed, 
                Percentage = Math.Round(percentage, 1) 
            });
        }
    }
}
