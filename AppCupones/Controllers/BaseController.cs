using AppCupones.Data;
using AppCupones.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppCupones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T> : ControllerBase where T : Model
    {
        protected readonly DbAppContext _context;

        public BaseController(DbAppContext context)
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
