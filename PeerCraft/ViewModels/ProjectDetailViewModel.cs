using PeerCraft.Models;
using System.Collections.Generic;

namespace PeerCraft.ViewModels
{
    public class ProjectDetailViewModel
    {
        public Project Project { get; set; }
        public IEnumerable<Technology> Technologies { get; set; }
        public IEnumerable<User> TeamMembers { get; set; }
        public IEnumerable<ProjectTask> Tasks { get; set; }
        public double ProgressPercentage { get; set; }
        public bool IsCurrentUserTeamMember { get; set; }
        public AddTaskViewModel AddTaskForm { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}
