using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration.Roles
{
    [Authorize(Policy = "PolicyAdmin")]
    public partial class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateModel> _logger;

        public string RoleId;
        public IdentityRole RoleDetails;
        public List<ApplicationUser> members;
        public List<ApplicationUser> nonMembers;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<CreateModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [MinLength(3)]
            [MaxLength(32)]
            public string Name { get; set; }
            public string[] AddUsers { get; set; }
            public string[] RemoveUsers { get; set; }
        }

        [TempData]
        public string ErrorMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            string RoleId = id; //UID from URI
            RoleDetails = await _roleManager.FindByIdAsync(RoleId);
            if (RoleDetails == null)
            {
                return NotFound($"Unable to load role from database.");
            }
            //UserList = await GetUsersForRole(RoleDetails); // Get users in role
            members = new List<ApplicationUser>();
            nonMembers = new List<ApplicationUser>();
            foreach (ApplicationUser user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, RoleDetails.Name) ? members : nonMembers;
                list.Add(user);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            string RoleId = id; //UID from URI
            RoleDetails = await _roleManager.FindByIdAsync(RoleId);
            if (RoleDetails == null)
            {
                return NotFound($"Unable to load user from database.");
            }
            if (ModelState.IsValid)
            {
                RoleDetails.Name = Input.Name;
                var result = await _roleManager.UpdateAsync(RoleDetails);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(Input.Name, "Failed to update database.");
                    return Page();
                }
                _logger.LogInformation($"Role edited: {RoleDetails.Id} named {RoleDetails.Name}.");
                foreach (string userId in Input.AddUsers ?? new string[] { })
                //foreach (string userId in Input.AddUsers)
                    {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, RoleDetails.Name);
                        if (!result.Succeeded)
                        {
                            _logger.LogError($"Failed to add user to role: {user.Id} ({user.UserName}) added to {RoleDetails.Id}({RoleDetails.Name}).");
                            ErrorMessage = result.ToString();
                        }
                        _logger.LogInformation($"User added to role: {user.Id} ({user.UserName}) added to {RoleDetails.Id}({RoleDetails.Name}).");
                    }
                }
                foreach (string userId in Input.RemoveUsers ?? new string[] { })
                //foreach (string userId in Input.RemoveUsers)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, RoleDetails.Name);
                        if (!result.Succeeded)
                        {
                            _logger.LogError($"Failed to remove user from role: {user.Id} ({user.UserName}) added to {RoleDetails.Id}({RoleDetails.Name}).");
                            ErrorMessage = result.ToString();
                        }
                        _logger.LogInformation($"User removed from role: {user.Id} ({user.UserName}) removed from {RoleDetails.Id}({RoleDetails.Name}).");
                    }
                }
                return RedirectToPage("../Index");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        /*
        public async Task<List<ApplicationUser>> GetUsersForRole(IdentityRole role)
        {
            // Takes an IdentityRole object and returns list of ApplicationUser objects assigned to the role
            List<ApplicationUser> UserList = new List<ApplicationUser>();
            foreach (var user in _userManager.Users)
            {
                if (user != null && await _userManager.IsInRoleAsync(user, role.Name))
                    UserList.Add(user);
            }
            return UserList;
        }
        */
    }
}