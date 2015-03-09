using System;
using System.Collections.Generic;

namespace MvcNextMatch.Models {

   public class PasswordChange {

      public int UserId { get; set; }
      public string Password { get; set; }
      public DateTime CreatedOn { get; set; }
   }
}