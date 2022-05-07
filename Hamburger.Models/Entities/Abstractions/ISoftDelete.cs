using System;

namespace Hamburger.Models.Entities.Abstractions
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
