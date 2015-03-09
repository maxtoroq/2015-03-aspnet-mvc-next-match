using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcNextMatch {
   
   public static class NextMatchExtensions {

      public static ActionResult NextMatch(this ControllerBase controller) {

         RequestContext context = controller.ControllerContext.RequestContext;

         RouteData nextRouteData = NextRouteData(context);

         if (nextRouteData == null) {
            return null;
         }

         var nextContext = new RequestContext(context.HttpContext, nextRouteData);

         return InvokeAction(controller, nextContext);
      }

      public static RouteData NextRouteData(RequestContext context) {

         var allRoutes = RouteTable.Routes;

         var nextRoutes = new RouteCollection {
            AppendTrailingSlash = allRoutes.AppendTrailingSlash,
            LowercaseUrls = allRoutes.LowercaseUrls,
            RouteExistingFiles = allRoutes.RouteExistingFiles
         };

         foreach (var route in allRoutes
            .SkipWhile(r => !Object.ReferenceEquals(r, context.RouteData.Route))
            .Skip(1)) {

            nextRoutes.Add(route);
         }

         return nextRoutes.GetRouteData(context.HttpContext);
      }

      public static ActionResult InvokeAction(ControllerBase currentController, RequestContext context) {

         RouteData routeData = context.RouteData;

         ControllerBase nextController = (ControllerBase)ControllerBuilder.Current
            .GetControllerFactory()
            .CreateController(context, routeData.GetRequiredString("controller"));

         nextController.ViewData = currentController.ViewData;

         nextController.GetType()
            .GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[1] { typeof(RequestContext) }, null)
            .Invoke(nextController, new object[1] { context });

         ControllerContext controllerContext = nextController.ControllerContext;

         var actionInvoker = new ControllerActionInvokerExposer();

         ControllerDescriptor controllerDescriptor = actionInvoker.GetControllerDescriptor(controllerContext);
         ActionDescriptor actionDescriptor = actionInvoker.FindAction(controllerContext, controllerDescriptor, routeData.GetRequiredString("action"));

         if (actionDescriptor == null) {
            throw new InvalidOperationException();
         }

         IDictionary<string, object> parameterValues = actionInvoker.GetParameterValues(controllerContext, actionDescriptor);

         return actionInvoker.InvokeActionMethod(controllerContext, actionDescriptor, parameterValues);
      }

      class ControllerActionInvokerExposer : ControllerActionInvoker {

         public new ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext) {
            return base.GetControllerDescriptor(controllerContext);
         }

         public new ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
            return base.FindAction(controllerContext, controllerDescriptor, actionName);
         }

         public new IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
            return base.GetParameterValues(controllerContext, actionDescriptor);
         }

         public new ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters) {
            return base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
         }
      }
   }
}