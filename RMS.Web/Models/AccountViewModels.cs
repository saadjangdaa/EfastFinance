using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class UserPrivilegeViewModel
    {
        public List<AspNetUsers> UserDetails { get; set; }
        public List<MenuItems> MenuItems { get; set; }
        public List<int> ExistingAssignedMenuIds { get; set; }
        public int SelectedUserId { get; set; }

        public RegisterViewModel registerViewModel { get; set; }
    }


    public class AddUserPrivilegeModel
    {
        public int UserId { get; set; }
        public List<int?> MenuIds { get; set; }
        public string Replace { get; set; }
    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int? locationid { get; set; }
        public int Userid{ get; set; }
        public string Id { get; set; }



        public List<MenuItems> Menulists { get; set; }

        public AspNetUsers Users { get; set; }
        public List<UsersMenu> Usersmenu { get; set; }

        public RegisterViewModel registerViewModel { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
