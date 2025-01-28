using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MinhasFinancas.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "PorCodigoAcao",
                url: "Acao/{codigo}",
                defaults: new { controller = "Papel", action = "Detalhes", codigo = "" }
            );

            routes.MapRoute(
                name: "PorCodigoFII",
                url: "FII/{codigo}",
                defaults: new { controller = "Papel", action = "Detalhes", codigo = "" }
            );

            routes.MapRoute(
                name: "PorCodigoBDR",
                url: "BDR/{codigo}",
                defaults: new { controller = "Papel", action = "Detalhes", codigo = "" }
            );

            routes.MapRoute(
                name: "PorCodigoETF",
                url: "ETF/{codigo}",
                defaults: new { controller = "Papel", action = "Detalhes", codigo = "" }
            );

            routes.MapRoute(
                name: "Normal",
                url: "Papel/{action}",
                defaults: new { controller = "Papel", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
