﻿#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using DotNetNuke.Web.Api;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Http;

namespace Hotcakes.Commerce.Dnn.Providers
{
	[Serializable()]
	public class ServiceRouteMapper : IServiceRouteMapper
	{
		public void RegisterRoutes(IMapRoute mapRouteManager)
		{
			AreaRegistration.RegisterAllAreas();

            RegisterWebApi(GlobalConfiguration.Configuration);
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			RegisterDNNApi(mapRouteManager);
		}

		private static void RegisterDNNApi(IMapRoute mapRouteManager)
		{
			mapRouteManager.MapHttpRoute("Hotcakes/ProductViewer", "default", "{controller}/{action}", new string[] { "Hotcakes.Commerce.Dnn.Social" });
			mapRouteManager.MapHttpRoute("Hotcakes/CategoryViewer", "default", "{controller}/{action}", new string[] { "Hotcakes.Commerce.Dnn.Social" });
		}

		public static void RegisterWebApi(HttpConfiguration config)
		{
			RegisterWebApiRoute("hcapi-authorize", "DesktopModules/Hotcakes/API/mobile/v1-0/authorize", new { controller = "Login" });
			RegisterWebApiRoute("hcapi-reports", "DesktopModules/Hotcakes/API/mobile/v1-0/reports/{action}/{period}", new { controller = "Reports", period = System.Web.Http.RouteParameter.Optional });
			RegisterWebApiRoute("hcapi-orders", "DesktopModules/Hotcakes/API/mobile/v1-0/orders/list/{id}", new { controller = "Orders", id = System.Web.Http.RouteParameter.Optional });
			RegisterWebApiRoute("hcapi-capturepayment", "DesktopModules/Hotcakes/API/mobile/v1-0/orders/capturepayment", new { controller = "Orders", action = "CapturePayment" });
			RegisterWebApiRoute("hcapi-shipperlist", "DesktopModules/Hotcakes/API/mobile/v1-0/orders/shipperlist/{orderId}", new { controller = "Orders", action = "GetShipperList" });
			RegisterWebApiRoute("hcapi-markshipped", "DesktopModules/Hotcakes/API/mobile/v1-0/orders/markshipped", new { controller = "Orders", action = "MarkAsShipped" });
			RegisterWebApiRoute("hcapi-storesettings", "DesktopModules/Hotcakes/API/mobile/v1-0/storesettings", new { controller = "Settings" });
		}

		public static void RegisterWebApiRoute(string name, string routeTemplate, object defaults)
		{
			var r = RouteTable.Routes.MapHttpRoute(name, routeTemplate, defaults);

			r.DataTokens = new RouteValueDictionary();
			r.DataTokens["namespaces"] = new string[] { "Hotcakes.Modules.Core.Api.Mobile" };
			r.DataTokens["name"] = name;
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			// Ignore the website root
			routes.IgnoreRoute("");

			// ignore all .aspx , .ashx and .htmtemplate regardless of path
			routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
			routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
			routes.IgnoreRoute("{*htmtemplate}", new { htmtemplate = @".*\.htmtemplate(/.*)?" });

			routes.IgnoreRoute("favicon.ico");
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("images/{*imagedata}");
			
			var namespaces = new string[] { "Hotcakes.Modules.Core.Controllers" };
			
            // Products
			routes.MapRoute(
				"products-validate",
				"DesktopModules/Hotcakes/API/mvc/products/validate/{*params}",
				new { controller = "Products", action = "Validate" },
				namespaces
			);

			// Product Reviews
			routes.MapRoute(
				"productreviews",
				"DesktopModules/Hotcakes/API/mvc/productreviews/{action}/{id}",
                new { controller = "ProductReviews", action = "index", id = string.Empty },
				namespaces
			);

			// Estimate shipping
			routes.MapRoute(
				"estimate-shipping",
				"DesktopModules/Hotcakes/API/mvc/estimateshipping/{action}/{id}",
				new { controller = "EstimateShipping", action = "index", id = "0" },
				namespaces
			);

			// Cart
			routes.MapRoute(
				"cart-addcoupon",
				"DesktopModules/Hotcakes/API/mvc/cart/addcoupon",
				new { controller = "Cart", action = "AddCoupon" },
				namespaces
			);
			routes.MapRoute(
				"cart-removecoupont",
				"DesktopModules/Hotcakes/API/mvc/cart/removecoupon",
				new { controller = "Cart", action = "RemoveCoupon" },
				namespaces
			);
			routes.MapRoute(
				"cart-removeitem",
				"DesktopModules/Hotcakes/API/mvc/cart/removelineitem",
				new { controller = "Cart", action = "RemoveLineItem" },
				namespaces
			);
			routes.MapRoute(
				"cart-updateitem",
				"DesktopModules/Hotcakes/API/mvc/cart/updatelineitem",
				new { controller = "Cart", action = "UpdateLineItem" },
				namespaces
			);
			routes.MapRoute(
				"cart-bulkadd",
				"DesktopModules/Hotcakes/API/mvc/cart/bulkadd",
				new { controller = "Cart", action = "BulkAdd" },
				namespaces
			);

			// Checkouts
			routes.MapRoute(
				"checkout-route",
				"DesktopModules/Hotcakes/API/mvc/checkout/{action}/{*slug}",
				new { controller = "Checkout", action = "Index" },
				namespaces
			);

			// Api
			routes.MapRoute(
				"apirest",
				"DesktopModules/Hotcakes/API/rest/v{version}/{modelname}/{*parameters}",
				new { controller = "ApiRest", action = "Index", version = 1 },
				namespaces
			);

			// Default route have to be present to prevent crashing on RenderAction
			// when no route can be found
			routes.MapRoute(
				"Default",
				"DesktopModules/Hotcakes/API/mvc/{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces
			);
		}
	}
}