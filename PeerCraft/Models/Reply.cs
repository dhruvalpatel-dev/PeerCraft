using System;
using System.ComponentModel.DataAnnotations;

namespace PeerCraft.Models
{
    public class Reply
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The parent comment this reply is for
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }

        // The user who wrote the reply
        public int AuthorId { get; set; } // foreign key
        public virtual User Author { get; set; }
    }
}

