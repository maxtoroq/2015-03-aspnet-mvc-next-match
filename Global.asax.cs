using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting;
using Microsoft.Practices.Unity;

namespace MvcNextMatch {
   
   public class MvcApplication : System.Web.HttpApplication {

      static readonly IUnityContainer Container = new UnityContainer();

      void Application_Start() {

         AreaRegistration.RegisterAllAreas();
         ViewEngines.Engines.EnableCodeRouting();
         DependencyResolver.SetResolver(new UnityResolver(Container));

         RegisterDependencies(Container);
         RegisterRoutes(RouteTable.Routes);
      }

      void RegisterDependencies(IUnityContainer container) {

         container.RegisterType<MvcAccount.AccountRepository, Models.TestAccountRepository>();
         container.RegisterType<MvcAccount.PasswordService, MvcAccount.ClearTextPasswordService>();
         container.RegisterType<Models.PasswordChangeRepository, Models.TestPasswordChangeRepository>();
      }

      void RegisterRoutes(RouteCollection routes) {

         routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

         routes.MapCodeRoutes(typeof(Controllers.HomeController));

         routes.MapCodeRoutes(
            baseRoute: "Account",
            rootController: typeof(MvcAccount.AccountController),
            settings: new CodeRoutingSettings {
               EnableEmbeddedViews = true
            }
         );
      }
   }
}
