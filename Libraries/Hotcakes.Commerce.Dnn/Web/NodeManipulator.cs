#region License

// Distributed under the MIT License
// ============================================================
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