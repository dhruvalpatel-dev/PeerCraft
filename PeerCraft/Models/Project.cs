using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string GitHubLink { get; set; }

        // Each project is owned by one user
        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        // A project can have many tasks
        public virtual ICollection<ProjectTask> Tasks { get; set; }

        // A project can have many team members
        public virtual ICollection<TeamMember> TeamMembers { get; set; }

        // A project can use many technologies
        public virtual ICollection<ProjectTechnology> ProjectTechnologies { get; set; }

        // A project can have many comments
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

