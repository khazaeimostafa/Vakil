using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Met.DTOs
{

    [ValidateNever]
    public class RegisterVM
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }


        [Required]
        public string EmailAddress { get; set; }


        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }


    }
}
