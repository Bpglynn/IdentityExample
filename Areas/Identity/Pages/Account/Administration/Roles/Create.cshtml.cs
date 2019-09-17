using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace IdentityExample.Areas.Identity.Pages.Account.Administration.Roles
{
    [Authorize(Policy = "PolicyAdmin")]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(RoleManager<IdentityRole> roleManager, ILogger<CreateModel> logger)
        {
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
        }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(Input.Name));
                if (result.Succeeded)
                {
                    _logger.LogInformation($"New identity role created: {Input.Name}");
                    return RedirectToPage("../Index");
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}