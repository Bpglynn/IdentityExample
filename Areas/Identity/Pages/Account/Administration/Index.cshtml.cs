using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration
{
    [Authorize(Policy = "PolicyAdmin")]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<ApplicationUser> Users;
        public List<IdentityRole> Roles;

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            Users = _userManager.Users.ToList();
            Roles = _roleManager.Roles.ToList();
            if (Users == null)
            {
                return NotFound($"Unable to load users from database.");
            }
            return Page();
        }
    }
}