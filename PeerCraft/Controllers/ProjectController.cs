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
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            // Load the project and all its related data at once to avoid multiple database calls
            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
                .Include(p => p.TeamMembers).ThenInclude(tm => tm.User)
                .Include(p => p.Tasks).ThenInclude(t => t.Assignee)
                .Include(p => p.Comments).ThenInclude(c => c.Author)
                .Include(p => p.Comments).ThenInclude(c => c.Replies).ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var totalTasks = project.Tasks.Count;
            var completedTasks = project.Tasks.Count(t => t.IsCompleted);
            var progress = (totalTasks > 0) ? ((double)completedTasks / totalTasks) * 100 : 0;

            var viewModel = new ProjectDetailViewModel
            {
                Project = project,
                Technologies = project.ProjectTechnologies.Select(pt => pt.Technology),
                TeamMembers = project.TeamMembers.Select(tm => tm.User).OrderBy(u => u.Username),
                Tasks = project.Tasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.Id),
                ProgressPercentage = progress,
                IsCurrentUserTeamMember = project.TeamMembers.Any(tm => tm.UserId == currentUserId),
                AddTaskForm = new AddTaskViewModel { ProjectId = id },
                Comments = project.Comments.OrderByDescending(c => c.CreatedAt)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var allTechnologies = await _context.Technologies.ToListAsync();
            var model = new CreateProjectViewModel
            {
                AvailableTechnologies = allTechnologies.Select(t => new SelectableTechnologyViewModel { Id = t.Id, Name = t.Name, IsSelected = false }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model is not valid, return to the view so user can fix errors
                return View(model);
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var project = new Project
            {
                Title = model.Title,
                Description = model.Description,
                GitHubLink = model.GitHubLink,
                OwnerId = currentUserId
            };

            // Use a transaction to make sure all database changes succeed or fail together
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync(); // This generates the new Project ID

                // The project creator is always the first team member
                _context.TeamMembers.Add(new TeamMember { ProjectId = project.Id, UserId = currentUserId });

                foreach (var tech in model.AvailableTechnologies.Where(t => t.IsSelected))
                {
                    _context.ProjectTechnologies.Add(new ProjectTechnology { ProjectId = project.Id, TechnologyId = tech.Id });
                }

                if (!string.IsNullOrWhiteSpace(model.TeamMembers))
                {
                    var usernames = model.TeamMembers.Split(new[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
                    var usersToAdd = await _context.Users.Where(u => usernames.Contains(u.Username) && u.Id != currentUserId).ToListAsync();
                    foreach (var user in usersToAdd)
                    {
                        _context.TeamMembers.Add(new TeamMember { ProjectId = project.Id, UserId = user.Id });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Something went wrong while creating the project. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectTechnologies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Make sure only the project owner can edit the project
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (project.OwnerId != currentUserId)
            {
                return Forbid();
            }

            var allTechnologies = await _context.Technologies.ToListAsync();
            var projectTechnologyIds = new HashSet<int>(project.ProjectTechnologies.Select(pt => pt.TechnologyId));

            var viewModel = new EditProjectViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                GitHubLink = project.GitHubLink,
                AvailableTechnologies = allTechnologies.Select(t => new SelectableTechnologyViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsSelected = projectTechnologyIds.Contains(t.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProjectViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var projectToUpdate = await _context.Projects
                .Include(p => p.ProjectTechnologies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projectToUpdate == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (projectToUpdate.OwnerId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                projectToUpdate.Title = model.Title;
                projectToUpdate.Description = model.Description;
                projectToUpdate.GitHubLink = model.GitHubLink;

                // For technologies, the easiest way is to remove the old ones and add the new selection
                _context.ProjectTechnologies.RemoveRange(projectToUpdate.ProjectTechnologies);
                foreach (var techViewModel in model.AvailableTechnologies.Where(t => t.IsSelected))
                {
                    _context.ProjectTechnologies.Add(new ProjectTechnology { ProjectId = id, TechnologyId = techViewModel.Id });
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Project updated successfully!";
                return RedirectToAction("Details", new { id = id });
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (project.OwnerId != currentUserId)
            {
                return Forbid();
            }

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (project.OwnerId != currentUserId)
            {
                return Forbid();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project has been deleted.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask([Bind(Prefix = "AddTaskForm")] AddTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.ProjectId });
            }

            var task = new ProjectTask
            {
                Title = model.Title,
                ProjectId = model.ProjectId,
                AssigneeId = model.AssigneeId,
                DueDate = model.DueDate,
                IsCompleted = false
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTaskStatus(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = task.ProjectId });
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task != null)
            {
                var projectId = task.ProjectId;
                _context.ProjectTasks.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = projectId });
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamMember(int projectId, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Username cannot be empty.";
                return RedirectToAction("Details", new { id = projectId });
            }

            var userToAdd = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (userToAdd == null)
            {
                TempData["ErrorMessage"] = $"User '{username}' not found.";
            }
            else
            {
                var isAlreadyMember = await _context.TeamMembers.AnyAsync(tm => tm.ProjectId == projectId && tm.UserId == userToAdd.Id);
                if (isAlreadyMember)
                {
                    TempData["InfoMessage"] = $"User '{username}' is already a member of this project.";
                }
                else
                {
                    _context.TeamMembers.Add(new TeamMember { ProjectId = projectId, UserId = userToAdd.Id });
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"User '{username}' has been added to the team.";
                }
            }

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTeamMember(int projectId, int userId)
        {
            var teamMember = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.ProjectId == projectId && tm.UserId == userId);
            var project = await _context.Projects.FindAsync(projectId);

            // Make sure a user exists and is not the project owner before removing
            if (teamMember != null && project != null && project.OwnerId != userId)
            {
                var tasksToUnassign = await _context.ProjectTasks
                    .Where(t => t.ProjectId == projectId && t.AssigneeId == userId)
                    .ToListAsync();

                foreach (var task in tasksToUnassign)
                {
                    task.AssigneeId = null;
                }

                _context.TeamMembers.Remove(teamMember);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Team member has been removed. Their tasks are now unassigned.";
            }

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int projectId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToAction("Details", new { id = projectId });
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var comment = new Comment
            {
                Content = content,
                ProjectId = projectId,
                AuthorId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int projectId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (comment.AuthorId != currentUserId)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int commentId, int projectId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToAction("Details", new { id = projectId });
            }

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (comment.AuthorId != currentUserId)
            {
                return Forbid();
            }

            comment.Content = content;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int commentId, int projectId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Reply cannot be empty.";
                return RedirectToAction("Details", new { id = projectId });
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reply = new Reply
            {
                Content = content,
                CommentId = commentId,
                AuthorId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(int replyId, int projectId)
        {
            var reply = await _context.Replies.FindAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (reply.AuthorId != currentUserId)
            {
                return Forbid();
            }

            _context.Replies.Remove(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReply(int replyId, int projectId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Reply cannot be empty.";
                return RedirectToAction("Details", new { id = projectId });
            }

            var reply = await _context.Replies.FindAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (reply.AuthorId != currentUserId)
            {
                return Forbid();
            }

            reply.Content = content;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = projectId });
        }
    }
}