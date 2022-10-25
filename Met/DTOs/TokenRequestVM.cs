using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Met.DTOs
{
    public class TokenRequestVM
    {

        [Required]
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
