using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Microsoft.CSharp.RuntimeBinder;
using Moslem.Modules.GDSApi.GDSModule;
using Moslem.Modules.GDSModuleMVC.Components;
using Newtonsoft.Json;
using static Moslem.Modules.GDSModuleMVC.Components.Cards;

namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
    public class SearchController : DnnController
    {
        [HttpGet]
        public ActionResult Index()
        {
            ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(base.PortalSettings.PortalId, "GDSModuleMVCReserve");
            if (moduleByDefinition == null)
            {
                ViewBag.Message = "ماژول دریافت اطلاعات کاربر جهت رزرو را در پورتال قرار دهید.";
                return View("Error");
            }
            ViewBag.Message = "پارامتر های جست و جو کامل نمی باشد.";
            return View("Error");

        }
        [HttpPost]
        public ActionResult Index(searchObject ob)
        {
            ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(base.PortalSettings.PortalId, "GDSModuleMVCReserve");
            if (moduleByDefinition == null)
            {
                ViewBag.Message = "ماژول دریافت اطلاعات کاربر جهت رزرو را در پورتال قرار دهید.";
                return View("Error");
            }
            if (ob.sdate == null || ob.edate == null || ob.hotelID == null)
            {
                ViewBag.Message = "پارامتر های جست و جو کامل نمی باشد.";
                return View("Error");
            }

            CultureInfo cultureInfo = new CultureInfo("en-US");
            ViewBag.sdate = ob.sdate;
            ViewBag.edate = ob.edate;
            double nightShamsi = Utility.GetNightShamsi(ob.sdate, ob.edate);
            ViewBag.night = nightShamsi;
            ViewBag.Count = ob.count;
            base.ViewBag.CityText = ob.citytitle;
            int capacityId = 1;
            int num;
            bool flag = !int.TryParse(ob.count, out num);
            if (flag)
            {
                capacityId = 1;
                num = 5;
            }
            Api api = new Api();
            api = api.InitializeApi(base.PortalSettings.PortalId);
            reqsearchHotel @object = new reqsearchHotel
            {
                sessionId = api.SessionID,
                cityId = int.Parse(ob.cityid),
                capacity = 1,
                checkin = Utility.ShamsiToMiladi(ob.sdate),
                hotelId = int.Parse(ob.hotelID),
                capacityId = capacityId,
                nights = int.Parse(nightShamsi.ToString()),
                commission = 0,
                person = 0
            };
            ressearchHotel ressearchHotel = api.searchHotel(@object);
            Session["SearchHotel"] = ressearchHotel;
            string text = JsonConvert.SerializeObject(ressearchHotel);
            resgetHotel hotel = api.getHotel(new reqgetHotel
            {
                cityId = int.Parse(ob.cityid),
                sessionId = api.SessionID
            });
            SearchViewModel list = new SearchViewModel();

            if (Session["GiftCardValue"] != null)
            {

            }
            using (List<ResponsesearchHotel>.Enumerator enumerator = ressearchHotel.response.GetEnumerator())
            {
                list.responsesearchHotel = new List<RoomsesearchHotel>();
                while (enumerator.MoveNext())
                {
                    ResponsesearchHotel item = enumerator.Current;
                    foreach (RoomsesearchHotel roomsesearchHotel in item.room)
                    {
                        list.responsegetHotel = (from p in hotel.response.Where(p => p.id == ob.hotelID.ToString()) select p).FirstOrDefault<ResponsegetHotel>();//.FirstOrDefault();
                        list.responsesearchHotel.Add(roomsesearchHotel);
                    }
                }
            }
            ViewBag.Url = base.ModuleContext.EditUrl("SaveReserveData");
            if (Session["ValidRetired"] != null && Session["RetiredNationCode"] != null && Session["DaftarNumber"] != null)
            {

                if (Utility.isValidRetired(Int64.Parse(Convert.ToString(Session["RetiredNationCode"])), Int64.Parse(Convert.ToString(Session["DaftarNumber"]))) == "1")
                    list.IsValidRetired = true;
                else
                    list.IsValidRetired = false;
            }
            return base.View(list);
        }
        [HttpPost]
        public ActionResult SaveReserveData(ReserveObject i)
        {
            base.Session["ReserveObject"] = i;
            ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(base.PortalSettings.PortalId, "GDSModuleMVCReserve");
            return this.Redirect(Globals.NavigateURL(moduleByDefinition.TabID, ""));
        }
        [HttpGet]
        public ActionResult SaveReserveData()
        {
            return base.View();
        }
    }
}
