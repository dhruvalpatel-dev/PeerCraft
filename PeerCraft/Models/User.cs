using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        // A user can own many projects
        public virtual ICollection<Project> OwnedProjects { get; set; }

        // A user can be a member of many teams
        public virtual ICollection<TeamMember> TeamMemberships { get; set; }
    }
}
