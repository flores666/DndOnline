﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndOnline.DataAccess.Objects
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(25)")]
        public string Name { get; set; }
        
        [Required]
        [Column(TypeName = "text")]
        public string PasswordHash { get; set; }

        public RefreshToken? RefreshToken { get; set; }
        public IEnumerable<Lobby> Lobbies { get; set; }
        public IEnumerable<Entity> Entities { get; set; } = new List<Entity>();
        public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();
    }
}