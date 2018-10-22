using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace serwisSpolecznosciowy
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "InitUsers",
                url: "Init",
                defaults: new { controller = "User", action = "Init" });

            routes.MapRoute(
               name: "UserLogout",
               url: "Logout",
               defaults: new { controller = "Login", action = "Logout"});

            routes.MapMvcAttributeRoutes();



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            // routes.MapRoute(
            //    name: "UserLoggg",
            //    url: "Login/{login}",
            //    defaults: new { controller = "Users", action = "Login", login = "" }
            //);


        }
    }
}
