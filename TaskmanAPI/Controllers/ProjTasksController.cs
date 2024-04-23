using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Model;

namespace TaskmanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjTasksController : ControllerBase
    {
        private readonly DefaultContext _context;

        public ProjTasksController(DefaultContext context)
        {
            _context = context;
        }

        // GET: api/ProjTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjTask>>> GetProjTasks()
        {
            //show all project tasks that belong to current user
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.ProjTasks.TakeWhile(pt =>
                            pt.Users == _context.Users
                                .Where(u => u.Id == userid)
                         ).ToListAsync();
        }

        // GET: api/ProjTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjTask>> GetProjTask(int id)
        {
            var projTask = await _context.ProjTasks.FindAsync(id);
            string currentUserid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // :skull:
            if (projTask.Users.Where(u => u.Id == currentUserid) == null)
                return Unauthorized();

            if (projTask == null)
            {
                return NotFound();
            }
            return projTask;
        }

        // PUT: api/ProjTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjTask(int id, ProjTask projTask)
        {
            if (id != projTask.Id)
            {
                return BadRequest();
            }

            _context.Entry(projTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjTaskExists(id))
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

        // POST: api/ProjTasks
        //[Authorize(Policy = "Team Leader")]
        [HttpPost]
        public async Task<ActionResult<ProjTask>> PostProjTask(ProjTask projTask)
        {
            _context.ProjTasks.Add(projTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjTask", new { id = projTask.Id }, projTask);
        }

        // DELETE: api/ProjTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjTask(int id)
        {
            string currentUserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var projTask = await _context.ProjTasks.FindAsync(id);
            if (projTask == null)
            {
                return NotFound();
            }

            if (projTask.Users.Where(u => u.Id == currentUserid) == null)
                return Unauthorized();

            _context.ProjTasks.Remove(projTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjTaskExists(int id)
        {
            return _context.ProjTasks.Any(e => e.Id == id);
        }
    }
}
