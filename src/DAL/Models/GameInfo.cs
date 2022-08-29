using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("GameInfo")]
    public class GameInfo
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [MaxLength(30), MinLength(1)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public int MaxPlayerCount { get; set; }

        [Required]
        public int MinPlayerCount { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }
    }
}
