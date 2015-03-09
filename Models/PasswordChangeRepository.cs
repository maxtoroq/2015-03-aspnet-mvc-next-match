using System;
using System.Collections.Generic;

namespace MvcNextMatch.Models {

   public abstract class PasswordChangeRepository {

      public abstract void PasswordChanged(PasswordChange passwordChange);

      public abstract ICollection<PasswordChange> GetLastFourPasswords(int userId);
   }
}