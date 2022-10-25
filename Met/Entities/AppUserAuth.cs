using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Met.Entities
{
    public class AppUserAuth
    {
        public AppUserAuth()
        {
            UserId = string.Empty;
            UserName = string.Empty;

            BearerToken = string.Empty;

            IsAuthenticated = false;

            Claims = new List<Claim>();
        }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("bearerToken")]
        public string BearerToken { get; set; }

        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonProperty("claims")]
        public List<Claim> Claims { get; set; }
    }
}
