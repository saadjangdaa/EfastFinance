using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RMS.Web.Models;
using RMS.Web.Services;

namespace RMS.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        MVCDBLiveEntities db = new MVCDBLiveEntities();
        MasterServices setupServices = new MasterServices();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value;   
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {

         
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true 

            Session["username"] = model.Email;
            var email = model.Email;

            var userid = db.AspNetUsers.Where(m => m.UserName == email).FirstOrDefault().UserID;

            Session["userid"] = userid;

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal("~/Home/Index");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }


            }
            catch (Exception e)
            {
                ViewBag.message = e.ToString();
                return View("~/Views/Shared/Error.cshtml");

            }
        }



        public ActionResult UserPrivilege(int UserId = 0)
        {
            MasterServices setupServices = new MasterServices();
            var userContext = db.AspNetUsers.ToList();
            UserPrivilegeViewModel model = new UserPrivilegeViewModel();
            if (UserId == 0)
            {
                model.MenuItems = setupServices.GetAllMenus();
                model.UserDetails = userContext;
                model.ExistingAssignedMenuIds = null;
                model.SelectedUserId = 0;

            }
            else
            {
                model.MenuItems = setupServices.GetAllMenus();
                model.UserDetails = userContext;
                model.ExistingAssignedMenuIds = setupServices.GetUserExistingMenus(UserId);
                model.SelectedUserId = UserId;
            }

            return PartialView(model);
        }

        // POST: /Account/UserPrivilege
        [HttpPost]
        public ActionResult UserPrivilege(AddUserPrivilegeModel model)
        {
            MasterServices setupServices = new MasterServices();
            if ( model != null )
            {
                if (model.UserId != 0 &&   model.MenuIds.Count > 0)
                {
                    //if (string.IsNullOrEmpty(model.Replace))
                    //{
                    //    var isAlreadyExisit = setupServices.isExistUserPrivilege(model.UserId);
                    //    if (isAlreadyExisit)
                    //        return Content("This User Priviliges already Exists");
                    //}
                    //else
                    //{
                    //}
                    setupServices.DeleteExisitingUserPrivileges(model.UserId);

                    setupServices.AddParentMenusInPrivileges(model.UserId, model.MenuIds);
                    foreach (var id in model.MenuIds)
                    {
                        var userPrivilege = new UsersMenu
                        {
                            MenuItemID = (int)id,
                            UserID = model.UserId
                        };

                        setupServices.AddUserPrivilege(userPrivilege);
                    }
                }
                else
                {
                    return Content("UserId OR MenuId is invalid");
                }
            }
            return Content("success");
        }


        //GET: /Account/VerifyCode


       [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/UserDetails
        public ActionResult UserDetails(int UserID = 0)
        {
            UserPrivilegeViewModel model = new UserPrivilegeViewModel();
            model.registerViewModel = new RegisterViewModel();

            var userContext = db.AspNetUsers.ToList();
            model.registerViewModel.Menulists = db.MenuItems.ToList();
            ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName");


            if (UserID != 0)
            {
                var user = db.AspNetUsers.Where(m => m.UserID == UserID).FirstOrDefault();
                model.registerViewModel.Userid = user.UserID;
                model.registerViewModel.UserName = user.UserName;
                model.registerViewModel.PhoneNumber = user.PhoneNumber;
                model.registerViewModel.Email = user.Email;
                model.registerViewModel.locationid = user.LocationID;
                ViewBag.MaterialCentres = new SelectList(db.MaterialCentre, "MaterialcentreID", "MaterialCentreName", user.LocationID);

            }

            if (UserID == 0)
            {
                model.MenuItems = setupServices.GetAllMenus();
                model.UserDetails = userContext;
                model.ExistingAssignedMenuIds = null;
                model.SelectedUserId = 0;

            }
            else
            {
                model.MenuItems = setupServices.GetAllMenus();
                model.UserDetails = userContext;
                model.ExistingAssignedMenuIds = setupServices.GetUserExistingMenus(UserID);
                model.SelectedUserId = UserID;
            }

            return View(model);
        }

        //
        // POST: /Account/UserDetails
        [HttpPost]
        public async Task<ActionResult> UserDetails(UserPrivilegeViewModel model)
        {
            try
            {
                AspNetUsers usermodel = new AspNetUsers();

                if (model.registerViewModel.Userid > 0)
                {
                    setupServices.SaveUser(model);
                }
                else
                {

                    if (ModelState.IsValid)
                    {
                        var user = new ApplicationUser { UserName = model.registerViewModel.UserName, Email = model.registerViewModel.Email };
                        var result = await UserManager.CreateAsync(user, model.registerViewModel.Password);
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


                            var userid = db.AspNetUsers.Where(m => m.Email == model.registerViewModel.Email).FirstOrDefault().UserID;
                            var menucount = db.MenuItems.ToList();
                            for (int i = 0; i < menucount.Count; i++)
                            {

                                var menuid = menucount[i].ID;
                                UsersMenu userm = new UsersMenu();
                                userm.UserID = (int)userid;
                                userm.MenuItemID = menuid;
                                userm.AllowUpdate = false;
                                userm.AllowRead = false;
                                userm.AllowDelete = false;
                                userm.AllowInsert = false;
                                userm.IsActive = false;

                                db.UsersMenu.Add(userm);
                                db.SaveChanges();

                            }

                            ViewBag.message = "User Added Successfully";
                            return View();
                        }
                        AddErrors(result);
                        if (result.Errors != null)
                        {
                            ViewBag.message = result.Errors.FirstOrDefault();
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception e)
            {

                ViewBag.message = e.ToString();
                return View("/Views/Shared/Error.cshtml");
                throw;
            }

        }



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword 
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id); 
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme); 
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");  
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback 
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login 
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account 
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["username"] = "";

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //------------------------------------Manage Users---------------------------------//
        public ActionResult UsersList()
        {
            var users = db.AspNetUsers.ToList();

            //AspNetUsers users2 = new AspNetUsers(); 
            

            return View(users);
        } 
        public ActionResult EditUser(int id)
        {
            RegisterViewModel model = new RegisterViewModel();

            model.Users = db.AspNetUsers.Where(m => m.UserID == id).FirstOrDefault();
            model.Usersmenu = db.UsersMenu.Where(m => m.UserID == id).ToList();

            return View("/Views/Account/Register.cshtml", model);
        }

            


    }
}