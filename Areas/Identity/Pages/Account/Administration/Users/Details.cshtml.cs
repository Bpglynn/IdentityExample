using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration.Users
{
    [Authorize(Policy = "PolicyAdmin")]
    public partial class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string UserId; 
        public ApplicationUser UserDetails;
        public List<IdentityRole> UserRoles = new List<IdentityRole>();

        public DetailsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            string UserId = id; //UID from URI
            UserDetails = await _userManager.FindByIdAsync(UserId);
            if (UserDetails == null)
            {
                return NotFound($"Unable to load user from database.");
            }
            UserRoles = await GetRolesForUser(UserDetails); // Populate user roles
            return Page();
        }
        public async Task<List<IdentityRole>> GetRolesForUser(ApplicationUser user)
        {
            // Takes an ApplicationUser object and returns list of IdentityRole objects assigned to the user
            List<IdentityRole> UserRoles = new List<IdentityRole>();
            foreach (var role in _roleManager.Roles)
            {
                if (role != null && await _userManager.IsInRoleAsync(user, role.Name))
                    //UserRoleList.Add(role);
                    UserRoles.Add(role);
            }
            return UserRoles;
        }
    }
}