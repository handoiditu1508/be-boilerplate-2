using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Hamburger.Helpers.Comparers
{
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y) => x.Type == y.Type && x.Value == y.Value;

        public int GetHashCode([DisallowNull] Claim obj) => base.GetHashCode();
    }
}
