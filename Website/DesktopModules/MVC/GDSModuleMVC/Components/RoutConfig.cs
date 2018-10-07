using DotNetNuke.Web.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moslem.Modules.GDSModuleMVC.Components
{
    public class RouteConfig : IMvcRouteMapper
    {
        // Token: 0x0600001E RID: 30 RVA: 0x00002841 File Offset: 0x00000A41
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapRoute("GDSModuleMVC", "GDSModuleMVC", "{controller}/{action}", new string[]
            {
                "Moslem.Modules.GDS.GDSModuleMVC.Controllers"
            });
        }
    }
}