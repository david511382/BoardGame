using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Required]
        public int ID { get; set; }

        [MaxLength(10), MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MaxLength(16), MinLength(4)]
        public string Username { get; set; }

        [Required]
        [MaxLength(64), MinLength(64)]
        public string Password { get; set; }

        [Required]
        public DateTime RegisterTime { get; set; }
    }
}
