using PeerCraft.Models;
using System.Collections.Generic;

namespace PeerCraft.ViewModels
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public IEnumerable<Project> OwnedProjects { get; set; }
    }
}
