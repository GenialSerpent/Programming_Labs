using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects(
            [FromQuery(Name = "filter")] string filterValue,
            [FromQuery(Name = "sort")] string sortColumn,
            [FromQuery(Name = "order")] string sortOrder,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "pageSize")] int pageSize = 10)
        {
            IQueryable<Project> query = _context.Projects;

            if (!string.IsNullOrEmpty(filterValue))
            {
                query = query.Where(p =>
                    p.Author.Contains(filterValue) ||
                    p.Budget.ToString().Contains(filterValue) ||
                    p.Score1.ToString().Contains(filterValue) ||
                    p.Score2.ToString().Contains(filterValue) ||
                    p.Score3.ToString().Contains(filterValue));
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.ToLower() == "asc")
                {
                    query = query.OrderBy(p => EF.Property<object>(p, sortColumn));
                }
                else
                {
                    query = query.OrderByDescending(p => EF.Property<object>(p, sortColumn));
                }
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            int skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            Response.Headers.Add("X-Total-Items", totalItems.ToString());
            Response.Headers.Add("X-Total-Pages", totalPages.ToString());
            Response.Headers.Add("X-Current-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
        {
            if (!ValidateProject(project))
            {
                return BadRequest();
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (!ValidateProject(project))
            {
                return BadRequest();
            }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(p => p.Id == id);
        }

        private bool ValidateProject(Project project)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(project, null, null);
            bool isValid = Validator.TryValidateObject(project, validationContext, validationResults, true);
            return isValid;
        }
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Z][a-z]+( [A-Z][a-z]+)*$")]
        public string Author { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score1 { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score2 { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score3 { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
    }
}
