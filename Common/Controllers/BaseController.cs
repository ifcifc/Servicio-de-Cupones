using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T, V> : ControllerBase where T : Model where V : DbContext
    {
        protected readonly V _context;

        public BaseController(V context)
        {
            _context = context;
        }

        protected abstract bool Any(int id);

        [HttpGet]
        public abstract Task<ActionResult<IEnumerable<T>>> GetAll();

        [HttpGet("{Id}")]
        public abstract Task<IActionResult> GetByID(int Id);

        [HttpPost]
        public abstract Task<IActionResult> Add(T model);

        [HttpPut]
        public abstract Task<IActionResult> Update(T model);

        [HttpDelete("{Id}")]
        public abstract Task<IActionResult> Delete(int Id);

    }
}
