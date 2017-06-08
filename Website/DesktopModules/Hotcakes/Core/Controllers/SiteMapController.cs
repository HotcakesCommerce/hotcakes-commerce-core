using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Controllers
{
	public class SiteMapController : BaseStoreController
	{
		//
		// GET: /SiteMap/
		public ActionResult Index()
		{
			List<CategorySnapshot> allCats = HccApp.CatalogServices.Categories.FindAllPaged(1, 5000);

			List<CategorySnapshot> displayCats = new List<CategorySnapshot>();
			foreach (CategorySnapshot snap in allCats)
			{
				if (snap.SourceType == CategorySourceType.CustomLink)
				{
					if (snap.CustomPageUrl.ToLowerInvariant().StartsWith("http")) continue;
				}
				displayCats.Add(snap);
			}

			ViewBag.Title = SiteTerms.GetTerm(SiteTermIds.SiteMap);
			return View(displayCats);
		}

		[OutputCache(VaryByHeader = "Host", VaryByParam = "none", VaryByCustom = "disablecsscaching", Duration = 150)]
		public ActionResult Xml()
		{
			string sitemap = SiteMapGenerator.BuildForStore(HccApp);
			return Content(sitemap, "text/xml");
		}

	}
}
