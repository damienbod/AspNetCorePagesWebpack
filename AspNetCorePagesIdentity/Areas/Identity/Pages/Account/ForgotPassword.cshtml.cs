using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCorePagesIdentity.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace AspNetCorePagesIdentity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer _identityLocalizer;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, IStringLocalizerFactory factory)
        {
            _userManager = userManager;
            _emailSender = emailSender;

            var type = typeof(IdentityResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _identityLocalizer = factory.Create("IdentityResource", assemblyName.Name);
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "EMAIL_REQUIRED")]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email, _identityLocalizer["RESET_PASSWORD"],
                    _identityLocalizer["RESET_PASSWORD_EMAIL_TEXT", HtmlEncoder.Default.Encode(callbackUrl)]);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
