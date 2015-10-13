using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace towerDefense
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Start",
                url: "Game/Start",
                defaults: new { controller = "Game", action = "Start" }
            );


            routes.MapRoute(
                name: "Stop",
                url: "Game/StopGame",
                defaults: new { controller = "Game", action = "StopGame" }
            );


            routes.MapRoute(
                name: "UploadFile",
                url: "Game/UploadFile",
                defaults: new { controller = "Game", action = "UploadFile" }
            );

            routes.MapRoute(
                name: "DeleteTank",
                url: "Game/DeleteTank",
                defaults: new { controller = "Game", action = "DeleteTank" }
            );

            routes.MapRoute(
                name: "GameName",
                url: "Game/{gameName}",
                defaults: new { controller = "Game", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
