using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAccount;
using MvcAccount.Authentication;
using MvcCodeRouting.Web.Mvc;
using MvcNextMatch.Models;

namespace MvcNextMatch.Controllers.Account.Authentication {

   public class AuthenticationController : Controller {

      // Short duration for testing purposes
      readonly TimeSpan passwordDuration = TimeSpan.FromMinutes(1);

      readonly AccountRepository accountRepo;
      readonly PasswordChangeRepository passwordChangeRepo;

      public AuthenticationController(AccountRepository repo, PasswordChangeRepository passwordChangeRepo) {
         
         this.accountRepo = repo;
         this.passwordChangeRepo = passwordChangeRepo;
      }

      [CustomRoute("~/Account/{action}")]
      [HttpGet]
      public ActionResult SignIn() {
         return this.NextMatch();
      }

      [CustomRoute("~/Account/{action}")]
      [HttpPost]
      public ActionResult SignIn(SignInInput input) {
         
         ActionResult nextResult = this.NextMatch();

         User user;
         PasswordChange lastPassword;

         if (this.ModelState.IsValid
            && (user = this.accountRepo.FindUserByName(input.Username) as User) != null
            && ((lastPassword = this.passwordChangeRepo.GetLastFourPasswords(user.Id).FirstOrDefault()) == null
               || lastPassword.CreatedOn.Add(this.passwordDuration) < DateTime.Now)) {

            return RedirectToAction("", "Password.Change", new { expired = 1 });
         }

         return nextResult;
      }
   }
}