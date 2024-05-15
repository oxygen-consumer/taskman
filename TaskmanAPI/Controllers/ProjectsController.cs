using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<Project>> Show(int id)
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
            project.ProjectOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            //creez un rol de admin pentru creatorul proiectului
            RolePerProject NewAdminRole = new RolePerProject(project.ProjectOwner, project.Id, "Admin");
            _context.RolePerProjects.Add(NewAdminRole);
            await _context.SaveChangesAsync();

            //adaug noul rol in bd
            project.RolePerProjects.Add(NewAdminRole);
            
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
                && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && rp.RoleName == "Admin");

            //verif daca userul curent are acces la proiect
            if (!privilege.Any())
                return Forbid();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
