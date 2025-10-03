namespace PeerCraft.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        // Each team member entry links to one project
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        // Each team member entry links to one user
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}