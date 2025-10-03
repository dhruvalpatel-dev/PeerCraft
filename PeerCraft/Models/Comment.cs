using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The project this comment is for
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        // The user who wrote the comment
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        // A comment can have many replies
        public virtual ICollection<Reply> Replies { get; set; }
    }
}

