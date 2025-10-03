using PeerCraft.Models;
using System.Collections.Generic;

namespace PeerCraft.ViewModels
{
    public class DashboardViewModel
    {
        // For the "Action Center"
        public IEnumerable<ProjectTask> MyOverdueTasks { get; set; }
        public IEnumerable<ProjectTask> MyUpcomingTasks { get; set; }

        // For the "Recent Activity" feed
        public IEnumerable<RecentActivityViewModel> RecentActivities { get; set; }

        // For the "Project Overview"
        public IEnumerable<Project> OwnedProjects { get; set; }
        public IEnumerable<Project> TeamProjects { get; set; }
    }
}

