using System;
using System.Collections.Generic;

namespace Wault.CS.Models
{
    public class WaultModel
    {
        public Guid EntityId { get; set; }

        public string OrganizationEmail { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationStatus { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public string UserType { get; set; }
    }
}
