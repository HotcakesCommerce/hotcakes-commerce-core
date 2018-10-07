using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Microsoft.CSharp.RuntimeBinder;
using Moslem.Modules.GDS.GDSModuleMVC.Components;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
    [DnnHandleError]
    public class HotelsController : DnnController
    {
        public ActionResult Edit(int itemId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration("DnnPlugins");
            ArrayList users = UserController.GetUsers(base.PortalSettings.PortalId);
            IEnumerable<SelectListItem> arg = from user in users.Cast<UserInfo>().ToList<UserInfo>()
                                              select new SelectListItem
                                              {
                                                  Text = user.DisplayName,
                                                  Value = user.UserID.ToString()
                                              };
            base.ViewBag.Users = arg;
            Item item;
            if (itemId != -1)
            {
                item = ServiceLocator<IItemManager, ItemManager>.Instance.GetItem(itemId, base.ModuleContext.ModuleId);
            }
            else
            {
                (item = new Item()).ModuleId = base.ModuleContext.ModuleId;
            }
            Item model = item;
            return base.View(model);
        }
        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
            bool flag = item.ItemId == -1;
            if (flag)
            {
                item.CreatedByUserId = base.User.UserID;
                item.CreatedOnDate = DateTime.UtcNow;
                item.LastModifiedByUserId = base.User.UserID;
                item.LastModifiedOnDate = DateTime.UtcNow;
                ServiceLocator<IItemManager, ItemManager>.Instance.CreateItem(item);
            }
            else
            {
                Item item2 = ServiceLocator<IItemManager, ItemManager>.Instance.GetItem(item.ItemId, item.ModuleId);
                item2.LastModifiedByUserId = base.User.UserID;
                item2.LastModifiedOnDate = DateTime.UtcNow;
                item2.ItemName = item.ItemName;
                item2.ItemDescription = item.ItemDescription;
                item2.AssignedUserId = item.AssignedUserId;
                ServiceLocator<IItemManager, ItemManager>.Instance.UpdateItem(item2);
            }
            return base.RedirectToDefaultRoute();
        }
        public ActionResult GetDemoPartial()
        {
            base.TempData["UserID"] = base.User.UserID;
            base.ViewData["PortalId"] = base.PortalSettings.PortalId;
            base.ViewBag.Alias = base.PortalSettings.PortalAlias.HTTPAlias;
            return this.PartialView("_DemoPartial", DateTime.Now);
        }
        public ActionResult Index()
        {
            Api api = new Api();
            if (DataCache.GetCache("CityOfCountry" + 1) != null && ((resgetCity)DataCache.GetCache("CityOfCountry" + 1)).status)
            {
                resgetCity resgetCity = (resgetCity)DataCache.GetCache("CityOfCountry" + 1);
                return View(resgetCity);
            }
            else
            {
                api = api.InitializeApi(base.PortalSettings.PortalId);
                bool success = api.Success;
                if (success)
                {
                    resgetCity city = api.getCity(new reqgetCity
                    {
                        countryId = 1,
                        sessionId = api.SessionID,Lang = 2
                    });
                    return View(city);
                }
                else
                {
                    bool flag = api.StatusCode == 1;
                    if (flag)
                    {
                        base.ViewBag.Message = "تنظیمات را بررسی نمایید";
                        return View("Error");
                    }
                    else
                    {
                        bool flag2 = api.StatusCode == 2;
                        if (flag2)
                        {
                            ViewBag.Message = "اتصال به API نا موفق بود";
                            return View("Error");
                        }
                        else
                        {
                            ViewBag.Message = "نام کاربری و رمزعبور نامعتبر است";
                            return View("Error");
                        }
                    }
                }
            }

        }
    }
}
