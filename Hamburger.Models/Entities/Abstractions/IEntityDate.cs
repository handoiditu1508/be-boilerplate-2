using System;

namespace Hamburger.Models.Entities.Abstractions
{
    public interface IEntityDate
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
