using System;
using System.Linq;
using System.Web.Mvc;
using MvcAccount;
using MvcAccount.Password.Change;
using MvcAccount.Shared;
using MvcCodeRouting.Web.Mvc;
using MvcNextMatch.Models;

namespace MvcNextMatch.Controllers.Account.Password.Change {

   [Authorize]
   public class ChangeController : Controller {

      readonly AccountRepository accountRepo;
      readonly PasswordService passServ;
      readonly PasswordChangeRepository passwordChangeRepo;

      public ChangeController(AccountRepository repo, PasswordService passwordService, PasswordChangeRepository passwordChangeRepo) {
         
         this.accountRepo = repo;
         this.passServ = passwordService;
         this.passwordChangeRepo = passwordChangeRepo;
      }

      protected override void OnActionExecuting(ActionExecutingContext filterContext) {

         if (this.Request.QueryString["expired"] == "1") {
            this.ViewBag.Alert = "Your password has expired and must be changed.";
         }
      }

      [DefaultAction]
      [HttpGet]
      public ActionResult Change() {
         return this.NextMatch();
      }

      [HttpPost]
      public ActionResult Change(ChangeInput input, FormButton cancel) {

         User user;

         if (!cancel
            && this.ModelState.IsValid
            && (user = this.accountRepo.FindUserByName(this.User.Identity.Name) as User) != null
            && this.passServ.PasswordEquals(input.CurrentPassword, user.Password)) {

            if (this.passwordChangeRepo.GetLastFourPasswords(user.Id).Any(p => p.Password == input.NewPassword)) {

               this.ModelState.AddModelError("NewPassword", "The new password must be different to your last four passwords.");
               
               this.ViewData.Model = new ChangeViewModel(input);

               return View();

            } else {

               ActionResult nextResult = this.NextMatch();

               if (this.ModelState.IsValid) { // Password was changed

                  this.passwordChangeRepo.PasswordChanged(new PasswordChange {
                     UserId = user.Id,
                     CreatedOn = DateTime.Now,
                     Password = this.passServ.ProcessPasswordForStorage(input.NewPassword)
                  });
               }

               return nextResult;
            }
         }

         return this.NextMatch();
      }
   }
}