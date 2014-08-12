using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BootstrapMvcSample.Controllers;
using Galeria.Web.Controllers;
using NavigationRoutes;

namespace BootstrapMvcSample
{
    public class ExampleLayoutsRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute<ExampleLayoutsController>("Options", c => c.Starter())
                .AddChildRoute<AccountProfileController>("View Profile", c => c.Profile())
                .AddChildRoute<DiskController>("My Files", c => c.ListAllContentRoot())
                .AddChildRoute<ChangePasswordController>("Change Password",p=>p.ChangePassword())
                .AddChildRoute<AccountController>("Log Out",a=>a.LogOut());

            
        }
    }
}
