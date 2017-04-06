using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppBasic.Configurations
{
    public class SomeConfiguration
    {
        public string APIBaseUrl { get; set; } = "https://graph.facebook.com";
        public string AdminAccessToken { get; set; }

        public string CommunityMembersPath => $"/{CommunityId}/members";

        public string CommunityId { get; set; }
    }
}
