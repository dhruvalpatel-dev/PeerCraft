using PeerCraft.Models;
using System.Collections.Generic;

namespace PeerCraft.ViewModels
{
    public class DiscoverViewModel
    {
        public IEnumerable<Project> Projects { get; set; }
        public List<SelectableTechnologyViewModel> AvailableTechnologies { get; set; }
        public int[] SelectedTechnologies { get; set; }
    }
}
