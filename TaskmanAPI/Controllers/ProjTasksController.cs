using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
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

        // GET: api/ProjTasks/getprojtask/{projectId}
        [HttpGet("getprojtask/{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjTask>>> GetProjTasks(int projectId)
        {
            //show all project tasks that belong to current user and are in the projectId project
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = new List<ProjTask>();
            var privilege = await _context.RolePerProjects.Where(rp => rp.ProjectId == projectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) 
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin")).ToListAsync();

            var projectTasks = await _context.ProjTasks.Where(t => t.ProjectId == projectId).ToListAsync();

            foreach (var t in projectTasks) 
            {
                if(t.Users == null) 
                { 
                    continue; 
                }

                if (t.Users.Any(u => u.Id == userid)) 
                { 
                    tasks.Add(t);
                    continue;
                }
                //if user is admin or owner then they can see task anyway
                else if(privilege.Any())
                {
                    tasks.Add(t);
                }  
            }

            return tasks;
        }

        // GET: api/ProjTasks/{taskid}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjTask>> GetProjTask(int id)
        {
            var projTask = await _context.ProjTasks.FindAsync(id);

            if (projTask == null)
            {
                return NotFound();
            }

            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == projTask.ProjectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            //assigned users, admins and the owner can see task
            if ((projTask.Users != null && projTask.Users.Any(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    || privilege.Any())
            {
                return projTask;
            }

            return Forbid();
        }

        // PUT: api/ProjTasks/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjTask>> PutProjTask(int id, ProjTask projTask)
        {
            if (id != projTask.Id)
            {
                return BadRequest();
            }

            //owner and admins can edit tasks
            if (!_context.RolePerProjects.Any(rp => rp.ProjectId == projTask.ProjectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin")))
                return Forbid();

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

            return Ok(projTask);
        }

        // POST: api/ProjTasks/{projId}
        [HttpPost("{projId}")]
        public async Task<ActionResult<ProjTask>> PostProjTask(int projId, ProjTask projTask)
        {
            var project = await _context.Projects.FindAsync(projId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == projId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            if (!privilege.Any())
                return Forbid();

            projTask.ProjectId = projId;
            projTask.Project = project;
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

        // api/ProjTasks/{id}/assignusers/{userid}
        [HttpPost("{id}/assignusers/{userid}")]
        public async Task<ActionResult<ProjTask>> AssignUsers(int id, string userid) 
        {
            var ptask = _context.ProjTasks.Find(id);
            var user = _context.Users.Find(userid);

            if(ptask == null)
                return NotFound();

            if(user == null)
                return BadRequest("User doesn't exist.");

            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == ptask.ProjectId
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));

            if(!privilege.Any()) 
                return Forbid();
            /*
            foreach (var uid in userIdsList) 
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == uid);
                users.Add(user);
            }*/

            ptask.Users.Add(user);
            _context.Entry(ptask).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return ptask;
        }
    }
}
