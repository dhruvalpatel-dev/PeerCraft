using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerCraft.Data;
using PeerCraft.Models;
using PeerCraft.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerCraft.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get IDs of all projects the user is a part of
            var userProjectIds = await _context.TeamMembers
                .Where(tm => tm.UserId == currentUserId)
                .Select(tm => tm.ProjectId)
                .ToListAsync();

            // Get tasks assigned to the current user
            var allMyTasks = await _context.ProjectTasks
                .Where(t => t.AssigneeId == currentUserId && !t.IsCompleted)
                .Include(t => t.Project)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            var overdueTasks = allMyTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now);
            var upcomingTasks = allMyTasks.Except(overdueTasks);

            // Get recent activity from the user's projects
            var recentComments = await _context.Comments
                .Where(c => userProjectIds.Contains(c.ProjectId) && c.CreatedAt > DateTime.Now.AddDays(-7))
                .Include(c => c.Author)
                .Include(c => c.Project)
                .Select(c => new RecentActivityViewModel
                {
                    Message = $"<b>{c.Author.Username}</b> commented on",
                    ProjectTitle = c.Project.Title,
                    ActivityDate = c.CreatedAt,
                    LinkUrl = Url.Action("Details", "Project", new { id = c.ProjectId }),
                    IconClass = "icon-comment"
                })
                .ToListAsync();

            var recentTasksCompleted = await _context.ProjectTasks
                .Where(t => userProjectIds.Contains(t.ProjectId) && t.IsCompleted && t.AssigneeId.HasValue)
                .Include(t => t.Assignee)
                .Include(t => t.Project)
                .OrderByDescending(t => t.Id)
                .Take(10)
                .Select(t => new RecentActivityViewModel
                {
                    Message = $"<b>{t.Assignee.Username}</b> completed task '{t.Title}' in",
                    ProjectTitle = t.Project.Title,
                    ActivityDate = DateTime.Now, // A "CompletedAt" date could be added to the Task model
                    LinkUrl = Url.Action("Details", "Project", new { id = t.ProjectId }),
                    IconClass = "icon-task"
                })
                .ToListAsync();

            var recentActivities = recentComments.Concat(recentTasksCompleted)
                .OrderByDescending(a => a.ActivityDate)
                .Take(5);

            // Get project overview lists
            var allMyProjects = await _context.Projects
                .Where(p => userProjectIds.Contains(p.Id))
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var ownedProjects = allMyProjects.Where(p => p.OwnerId == currentUserId);
            var teamProjects = allMyProjects.Where(p => p.OwnerId != currentUserId);

            var viewModel = new DashboardViewModel
            {
                MyOverdueTasks = overdueTasks,
                MyUpcomingTasks = upcomingTasks,
                RecentActivities = recentActivities,
                OwnedProjects = ownedProjects,
                TeamProjects = teamProjects
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

