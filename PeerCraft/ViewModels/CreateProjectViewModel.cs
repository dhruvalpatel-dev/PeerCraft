using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.ViewModels
{
    public class CreateProjectViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "GitHub Link (Optional)")]
        [Url]
        public string GitHubLink { get; set; }

        [Display(Name = "Team Members (Optional)")]
        [DataType(DataType.MultilineText)]
        public string TeamMembers { get; set; }

        public List<SelectableTechnologyViewModel> AvailableTechnologies { get; set; }
    }
}
