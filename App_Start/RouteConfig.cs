using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace alharamApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("account/OnlySecritOperation010");
            //routes.IgnoreRoute("account/adminOperations");


            routes.MapRoute(
             name: "defaults",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "alharam", action = "Index", id = UrlParameter.Optional });

        }
    }
}
