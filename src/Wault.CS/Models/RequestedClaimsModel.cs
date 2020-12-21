using System.Collections.Generic;

namespace Wault.CS.Models
{
    public class RequestedClaimsModel
    {
        public string TrackId { get; set; }
        public IEnumerable<ClaimModel> Claims { get; set; }
    }
}
