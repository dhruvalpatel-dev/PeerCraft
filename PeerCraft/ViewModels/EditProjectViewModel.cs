using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.ViewModels
{
    public class EditProjectViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "GitHub Link (Optional)")]
        [Url]
        public string GitHubLink { get; set; }

        public List<SelectableTechnologyViewModel> AvailableTechnologies { get; set; }
    }
}
