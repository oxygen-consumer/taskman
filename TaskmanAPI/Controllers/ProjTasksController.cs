using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Model;

/* TO DO
 * Test
 * Make the edit method
 * Make Assign User method
 */

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

        // GET: api/ProjTasks/{projectId}
        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjTask>>> GetProjTasks(int projectId)
        {
            //show all project tasks that belong to current user and are in the projectId project
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = new List<ProjTask>();
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == projectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) 
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            foreach (var t in _context.ProjTasks.Where(t => t.ProjectId == projectId)) 
            {
                if (t.Users.Any(u => u.Id == userid)) 
                { 
                    tasks.Add(t);
                    continue;
                }
                //if user is admin or owner then they can see task anyway
                if(privilege.Any())
                {
                    tasks.Add(t);
                }  
            }

            return tasks;
        }

        // GET: api/ProjTasks/{projectId}/{taskid}
        [HttpGet("{projectId}/{id}")]
        public async Task<ActionResult<ProjTask>> GetProjTask(int projectId, int id)
        {
            var projTask = await _context.ProjTasks.FindAsync(id);
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == projectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            if (projTask == null)
            {
                return NotFound();
            }

            //assigned users, admins and the owner can see task
            if (projTask.Users.Any(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    || privilege.Any())
            {
                return projTask;
            }

            return Forbid();
        }

        // PUT: api/ProjTasks/5
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
        [HttpPost]
        public async Task<ActionResult<ProjTask>> New(ProjTask projTask)
        {
            //can add privilege check here or in projects where we call new
            projTask.Project = _context.Projects.Find(projTask.ProjectId);
            projTask.Users = AssignUsers(projTask); //???
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

            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == projTask.ProjectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            if (!privilege.Any())
                return Forbid();

            _context.ProjTasks.Remove(projTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjTaskExists(int id)
        {
            return _context.ProjTasks.Any(e => e.Id == id);
        }

        //dummy for now
        private ICollection<User> AssignUsers(ProjTask ptask)
        {
            var users = new List<User>();
            return users;
        }
    }
}
