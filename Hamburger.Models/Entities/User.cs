using Hamburger.Models.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hamburger.Models.Entities
{
    public class User : IdentityUser<int>, ISoftDelete, IEntityDate
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string FullName => MiddleName == null ? $"{LastName} {FirstName}" : $"{LastName} {MiddleName} {FirstName}";
    }
}
