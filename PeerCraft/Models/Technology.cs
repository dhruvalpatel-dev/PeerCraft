using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class Technology
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // This links technologies to many projects
        public virtual ICollection<ProjectTechnology> ProjectTechnologies { get; set; }
    }
}