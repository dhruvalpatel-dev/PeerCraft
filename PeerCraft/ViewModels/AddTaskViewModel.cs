using System;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.ViewModels
{
    public class AddTaskViewModel
    {
        public int ProjectId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "Assign To")]
        public int? AssigneeId { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
    }
}