using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Moslem.Modules.GDS.GDSModuleMVC.Controllers;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
using Moslem.Modules.GDSApi.GDSModule;
using Moslem.Modules.GDSModuleMVC.Components;
using Moslem.Modules.GDSModuleMVC.Models;
using Newtonsoft.Json;
using static Moslem.Modules.GDSModuleMVC.Components.Cards;

namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
    // Token: 0x02000090 RID: 144
    [DnnHandleError]
    public class GDSController : DnnController
    {
        public JsonResult GetJsonResult()
        {
            return new JsonResult
            {
                Data = new
                {
                    UserID = base.User.UserID,
                    PortalId = base.PortalSettings.PortalId,
                    Alias = base.PortalSettings.PortalAlias.HTTPAlias,
                    Time = DateTime.Now.ToString("HH:mm:ss ttt")
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // Token: 0x170001D9 RID: 473
        private new string LocalResourceFile
        {
            get
            {
                string text = Thread.CurrentThread.CurrentCulture.ToString();
                bool flag = System.Web.HttpContext.Current.Request.Cookies["language"] != null;
                if (flag)
                {
                    text = System.Web.HttpContext.Current.Request.Cookies["language"].Value + ".";
                }
                bool flag2 = text == "en-us." || text == "en-US." || text == "EN-us." || text == "EN-US.";
                if (flag2)
                {
                    text = "";
                }
                return "DesktopModules/MVC/GDSModuleMVC/App_LocalResources/SharedResources." + text.ToLower() + "resx";
            }
        }
        private new string HotelsLocalResourceFile
        {
            get
            {
                string text = Thread.CurrentThread.CurrentCulture.ToString();
                bool flag = System.Web.HttpContext.Current.Request.Cookies["language"] != null;
                if (flag)
                {
                    text = System.Web.HttpContext.Current.Request.Cookies["language"].Value + ".";
                }
                bool flag2 = text == "en-us." || text == "en-US." || text == "EN-us." || text == "EN-US.";
                if (flag2)
                {
                    text = "";
                }
                return "DesktopModules/MVC/GDSModuleMVC/App_LocalResources/Hotels." + text.ToLower() + "resx";
            }
        }
        private new string CityLocalResourceFile
        {
            get
            {
                string text = Thread.CurrentThread.CurrentCulture.ToString();
                bool flag = System.Web.HttpContext.Current.Request.Cookies["language"] != null;
                if (flag)
                {
                    text = System.Web.HttpContext.Current.Request.Cookies["language"].Value + ".";
                }
                bool flag2 = text == "en-us." || text == "en-US." || text == "EN-us." || text == "EN-US.";
                if (flag2)
                {
                    text = "";
                }
                return "DesktopModules/MVC/GDSModuleMVC/App_LocalResources/City." + text.ToLower() + "resx";
            }
        }
        public HttpStatusCode UploadFile(string HotelID, string RoomID)
        {
            bool flag = this.CheckUserisHotelMaster(base.User.UserID, HotelID);
            if (flag)
            {
                try
                {
                    bool flag2 = base.Request.Files.AllKeys.Any<string>();
                    if (flag2)
                    {
                        HttpPostedFileBase httpPostedFileBase = base.Request.Files["UploadedImage"];
                        bool flag3 = httpPostedFileBase != null;
                        if (flag3)
                        {
                            bool flag4 = !Directory.Exists(string.Concat(new string[]
                            {
                                base.PortalSettings.HomeDirectoryMapPath,
                                "Images\\Hotel\\",
                                HotelID,
                                "\\",
                                RoomID
                            }));
                            if (flag4)
                            {
                                Directory.CreateDirectory(string.Concat(new string[]
                                {
                                    base.PortalSettings.HomeDirectoryMapPath,
                                    "Images\\Hotel\\",
                                    HotelID,
                                    "\\",
                                    RoomID
                                }));
                            }
                            httpPostedFileBase.SaveAs(string.Concat(new string[]
                            {
                                base.PortalSettings.HomeDirectoryMapPath,
                                "Images\\Hotel\\",
                                HotelID,
                                "\\",
                                RoomID,
                                "\\001s.jpg"
                            }));
                        }
                    }
                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return HttpStatusCode.InternalServerError;
                }
            }
            return HttpStatusCode.InternalServerError;
        }

        public bool CheckUserisHotelMaster(int UserID, string HotelID)
        {
            bool flag = ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo().IsInRole("Administrators");
            if (flag)
            {
                bool isSuperUser = ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo().IsSuperUser;
                if (isSuperUser)
                {
                    return true;
                }
            }
            UserInfo currentUserInfo = ServiceLocator<IUserController, UserController>.Instance.GetCurrentUserInfo();
            return HotelID == currentUserInfo.Profile.GetPropertyValue("MiddleName");
        }

        public ActionResult gethotelsRoom(int cityId, string HotelID)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                Api api = new Api();
                api = api.InitializeApi(base.PortalSettings.PortalId);
                bool success = api.Success;
                if (success)
                {
                    IEnumerable<ResponsegetHotel> source = from p in api.getHotel(new reqgetHotel
                    {
                        cityId = cityId,
                        sessionId = api.SessionID
                    }).response
                                                           select p;
                    IEnumerable<RoomgetHotel> enumerable = from p in (from p in source
                                                                      where p.id == HotelID
                                                                      select p).FirstOrDefault<ResponsegetHotel>().room
                                                           select p;
                    foreach (RoomgetHotel roomgetHotel in enumerable)
                    {
                        stringBuilder.Append("<div class=\"col-md-3 col-sm-6\">");
                        stringBuilder.Append("<div class=\"panel panel-primary editIMG\">");
                        stringBuilder.Append("<div class=\"panel-heading\">" + roomgetHotel.description + "</div>");
                        stringBuilder.Append("<div class=\"panel-body\">");
                        bool flag = System.IO.File.Exists(string.Concat(new object[]
                        {
                            base.PortalSettings.HomeDirectoryMapPath,
                            "Images\\Hotel\\",
                            HotelID,
                            "\\",
                            roomgetHotel.id,
                            "\\001s.jpg"
                        }));
                        if (flag)
                        {
                            stringBuilder.Append(string.Concat(new object[]
                            {
                                "<img id=\"img",
                                roomgetHotel.id,
                                "\"  src=\"",
                                base.PortalSettings.HomeDirectory,
                                "Images/Hotel/",
                                HotelID,
                                "/",
                                roomgetHotel.id,
                                "/001s.jpg?date=",
                                DateTime.Now.Ticks,
                                "\" />"
                            }));
                        }
                        else
                        {
                            stringBuilder.Append(string.Concat(new object[]
                            {
                                "<img id=\"img",
                                roomgetHotel.id,
                                "\" src=\"",
                                base.PortalSettings.HomeDirectory,
                                "noimg.jpg\" />"
                            }));
                        }
                        bool flag2 = this.CheckUserisHotelMaster(base.User.UserID, HotelID);
                        if (flag2)
                        {
                            stringBuilder.Append("<div><label for=\"fileUpload\">فایل را جهت ارسال انتخاب نمایید <input  id=\"file" + roomgetHotel.id + "\" type=\"file\" />");
                            stringBuilder.Append("<input  onclick=\"myFunction(this.id)\" class=\"btnUploadFile\" id=\"" + roomgetHotel.id + "\" type=\"button\" value=\"ارسال تصویر\" />");
                        }
                        stringBuilder.Append("</div>");
                        stringBuilder.Append(string.Concat(new object[]
                        {
                            enumerable.Count<RoomgetHotel>(),
                            " ",
                            roomgetHotel.id,
                            " ",
                            roomgetHotel.description,
                            "<br />"
                        }));
                        stringBuilder.Append("</div>");
                        stringBuilder.Append("</div>");
                        stringBuilder.Append("</div>");
                    }
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Content(stringBuilder.ToString());
                }
                else
                {
                    bool flag3 = api.StatusCode == 1101;
                    if (flag3)
                    {
                        throw new Exception("نام کاربری و یا رمز عبور نامعتبر است");
                    }
                    throw new Exception("خطایی نامشخص پیش آمده است");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content("");
            }

        }

        public JsonResult getHotel(int cityId)
        {
            JsonResult result;
            try
            {

                if (!(DataCache.GetCache("CityHotels" + cityId) != null && ((resgetHotel)DataCache.GetCache("CityHotels" + cityId)).status))
                {
                    Api api = new Api();
                    api = api.InitializeApi(base.PortalSettings.PortalId);
                    bool success = api.Success;
                    if (success)
                    {
                        resgetHotel hotel = api.getHotel(new reqgetHotel
                        {
                            cityId = cityId,
                            sessionId = api.SessionID
                        });
                        result = base.Json("{\"aaData\":" + JsonConvert.SerializeObject(from p in hotel.response
                                                                                        select new
                                                                                        {
                                                                                            text = Localization.GetString(p.description, HotelsLocalResourceFile, false),
                                                                                            value = p.id
                                                                                        }) + "}", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        bool flag2 = api.StatusCode == 1101;
                        if (flag2)
                        {
                            throw new Exception("نام کاربری و یا رمز عبور نامعتبر است");
                        }
                        throw new Exception("خطایی نامشخص پیش آمده است");
                    }
                }
                else
                {
                    resgetHotel hotel = (resgetHotel)DataCache.GetCache("CityHotels" + cityId);
                    result = base.Json("{\"aaData\":" + JsonConvert.SerializeObject(from p in hotel.response
                                                                                    select new
                                                                                    {
                                                                                        text = Localization.GetString(p.description, HotelsLocalResourceFile, false),
                                                                                        value = p.id
                                                                                    }) + "}", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                base.Response.StatusCode = 500;
                result = base.Json("این شهر دارای هتل نمی باشد", JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        public JsonResult GetRoomsInhote(string HotelID, int CityID)
        {
            JsonResult result;
            try
            {
                Api api = new Api();
                api = api.InitializeApi(base.PortalSettings.PortalId);
                bool success = api.Success;
                if (success)
                {
                    resgetHotel hotel = api.getHotel(new reqgetHotel
                    {
                        cityId = CityID,
                        sessionId = api.SessionID
                    });
                    var SpecialHotel = from p in hotel.response
                                       where p.id == HotelID
                                       select p.room;
                    result = base.Json("{\"aaData\":" + JsonConvert.SerializeObject(from p in SpecialHotel.FirstOrDefault()
                                                                                    select new
                                                                                    {
                                                                                        text = p.description,
                                                                                        CityLocalResourceFile,
                                                                                        value = p.id
                                                                                    }) + "}", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    bool flag2 = api.StatusCode == 1101;
                    if (flag2)
                    {
                        throw new Exception("نام کاربری و یا رمز عبور نامعتبر است");
                    }
                    throw new Exception("خطایی نامشخص پیش آمده است");
                }
            }
            catch (Exception ex)
            {
                base.Response.StatusCode = 500;
                result = base.Json("این شهر دارای هتل نمی باشد", JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        public ActionResult getCity()
        {
            JsonResult result;
            try
            {
                Api api = new Api();
                api = api.InitializeApi(base.PortalSettings.PortalId);
                bool success = api.Success;
                if (success)
                {
                    resgetCity City = api.getCity(new reqgetCity
                    {
                        countryId = 1,
                        sessionId = api.SessionID,
                        Lang = 2
                    });
                    result = base.Json("{\"aaData\":" + JsonConvert.SerializeObject(from p in City.response
                                                                                    select new
                                                                                    {
                                                                                        text = p.description,
                                                                                        value = p.id
                                                                                    }) + "}", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    bool flag2 = api.StatusCode == 1101;
                    if (flag2)
                    {
                        throw new Exception("نام کاربری و یا رمز عبور نامعتبر است");
                    }
                    throw new Exception("خطایی نامشخص پیش آمده است");
                }
            }
            catch (Exception ex)
            {
                base.Response.StatusCode = 500;
                result = base.Json("این شهر دارای هتل نمی باشد", JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        public ActionResult CheckUser(int portalID)
        {
            ActionResult result;
            try
            {
                Api api = new Api();
                api = api.InitializeApi(base.PortalSettings.PortalId);
                ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(portalID, "GDSModuleMVC");
                ModuleController moduleController = new ModuleController();
                Hashtable tabModuleSettings = moduleController.GetTabModuleSettings(moduleByDefinition.TabModuleID);
                resplogin value = api.Execute("login", JsonConvert.SerializeObject(new reqlogin
                {
                    user = tabModuleSettings["Username"].ToString(),
                    password = tabModuleSettings["Password"].ToString()
                })).ToObject<resplogin>();
                result = base.Json("{\"aaData\":" + JsonConvert.SerializeObject(value) + "}");
            }
            catch (Exception ex)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return result;
        }

        public ActionResult reserveandpay(reserveandpay objJson)
        {

            ActionResult result;
            try
            {
                Api api = new Api();
                api = api.InitializeApi(base.PortalSettings.PortalId);
                bool flag = !api.Success && api.StatusCode == 2;
                if (flag)
                {
                    base.Response.StatusCode = 403;
                    result = base.Content("اتصال به API ممکن نیست");
                }
                else
                {
                    bool flag2 = !api.Success && api.StatusCode == 1;
                    if (flag2)
                    {
                        base.Response.StatusCode = 403;
                        result = base.Content("تام کاربری یا رمز عبور اتصال به API نا معتبر است.");
                    }
                    else
                    {
                        List<string> RoomsID = objJson.roomid.Split('»').ToList<string>();
                        List<string> RoomsCount = objJson.roomCount.Split('»').ToList<string>();
                        List<string> RoomsCapacity = objJson.RoomCapacities.Split('»').ToList<string>();

                        int count = objJson.Passengers.Count;
                        List<Passengerreserve> lstPassanger = new List<Passengerreserve>();

                        for (int j = 0; j < RoomsID.Count; j++)
                        {
                            for (int k = 0; k < int.Parse(RoomsCount[j]); k++)
                            {
                                lstPassanger.Add(new Passengerreserve
                                {
                                    extraBedNo = "0",
                                    early = "0",
                                    late = "0",
                                    infantNo = "0",
                                    childNo = "0",
                                    description = objJson.Description,
                                    mobile = objJson.mobile,
                                    idNo = objJson.idno.ToString(),
                                    family = objJson.firstName,
                                    name = objJson.lastname,
                                    roomId = RoomsID[j]
                                });
                            }
                        }
                        resreserve resreserve = api.reserveandpay(new reqreserve
                        {
                            sessionId = api.SessionID,
                            firstname = objJson.firstName,
                            lastname = objJson.lastname,
                            email = objJson.email,
                            hotelId = int.Parse(objJson.hotelId.Replace(".", "")),
                            night = int.Parse(objJson.night),
                            tel = objJson.tel,
                            mobile = objJson.mobile,
                            checkin = Utility.ShamsiToMiladi(objJson.checkin),
                            passenger = lstPassanger
                        });

                        (new Cards()).UpdateReserveIDOfCard(Session.SessionID, resreserve.response.reserve.info.id);
                        decimal DiscountAmount;
                        objJson.amount = decimal.Parse(Convert.ToString(Session["PriceAfterGiftCard"]));
                        if (objJson.Retired && Utility.RetiredHasCredit(objJson.idno, objJson.DaftarNumber))
                        {
                            decimal AfterRetiredDiscount = Utility.GetRetiredDiscount(objJson.amount, objJson.hasPartner);
                            DiscountAmount = objJson.amount - AfterRetiredDiscount;
                            Utility.runQuery(string.Concat(new string[]
                               {
                                    "INSERT INTO [dbo].[GDS_RetiredCreditUsed]([NationalCode],[DaftarNumber],[DateOfUsed],[TotalAmount],[DiscountAmount],[Finalized],[HasPartner],[ReserveID])" ,
                                    "VALUES('" + objJson.idno + "','" + objJson.DaftarNumber + "',getdate() ," + objJson.amount + "," + DiscountAmount + " ,0, " + Convert.ToInt32( objJson.hasPartner) + "," + resreserve.response.reserve.info.id + ")"
                               }));
                        }
                        else
                        {
                            objJson.AmountAfterDiscount = objJson.amount;
                        }

                        if (Session["ValidRetired"] != null && Session["RetiredNationCode"] != null && Session["DaftarNumber"] != null)
                            if (Utility.isValidRetired(Int64.Parse(Convert.ToString(Session["RetiredNationCode"])), Int64.Parse(Convert.ToString(Session["DaftarNumber"]))) == "1")
                            {
                                objJson.amount = decimal.Parse(Convert.ToString(Session["RackPriceAfterGiftCard"]));
                                Utility.runQuery(string.Concat(new string[]
                                {
                                    "INSERT INTO [dbo].[GDS_RetiredCreditUsed]([NationalCode],[DaftarNumber],[DateOfUsed],[TotalAmount],[DiscountAmount],[Finalized],[HasPartner],[ReserveID])" ,
                                    "VALUES('" + objJson.idno + "','" + objJson.DaftarNumber + "',getdate() ," + objJson.amount + "," + objJson.amount/2 + " ,0, " + Convert.ToInt32( objJson.hasPartner) + "," + resreserve.response.reserve.info.id + ")"

                                }));
                                objJson.AmountAfterDiscount = objJson.amount/2;
                            }

                        if (string.IsNullOrEmpty(resreserve.response.reserve.info.thirdPartyCode))
                        {
                            resreserve.response.reserve.info.thirdPartyCode = "0";
                        }
                        bool flag6 = resreserve.status && resreserve.errorCode == 0;
                        if (flag6)
                        {
                            this.AddReserve(resreserve);
                            this.GetReserve(resreserve.response.reserve.info.id);
                            foreach (Passenger passenger in objJson.Passengers)
                            {
                                Utility.runQuery(string.Concat(new string[]
                                {
                                    "INSERT INTO [dbo].[GDS_Passenger]([fname],[lname],[email],[Codemelli],[reserveid]) VALUES(N'",
                                    passenger.name,
                                    "',N'",
                                    passenger.lname,
                                    "',N'",
                                    passenger.email,
                                    "',N'",
                                    passenger.codemelli,
                                    "',N'",
                                    resreserve.response.reserve.info.id,
                                    "')"
                                }));
                            }
                            base.Response.Cookies["Prc"].Value = objJson.AmountAfterDiscount.ToString();
                            base.Response.Cookies["Prc"].Expires = DateTime.Now.AddMinutes(20.0);
                            base.Response.Cookies["Prc"].Domain = base.Request.Url.Host;
                            base.Response.Cookies["Prc"].Path = "/";
                            base.Response.Cookies["rid"].Value = resreserve.response.reserve.info.id;
                            base.Response.Cookies["rid"].Expires = DateTime.Now.AddMinutes(20.0);
                            base.Response.Cookies["rid"].Domain = base.Request.Url.Host;
                            base.Response.Cookies["rid"].Path = "/";
                            result = this.Content("{\"aaData\":" + JsonConvert.SerializeObject(resreserve) + "}", "application/json", Encoding.UTF8);
                        }
                        else
                        {
                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new StringContent("{\"aaData\":" + JsonConvert.SerializeObject(resreserve) + "}", Encoding.UTF8, "application/json");
                            base.Response.StatusCode = 400;
                            result = this.Content("{\"aaData\":" + JsonConvert.SerializeObject(resreserve) + "}", "application/json", Encoding.UTF8);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                base.Response.StatusCode = 500;
                result = base.Content(ex.Message);
            }
            return result;
        }

        private void AddReserve(resreserve ObjReserve)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                bool flag = dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body] from [GDS_Reserve] where [ReserveID]= " + ObjReserve.response.reserve.info.id, new object[0]).Count<string>() == 0;
                if (flag)
                {
                    dataContext.Execute(CommandType.Text, string.Concat(new string[]
                    {
                        "INSERT INTO [GDS_Reserve]([ReserveID],[Body]) VALUES ('",
                        ObjReserve.response.reserve.info.id,
                        "',N'",
                        JsonConvert.SerializeObject(ObjReserve).Replace("@", "@@"),
                        "')"
                    }), new object[0]);
                }
            }
        }

        private resreserve GetReserve(string ReserveID)
        {
            resreserve result = new resreserve();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = JsonConvert.DeserializeObject<resreserve>(dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body]  from [GDS_Reserve] where [ReserveID]= " + ReserveID, new object[0]).FirstOrDefault<string>());
            }
            return result;
        }
        public ActionResult RetiredIsValid(long NationalCode, long DaftarNumber)
        {
            return Json(Utility.isValidRetired(NationalCode, DaftarNumber), JsonRequestBehavior.AllowGet);
        }
        public ActionResult retiredhascredit(long NationalCode, long DaftarNumber)
        {
            string returnValue = Utility.isValidRetired(NationalCode, DaftarNumber);
            if (returnValue == "1")
            {
                List<RetiredCreditUsed> objRetiredCreditUsed = new List<RetiredCreditUsed>();
                using (IDataContext dataContext = DataContext.Instance())
                {
                    objRetiredCreditUsed = dataContext.ExecuteQuery<RetiredCreditUsed>(CommandType.Text, "SELECT * FROM [GDS_RetiredCreditUsed] where NationalCode ='" + NationalCode + "' and DaftarNumber ='" + DaftarNumber + "'").ToList<RetiredCreditUsed>();

                }
                if (objRetiredCreditUsed.Count < 3)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return PartialView("CreditSummary", objRetiredCreditUsed);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Content("بازنشسته اعتبار ندارد");
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("خطا در اعتبارسنجی بازنشسته");
            }
        }
        public ActionResult ApplyFiftyDiscount(long NationalCode, long DaftarNumber)
        {
            var returnValue = Utility.isValidRetired(NationalCode, DaftarNumber);
            if (returnValue == "1")
            {
                Session["RetiredNationCode"] = NationalCode;
                Session["DaftarNumber"] = DaftarNumber;
                Session["ValidRetired"] = true;
            }
            return Content(returnValue);
        }
    }
}
