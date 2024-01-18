using System.ComponentModel.DataAnnotations;
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
        public List<Lobby> Lobbies { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Map> Maps { get; set; } = new List<Map>();
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public List<Item> GameItems { get; set; } = new List<Item>();
    }
}