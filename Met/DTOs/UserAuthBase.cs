using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Met.Entities;
using Newtonsoft.Json;

namespace Met.DTOs
{
    public class UserAuthBase
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonProperty("claims")]
        public List<Claim> Claims { get; set; }

        public AuthResultVM appUserAuth { get; set; }
    }
}
