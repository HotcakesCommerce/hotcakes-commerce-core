using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Web.DDRMenu;
using Hotcakes.DNN.Mvc;
using System.Web.Mvc;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Urls;


namespace Hotcakes.DNN.Mvc
{
	public class NodeManipulator : INodeManipulator
	{
		public List<MenuNode> ManipulateNodes(List<MenuNode> nodes, DotNetNuke.Entities.Portals.PortalSettings portalSettings)
		{
			MenuNode categoriesMenu = new MenuNode { Text = "Product Categories" };
			nodes.Insert(0, categoriesMenu);

			//Find Categories to Display in Menu

			HotcakesApplication hccApp = HccAppHelper.InitHccApp();
			List<CategorySnapshot> categories = hccApp.CatalogServices.Categories.FindForMainMenu();

			foreach (CategorySnapshot category in categories)
			{
				string url = HccUrlBuilder.RouteHccUrl(HccRoute.Category, new { slug = category.RewriteUrl });
				categoriesMenu.Children.Add(new MenuNode { Text = category.Name, Url = url, Enabled = true, Parent = categoriesMenu });
			}

			return nodes;
		}
	}
}