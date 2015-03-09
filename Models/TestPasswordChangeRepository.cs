using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcNextMatch.Models {
   
   public class TestPasswordChangeRepository : PasswordChangeRepository {
      
      static readonly IDictionary<int, ICollection<PasswordChange>> passwordChanges =
         new Dictionary<int, ICollection<PasswordChange>>();

      public override ICollection<PasswordChange> GetLastFourPasswords(int userId) {

         ICollection<PasswordChange> userPasswordChanges;

         if (!passwordChanges.TryGetValue(userId, out userPasswordChanges)) {
            return new PasswordChange[0];
         }

         return userPasswordChanges.OrderByDescending(p => p.CreatedOn)
            .Take(4)
            .ToList();
      }

      public override void PasswordChanged(PasswordChange passwordChange) {

         ICollection<PasswordChange> userPasswordChanges;

         if (!passwordChanges.TryGetValue(passwordChange.UserId, out userPasswordChanges)) {
            
            userPasswordChanges = new List<PasswordChange>();
            passwordChanges[passwordChange.UserId] = userPasswordChanges;
         }

         userPasswordChanges.Add(passwordChange);
      }
   }
}