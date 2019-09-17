using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace IdentityExample.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extended Properties
        [MinLength(2)]
        [MaxLength(32)]
        [PersonalData]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "Untitled";
        [MinLength(2)]
        [MaxLength(32)]
        [PersonalData]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "Untitled";
    }
}
