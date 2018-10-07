using System;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	[DnnHandleError]
	public class MoslemController : DnnController
	{
		public ActionResult Index()
		{
			return base.View();
		}
	}
}
