using System;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? DueDate { get; set; }

        // This task belongs to one project
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        // This task can be assigned to one user (or none)
        public int? AssigneeId { get; set; }
        public virtual User Assignee { get; set; }
    }
}