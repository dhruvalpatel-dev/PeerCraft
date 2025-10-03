using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerCraft.Data;
using PeerCraft.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PeerCraft.Controllers
{
    [Authorize]
    public class DiscoverController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscoverController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Discover
        public async Task<IActionResult> Index(int[] selectedTechnologies)
        {
            // Base query to get all projects with their related data
            var projectsQuery = _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
                .AsQueryable();

            // If any technology filters are selected, apply them
            if (selectedTechnologies != null && selectedTechnologies.Any())
            {
                // Filter projects to include only those that have at least one of the selected technologies
                projectsQuery = projectsQuery.Where(p => p.ProjectTechnologies.Any(pt => selectedTechnologies.Contains(pt.TechnologyId)));
            }

            // Fetch all available technologies to display as filter options
            var allTechnologies = await _context.Technologies.OrderBy(t => t.Name).ToListAsync();

            var viewModel = new DiscoverViewModel
            {
                Projects = await projectsQuery.OrderByDescending(p => p.Id).ToListAsync(),
                AvailableTechnologies = allTechnologies.Select(t => new SelectableTechnologyViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    // Mark as selected if its ID is in the submitted filter array
                    IsSelected = selectedTechnologies != null && selectedTechnologies.Contains(t.Id)
                }).ToList(),
                SelectedTechnologies = selectedTechnologies
            };

            return View(viewModel);
        }
    }
}