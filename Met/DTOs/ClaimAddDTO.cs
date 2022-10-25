using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Met.DTOs
{
    public class ClaimAddDTO
    {
        public string Email { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
