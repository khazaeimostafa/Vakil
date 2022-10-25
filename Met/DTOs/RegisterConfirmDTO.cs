using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Met.DTOs
{
    public class RegisterConfirmDTO
    {

        public string Email { get; set; }

        public string Token { get; set; }

        public bool ShowConfirmedMessage { get; set; } = false;



    }
}
