namespace PeerCraft.Models
{
    // This is the link between a Project and a Technology
    public class ProjectTechnology
    {
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int TechnologyId { get; set; }
        public virtual Technology Technology { get; set; }
    }
}

