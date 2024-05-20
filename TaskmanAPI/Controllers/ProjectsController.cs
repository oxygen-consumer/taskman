﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using TaskmanAPI.Contexts;
using TaskmanAPI.Models;

namespace TaskmanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : Controller
    {
        private readonly DefaultContext _context;

        public ProjectsController(DefaultContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var proles = _context.RolePerProjects.Where(rp => rp.UserId == userId).ToList();
            var projects = new List<Project>();
            var projectids = new List<int>();

            //extrag id-urile proiectelor in care userul are roluri
            foreach (var p in proles)
            {
                if(p.ProjectId != null)
                {
                    if(!projects.Any(proj => proj.Id == p.ProjectId))
                        projectids.Add((int)p.ProjectId);
                }
            }
  
            foreach (var id in projectids)
            {
                var project = _context.Projects.Where(p => p.Id == id).First();
                projects.Add(project);
            }
            return projects;
        }
        

        // GET: api/Projects
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }*/

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            //verific daca userul curent are acces la proiect
            if(_context.RolePerProjects.Any(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))) 
            {
                return project;
            }

            return Forbid();
        }

       
        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            //verif daca userul curent are acces la proiect
            if (!_context.RolePerProjects.Any(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Forbid();

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> New(Project project)
        {
            var ProjectOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // check if user exists
            if (!_context.Users.Any(u => u.Id == ProjectOwner))
                return BadRequest("User does not exist");
            
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            //creez un rol de owner pentru creatorul proiectului
            RolePerProject NewOwnerRole = new RolePerProject(ProjectOwner, project.Id, "Owner");
            _context.RolePerProjects.Add(NewOwnerRole);
            await _context.SaveChangesAsync();

            //adaug noul rol in bd
            project.RolePerProjects.Add(NewOwnerRole);
            
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }
        
        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && rp.RoleName == "Owner");

            //verif daca userul curent are acces la proiect
            if (!privilege.Any())
                return Forbid();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // add another user to project
        [HttpPost("{id}/adduser/{user_id}")]
        public async Task<ActionResult<Project>> AddUser(int id, string user_id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // check if user has privilege to add another user
            // TODO: we should move the privilege check to a separate function (maybe even a service)
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));
            if (!privilege.Any())
                return Forbid();

            RolePerProject NewUserRole = new RolePerProject(user_id, id, "User");
            _context.RolePerProjects.Add(NewUserRole);
            await _context.SaveChangesAsync();

            project.RolePerProjects.Add(NewUserRole);
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }
        
        // remove user from project
        [HttpDelete("{id}/removeuser/{user_id}")]
        public async Task<ActionResult<Project>> RemoveUser(int id, string user_id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // check if user has privilege to remove another user
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (rp.RoleName == "Owner" || rp.RoleName == "Admin"));
            if (!privilege.Any())
                return Forbid();
            
            var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
            if (!userRole.Any())
                return NotFound();
            
            _context.RolePerProjects.Remove(userRole.First());
            await _context.SaveChangesAsync();
            return project;
        }
        
        // promote user to admin
        [HttpPost("{id}/promoteuser/{user_id}")]
        public async Task<ActionResult<Project>> PromoteUser(int id, string user_id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // check if user has privilege to promote another user
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && rp.RoleName == "Owner");
            if (!privilege.Any())
                return Forbid();
            
            var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
            if (!userRole.Any())
                return NotFound();
            
            userRole.First().RoleName = "Admin";
            _context.Entry(userRole.First()).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }
        
        // transfer ownership
        [HttpPost("{id}/transferownership/{user_id}")]
        public async Task<ActionResult<Project>> TransferOwnership(int id, string user_id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // check if user has privilege to transfer ownership
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && rp.RoleName == "Owner");
            if (!privilege.Any())
                return Forbid();
            
            var newOwnerRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
            if (!newOwnerRole.Any())
                return NotFound();
            
            var oldOwnerRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!oldOwnerRole.Any())
                return NotFound();
            
            oldOwnerRole.First().RoleName = "Admin";
            newOwnerRole.First().RoleName = "Owner";
            _context.Entry(oldOwnerRole.First()).State = EntityState.Modified;
            _context.Entry(newOwnerRole.First()).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }
        
        // demote user to regular user
        [HttpPost("{id}/demoteuser/{user_id}")]
        public async Task<ActionResult<Project>> DemoteUser(int id, string user_id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            // check if user has privilege to demote another user
            var privilege = _context.RolePerProjects.Where(rp => rp.ProjectId == id
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && rp.RoleName == "Owner");
            if (!privilege.Any())
                return Forbid();
            
            var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
            if (!userRole.Any())
                return NotFound();
            
            userRole.First().RoleName = "User";
            _context.Entry(userRole.First()).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
