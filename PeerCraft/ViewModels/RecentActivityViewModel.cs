using System;

namespace PeerCraft.ViewModels
{
    public class RecentActivityViewModel
    {
        public string Message { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime ActivityDate { get; set; }
        public string LinkUrl { get; set; }
        public string IconClass { get; set; } // e.g., "icon-comment" or "icon-task"
    }
}

