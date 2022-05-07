using System;
using System.ComponentModel.DataAnnotations;

namespace Hamburger.Models.Entities
{
    public class LoginSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string UserAgent { get; set; }
        public int UserId { get; set; }
    }
}
