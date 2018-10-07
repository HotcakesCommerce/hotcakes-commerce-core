using System;
using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
	[DnnHandleError]
	public class SettingsController : DnnController
	{
		[HttpGet]
		public ActionResult Settings()
		{
			return base.View(new Settings
			{
				Username = base.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Username", ""),
				Password = base.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("Password", ""),
				ApiKey = base.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ApiKey", ""),
				SessionID = base.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("SessionID", ""),
                WCURL = base.ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("WCURL", "http://demo.golbansystem.com/api/")
            });
		}
		[HttpPost]
		[ValidateInput(false)]
		[DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
		public ActionResult Settings(Settings settings)
		{
			base.ModuleContext.Configuration.ModuleSettings["Username"] = settings.Username.ToString();
			base.ModuleContext.Configuration.ModuleSettings["Password"] = settings.Password.ToString();
			base.ModuleContext.Configuration.ModuleSettings["SessionID"] = settings.SessionID.ToString();
			base.ModuleContext.Configuration.ModuleSettings["ApiKey"] = settings.ApiKey.ToString();
			base.ModuleContext.Configuration.ModuleSettings["WCURL"] = settings.ApiKey.ToString();
			ModuleController moduleController = new ModuleController();
			moduleController.UpdateModuleSetting(base.ModuleContext.ModuleId, "Username", settings.Username.ToString());
			moduleController.UpdateModuleSetting(base.ModuleContext.ModuleId, "Password", settings.Password.ToString());
			moduleController.UpdateModuleSetting(base.ModuleContext.ModuleId, "ApiKey", settings.ApiKey.ToString());
			moduleController.UpdateModuleSetting(base.ModuleContext.ModuleId, "SessionID", settings.SessionID.ToString());
			moduleController.UpdateModuleSetting(base.ModuleContext.ModuleId, "WCURL", settings.WCURL.ToString());
			moduleController.UpdateTabModuleSetting(base.ModuleContext.TabModuleId, "Username", settings.Username.ToString());
			moduleController.UpdateTabModuleSetting(base.ModuleContext.TabModuleId, "Password", settings.Password.ToString());
			moduleController.UpdateTabModuleSetting(base.ModuleContext.TabModuleId, "ApiKey", settings.ApiKey.ToString());
			moduleController.UpdateTabModuleSetting(base.ModuleContext.TabModuleId, "SessionID", settings.SessionID.ToString());
			moduleController.UpdateTabModuleSetting(base.ModuleContext.TabModuleId, "WCURL", settings.WCURL.ToString());
			return base.RedirectToDefaultRoute();
		}
	}
}
