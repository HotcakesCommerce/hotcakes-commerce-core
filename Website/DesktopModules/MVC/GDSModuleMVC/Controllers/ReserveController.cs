using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Microsoft.CSharp.RuntimeBinder;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
using Moslem.Modules.GDSApi.GDSModule;
using Moslem.Modules.GDSModuleMVC.Components;
using Moslem.Modules.GDSModuleMVC.ir.shaparak.sep;
using Newtonsoft.Json;
using Stimulsoft.Base;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Moslem.Modules.GDSModuleMVC.Components.Cards;

namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
    [DnnHandleError]
    public class ReserveController : DnnController
    {
        public ActionResult Index()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            ReserveObject reserveObject = new ReserveObject();
            reserveObject = (ReserveObject)base.Session["ReserveObject"];
            //Return If No Valid Reserve Data 
            if (reserveObject == null)
            {
                ViewBag.Message = "امکان مراجعه مستقیم به این صفحه وجود ندارد.";
                return View("Error");
            }
            DiscountModelForCard obj = new DiscountModelForCard();
            obj.Card_PassKey = "";
            if (Session["GiftCardValue"] != null)
            {
                obj.Card_PassKey = (String)Session["GiftCardValue"];
            }
            obj.Count = reserveObject.roomCount.Split('»');
            obj.HotelID = Int32.Parse(reserveObject.hotelID);
            obj.RoomID = reserveObject.roomID.Split('»');
            obj.ReserveID = Session.SessionID;
            obj.night = new int[obj.RoomID.Count()];
            obj.ALLNight = Int32.Parse(reserveObject.night);
            DiscountModelForCard roomCountAfterCardDiscount = (new Cards()).ApplyCardDiscount(obj);
            ressearchHotel ressearchHotel = (ressearchHotel)Session["SearchHotel"];
            int? PriceAfterGiftCard = 0;
            int? RackPriceAfterGiftCard = 0;
            for (int i = 0; i < obj.RoomID.Count(); i++)
            {
                RoomsesearchHotel ObjRoom = ressearchHotel.response.Where(p => p.hotelId == obj.HotelID).Select(p => p).FirstOrDefault().room.Where(p => p.roomId == Int32.Parse(obj.RoomID[i])).FirstOrDefault();
                int[] TotalNightOfRoomsAfterDiscount = new int[obj.RoomID.Count()];
                TotalNightOfRoomsAfterDiscount[i] = obj.ALLNight * Int32.Parse(obj.Count[i]) - obj.night[i];
                for (int k = 0; k < TotalNightOfRoomsAfterDiscount[i]; k++)
                {
                    PriceAfterGiftCard += ObjRoom.detailPrice[0].price;
                    RackPriceAfterGiftCard += ObjRoom.detailPrice[0].rackRate;
                }
            }

            Session["PriceAfterGiftCard"] = PriceAfterGiftCard;
            Session["RackPriceAfterGiftCard"] = RackPriceAfterGiftCard;
            Session["roomCountAfterCardDiscount"] = roomCountAfterCardDiscount;
            ViewBag.TotalPriceAfterGiftcard = PriceAfterGiftCard;
            string[] array = reserveObject.roomCount.Split('»');
            int RoomCount = 0;
            foreach (string s in array)
                RoomCount += int.Parse(s);
            ViewBag.RoomCount = RoomCount;
            //reqgetHotel reqgetHotel = new reqgetHotel();
            reserveObject.ChecoutDate = DateTime.Parse(reserveObject.CheckinDate).AddDays((double)int.Parse(reserveObject.night)).ToShortDateString();
            ViewBag.ApplyUrl = Globals.NavigateURL(base.PortalSettings.ActiveTab.TabID, "Applied", new string[]
                {
                "mid=" + base.ActiveModule.ModuleID
                });
            if (Session["ValidRetired"] != null && Session["RetiredNationCode"] != null && Session["DaftarNumber"] != null)
            {
                if (Utility.isValidRetired(Int64.Parse(Convert.ToString(Session["RetiredNationCode"])), Int64.Parse(Convert.ToString(Session["DaftarNumber"]))) == "1")
                {
                    reserveObject.amount = RackPriceAfterGiftCard.ToString();
                    ViewBag.TotalPriceAfterGiftcard = (RackPriceAfterGiftCard / 2).ToString();
                   // Session["RackPriceAfterGiftCard"] = RackPriceAfterGiftCard / 2;
                }
            }
            return base.View(reserveObject);
        }
        public ActionResult Applied()
        {
            DiscountModelForCard roomCountAfterCardDiscount = (DiscountModelForCard)Session["roomCountAfterCardDiscount"];
            string text = Globals.NavigateURL(base.PortalSettings.ActiveTab.TabID, "Applied", new string[]
            {
                "mid=" + base.ActiveModule.ModuleID,
                "popUp=true"
            });
            string AmountFromCookie = 0.ToString();
            bool flag = base.Request.Cookies["Prc"] != null;
            if (flag)
            {
                AmountFromCookie = base.Request.Cookies["Prc"].Value;
            }
            int ReserveID = int.Parse(base.Request.Cookies["rid"].Value);
            Api api = new Api();
            api = api.InitializeApi(base.PortalSettings.PortalId);
            respgetReserveInfo respgetReserveInfo = new respgetReserveInfo();
            respgetReserveInfo = api.GetReserveInfo(ReserveID);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PassengerReserveInfo passengerReserveInfo in respgetReserveInfo.response.passenger)
            {
                stringBuilder.Append(passengerReserveInfo.roomName + ",");
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            string OrderID;
            if (Session["OrderID"] == null)
                OrderID = base.Request["OrderID"];
            else
                OrderID = base.Session["OrderID"].ToString();
            ActionResult result;
            if (Request.Params["state"] != null)
            {
                PaymentModel paymentModel = new PaymentModel();
                if (AmountFromCookie != "")
                {
                    paymentModel.Amount = AmountFromCookie;
                }
                bool flag5 = base.Request.Form["state"] != null;
                if (flag5)
                {
                    paymentModel.State = base.Request.Form["state"].ToString();
                }
                bool flag6 = base.Request.Form["StateCode"] != null;
                if (flag6)
                {
                    paymentModel.StateCode = base.Request.Form["StateCode"].ToString();
                }
                bool flag7 = base.Request.Form["ResNum"] != null;
                if (flag7)
                {
                    paymentModel.ResNum = base.Request.Form["ResNum"].ToString();
                }
                bool flag8 = base.Request.Form["MID"] != null;
                if (flag8)
                {
                    paymentModel.MID = base.Request.Form["MID"].ToString();
                }
                bool flag9 = base.Request.Form["RefNum"] != null;
                if (flag9)
                {
                    paymentModel.RefNum = base.Request.Form["RefNum"].ToString();
                }
                bool flag10 = base.Request.Form["CID"] != null;
                if (flag10)
                {
                    paymentModel.CID = base.Request.Form["CID"].ToString();
                }
                bool flag11 = base.Request.Form["TraceNo"] != null;
                if (flag11)
                {
                    paymentModel.TRACENO = base.Request.Form["TraceNo"].ToString();
                }
                bool flag12 = base.Request.Form["RRN"] != null;
                if (flag12)
                {
                    paymentModel.RRN = base.Request.Form["RRN"].ToString();
                }
                bool flag13 = base.Request.Form["SecurePan"] != null;
                if (flag13)
                {
                    paymentModel.SecurePan = base.Request.Form["SecurePan"].ToString();
                }
                bool flag14 = paymentModel.RefNum.Equals(string.Empty) && paymentModel.State.Equals(string.Empty);
                string PdfLink;
                if (flag14)
                {
                    paymentModel.Message = "مشکلی در تراکنش توسط خریدار بوجود آمده است.";
                }
                else
                {
                    bool flag15 = paymentModel.ResNum.Equals(string.Empty) && paymentModel.State.Equals(string.Empty);
                    if (flag15)
                    {
                        paymentModel.Message = "خطا در برقراری ارتباط با بانک";
                    }
                    else
                    {
                        bool flag16 = paymentModel.ResNum != OrderID;
                        if (flag16)
                        {
                            paymentModel.Message = "شماره فاکتور نامعتبر می باشد !";
                        }
                        else
                        {
                            if (paymentModel.State.Equals("OK"))
                            {
                                double AmounBankReturned = 0;
                                //Zero Payment
                                if (AmountFromCookie != "0")
                                {
                                    ReferencePayment1 referencePayment = new ReferencePayment1();
                                    AmounBankReturned = referencePayment.verifyTransaction(paymentModel.RefNum, paymentModel.MID);
                                    if (!(AmounBankReturned > 0.0))
                                    {
                                        paymentModel.Successful = false;
                                        paymentModel.Message = this.GetErrorInfo((int)AmounBankReturned);
                                        VoucherModel model = new VoucherModel
                                        {
                                            PaymentModel = paymentModel,
                                            ReserveInfo = respgetReserveInfo,
                                            lstPassangers = this.GetPassanger(ReserveID)
                                        };
                                        return base.View(model);
                                    }
                                }
                                bool flag19 = AmountFromCookie != "";
                                if (flag19)
                                {
                                    paymentModel.Amount = AmountFromCookie;
                                }
                                bool flag20 = AmounBankReturned == (double)long.Parse(paymentModel.Amount);
                                if (!flag20 && AmountFromCookie != "0")
                                {
                                    ReferencePayment1 referencePayment = new ReferencePayment1();
                                    AmounBankReturned = referencePayment.reverseTransaction(paymentModel.RefNum, paymentModel.MID, paymentModel.MID, "Ittic@SEP79016394");
                                    bool flag21 = AmounBankReturned == 1.0;
                                    if (flag21)
                                    {
                                        paymentModel.Message = "به علت وجود خطا ، مبلغ تراکنش برگشت داده شد!";
                                    }
                                    else
                                    {
                                        bool flag22 = AmounBankReturned == -1.0;
                                        if (flag22)
                                        {
                                            paymentModel.Message = "خطا در برگشت مبلغ تراکنش به حساب خریدار!";
                                        }
                                    }
                                    VoucherModel voucherModel = new VoucherModel
                                    {
                                        PaymentModel = paymentModel,
                                        ReserveInfo = respgetReserveInfo,
                                        lstPassangers = this.GetPassanger(ReserveID)
                                    };
                                    bool flag23 = string.IsNullOrEmpty(paymentModel.Amount);
                                    if (flag23)
                                    {
                                        paymentModel.Amount = "0";
                                    }
                                    this.AddReservInfo(respgetReserveInfo);
                                    PdfLink = this.CreatPDF(voucherModel, paymentModel.Successful);
                                    bool successful = paymentModel.Successful;
                                    if (successful)
                                    {
                                        this.sendEmial(new List<string>
                                        {
                                            base.PortalSettings.HomeDirectoryMapPath + PdfLink
                                        }, voucherModel);
                                    }
                                    this.AddLog(respgetReserveInfo, paymentModel, PdfLink, stringBuilder.ToString());
                                    return base.View(voucherModel);
                                }

                                paymentModel.Message = "پرداخت با موفقیت انجام شد.";
                                paymentModel.Successful = true;
                                this.AddPaymentModel(paymentModel, ReserveID);
                                resfinalize resfinalize = api.finalize(ReserveID);
                                this.AddFinalizeInfo(resfinalize, ReserveID);
                                bool flag24 = !resfinalize.status;
                                if (resfinalize.status)
                                {
                                    Utility.FinalizeRetiredCredit(ReserveID.ToString());
                                    (new Cards()).FinalizeCard(ReserveID.ToString());
                                }
                                if (flag24)
                                {
                                    bool flag25 = api.isFinalize(ReserveID).response.finalize == 0;
                                    if (flag25)
                                    {
                                        Utility.SendSMS(new string[]
                                        {
                                            "09362155476",
                                            respgetReserveInfo.response.mobile
                                        }, "رزرو شما نهایی نشد در صورتیکه مبلغ از حساب شما کسر شده است، مبلغ به حساب شما عودت می شود");
                                        if (AmountFromCookie != "0")
                                        {
                                            ReferencePayment1 referencePayment = new ReferencePayment1();
                                            AmounBankReturned = referencePayment.reverseTransaction(paymentModel.RefNum, paymentModel.MID, paymentModel.MID, "Ittic@SEP79016394");
                                            bool flag26 = AmounBankReturned == 1.0;
                                            if (flag26)
                                            {
                                                paymentModel.Message = "عملیات رزرو ناموفق بود و هزینه به حساب شما بازگشت داده شد!";
                                                paymentModel.Successful = false;
                                            }
                                            else
                                            {
                                                bool flag27 = AmounBankReturned == -1.0;
                                                if (flag27)
                                                {
                                                    paymentModel.Message = "خطا در برگشت مبلغ تراکنش به حساب خریدار!";
                                                    paymentModel.Successful = false;
                                                }
                                                else
                                                {
                                                    paymentModel.Message = "عملیات برگشت وجه ناموفق بود ! " + this.GetErrorInfo(int.Parse(AmounBankReturned.ToString()));
                                                    paymentModel.Successful = false;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        paymentModel.Message = "شما درخواست را مجدد ارسال کرده اید و قبلا رزرو انجام شده است.";
                                        paymentModel.Successful = true;
                                    }
                                    bool flag28 = string.IsNullOrEmpty(paymentModel.Amount);
                                    if (flag28)
                                    {
                                        paymentModel.Amount = "0";
                                    }
                                    this.AddReservInfo(respgetReserveInfo);
                                    resreserve reserve = this.GetReserve(ReserveID.ToString());
                                    bool flag29 = string.IsNullOrEmpty(reserve.response.reserve.info.thirdPartyCode);
                                    if (flag29)
                                    {
                                        reserve.response.reserve.info.thirdPartyCode = "0";
                                    }
                                    List<ReservedRoom> list = new List<ReservedRoom>();
                                    using (List<PassengerReserveInfo>.Enumerator enumerator2 = respgetReserveInfo.response.passenger.GetEnumerator())
                                    {
                                        while (enumerator2.MoveNext())
                                        {
                                            PassengerReserveInfo item = enumerator2.Current;
                                            //bool flag30 = list.Any((ReservedRoom p) => p.Title == item.roomName);
                                            if (list.Any((ReservedRoom p) => p.Title == item.roomName))
                                            {
                                                int TotalPriceAmountFromAPI = 0;
                                                int TotalRackPriceAmountFromAPI = 0;
                                                foreach (DayPriceReserveInfo dayPriceReserveInfo in item.dayPrice)
                                                {
                                                    TotalPriceAmountFromAPI += dayPriceReserveInfo.price;
                                                    TotalRackPriceAmountFromAPI += dayPriceReserveInfo.rackRate;
                                                }
                                                (from p in list
                                                 where p.Title == item.roomName
                                                 select p).First<ReservedRoom>().Count = (from p in list
                                                                                          where p.Title == item.roomName
                                                                                          select p).First<ReservedRoom>().Count + 1;
                                                (from p in list
                                                 where p.Title == item.roomName
                                                 select p).First<ReservedRoom>().Price = (from p in list
                                                                                          where p.Title == item.roomName
                                                                                          select p).First<ReservedRoom>().Price + TotalPriceAmountFromAPI;
                                                (from p in list
                                                 where p.Title == item.roomName
                                                 select p).First<ReservedRoom>().RackPrice = (from p in list
                                                                                              where p.Title == item.roomName
                                                                                              select p).First<ReservedRoom>().RackPrice + TotalRackPriceAmountFromAPI;
                                            }
                                            else
                                            {
                                                int num5 = 0;
                                                int num6 = 0;
                                                foreach (DayPriceReserveInfo dayPriceReserveInfo2 in item.dayPrice)
                                                {
                                                    num5 += dayPriceReserveInfo2.price;
                                                    num6 += dayPriceReserveInfo2.rackRate;
                                                }
                                                list.Add(new ReservedRoom
                                                {
                                                    Title = item.roomName,
                                                    Count = 1,
                                                    Price = num5,
                                                    RackPrice = num6
                                                });
                                            }
                                        }
                                    }
                                    resfinalize = this.GetFinalizeInfo(ReserveID.ToString());
                                    VoucherModel voucherModel2 = new VoucherModel
                                    {
                                        PaymentModel = paymentModel,
                                        ReserveInfo = respgetReserveInfo,
                                        lstPassangers = this.GetPassanger(ReserveID),
                                        lstRooms = list,
                                        finalize = resfinalize,
                                        RequestReserve = reserve
                                    };
                                    PdfLink = this.CreatPDF(voucherModel2, paymentModel.Successful);
                                    bool flag31 = voucherModel2.finalize.response != null && string.IsNullOrEmpty(voucherModel2.finalize.response.thirdPartyCode);
                                    if (flag31)
                                    {
                                        voucherModel2.finalize.response.thirdPartyCode = reserve.response.reserve.info.thirdPartyCode.ToString();
                                    }
                                    bool successful2 = paymentModel.Successful;
                                    if (successful2)
                                    {
                                        this.sendEmial(new List<string>
                                        {
                                            base.PortalSettings.HomeDirectoryMapPath + PdfLink
                                        }, voucherModel2);
                                        StringBuilder stringBuilder3 = new StringBuilder();
                                        stringBuilder3.Append("آقا/خانم " + voucherModel2.ReserveInfo.response.firstname + " " + voucherModel2.ReserveInfo.response.lastname);
                                        stringBuilder3.Append(Environment.NewLine);
                                        stringBuilder3.Append(string.Concat(new string[]
                                        {
                                            "رزرو شما  به مدت ",
                                            voucherModel2.ReserveInfo.response.passenger[0].night,
                                            " شب در ",
                                            base.LocalizeString(voucherModel2.ReserveInfo.response.hotel),
                                            " انجام شد "
                                        }));
                                        stringBuilder3.Append(Environment.NewLine);
                                        stringBuilder3.Append("کد رزرو: " + voucherModel2.ReserveInfo.response.id);
                                        stringBuilder3.Append(Environment.NewLine);
                                        stringBuilder3.Append("شماره سفارش: " + reserve.response.reserve.info.thirdPartyCode.ToString());
                                        stringBuilder3.Append(Environment.NewLine);
                                        stringBuilder3.Append("گروه هتلهای ایرانگردی و جهانگردی www.ittic.com");
                                        Utility.SendSMS(new string[]
                                        {
                                            respgetReserveInfo.response.mobile
                                        }, stringBuilder3.ToString());
                                    }
                                    this.AddLog(respgetReserveInfo, paymentModel, PdfLink, stringBuilder.ToString());
                                    return base.View(voucherModel2);
                                }
                                List<ReservedRoom> list2 = new List<ReservedRoom>();
                                using (List<PassengerReserveInfo>.Enumerator enumerator5 = respgetReserveInfo.response.passenger.GetEnumerator())
                                {
                                    while (enumerator5.MoveNext())
                                    {
                                        PassengerReserveInfo item = enumerator5.Current;
                                        bool flag32 = list2.Any((ReservedRoom p) => p.Title == item.roomName);
                                        if (flag32)
                                        {
                                            int num7 = 0;
                                            int num8 = 0;
                                            foreach (DayPriceReserveInfo dayPriceReserveInfo3 in item.dayPrice)
                                            {
                                                num7 += dayPriceReserveInfo3.price;
                                                num8 += dayPriceReserveInfo3.rackRate;
                                            }
                                            (from p in list2
                                             where p.Title == item.roomName
                                             select p).First<ReservedRoom>().Count = (from p in list2
                                                                                      where p.Title == item.roomName
                                                                                      select p).First<ReservedRoom>().Count + 1;
                                            (from p in list2
                                             where p.Title == item.roomName
                                             select p).First<ReservedRoom>().Price = (from p in list2
                                                                                      where p.Title == item.roomName
                                                                                      select p).First<ReservedRoom>().Price + num7;
                                            (from p in list2
                                             where p.Title == item.roomName
                                             select p).First<ReservedRoom>().RackPrice = (from p in list2
                                                                                          where p.Title == item.roomName
                                                                                          select p).First<ReservedRoom>().RackPrice + num8;
                                        }
                                        else
                                        {
                                            int num9 = 0;
                                            int num10 = 0;
                                            foreach (DayPriceReserveInfo dayPriceReserveInfo4 in item.dayPrice)
                                            {
                                                num9 += dayPriceReserveInfo4.price;
                                                num10 += dayPriceReserveInfo4.rackRate;
                                            }
                                            list2.Add(new ReservedRoom
                                            {
                                                Title = item.roomName,
                                                Count = 1,
                                                Price = num9,
                                                RackPrice = num10
                                            });
                                        }
                                    }
                                }
                                bool flag33 = string.IsNullOrEmpty(paymentModel.Amount);
                                if (flag33)
                                {
                                    paymentModel.Amount = "0";
                                }
                                resreserve reserve2 = this.GetReserve(ReserveID.ToString());
                                VoucherModel voucherModel3 = new VoucherModel
                                {
                                    PaymentModel = paymentModel,
                                    ReserveInfo = respgetReserveInfo,
                                    lstPassangers = this.GetPassanger(ReserveID),
                                    lstRooms = list2,
                                    finalize = resfinalize,
                                    RequestReserve = reserve2
                                };
                                bool flag34 = voucherModel3.finalize.response != null && string.IsNullOrEmpty(voucherModel3.finalize.response.thirdPartyCode);
                                if (flag34)
                                {
                                    voucherModel3.finalize.response.thirdPartyCode = reserve2.response.reserve.info.thirdPartyCode.ToString();
                                }
                                this.AddReservInfo(respgetReserveInfo);
                                PdfLink = this.CreatPDF(voucherModel3, paymentModel.Successful);
                                this.AddPaymentModel(paymentModel, ReserveID);
                                bool successful3 = paymentModel.Successful;
                                if (successful3)
                                {
                                    this.sendEmial(new List<string>
                                    {
                                        base.PortalSettings.HomeDirectoryMapPath + PdfLink
                                    }, voucherModel3);
                                    StringBuilder stringBuilder4 = new StringBuilder();
                                    stringBuilder4.Append("آقا/خانم " + voucherModel3.ReserveInfo.response.firstname + " " + voucherModel3.ReserveInfo.response.lastname);
                                    stringBuilder4.Append(Environment.NewLine);
                                    stringBuilder4.Append(string.Concat(new string[]
                                    {
                                        "رزرو شما  به مدت ",
                                        voucherModel3.ReserveInfo.response.passenger[0].night,
                                        " شب در ",
                                        base.LocalizeString(voucherModel3.ReserveInfo.response.hotel),
                                        " انجام شد "
                                    }));
                                    stringBuilder4.Append(Environment.NewLine);
                                    stringBuilder4.Append("کد رزرو: " + voucherModel3.ReserveInfo.response.id);
                                    stringBuilder4.Append(Environment.NewLine);
                                    stringBuilder4.Append("شماره سفارش: " + voucherModel3.finalize.response.thirdPartyCode);
                                    stringBuilder4.Append(Environment.NewLine);
                                    stringBuilder4.Append("گروه هتلهای ایرانگردی و جهانگردی www.ittic.com");
                                    Utility.SendSMS(new string[]
                                    {
                                        respgetReserveInfo.response.mobile
                                    }, stringBuilder4.ToString());
                                }
                                this.AddLog(respgetReserveInfo, paymentModel, PdfLink, stringBuilder.ToString());
                                return base.View(voucherModel3);
                            }
                            else
                            {
                                paymentModel.Successful = false;
                                paymentModel.Message = "متاسفانه بانک خريد شما را تاييد نکرده است";
                                bool flag35 = paymentModel.State.Equals("Canceled By User") || paymentModel.State.Equals(string.Empty);
                                if (flag35)
                                {
                                    paymentModel.Message = "تراكنش توسط خريدار كنسل شد";
                                }
                                else
                                {
                                    bool flag36 = paymentModel.State.Equals("Invalid Amount");
                                    if (flag36)
                                    {
                                        paymentModel.Message = " مبلغ سند برگشتی، از مبلغ تراکنش اصلی بیشتر است. ";
                                    }
                                    else
                                    {
                                        bool flag37 = paymentModel.State.Equals("Invalid Transaction");
                                        if (flag37)
                                        {
                                            paymentModel.Message = " درخواست برگشت یک تراکنش رسیده است،در حالی که تراکنش اصلی پیدا نمی شود ";
                                        }
                                        else
                                        {
                                            bool flag38 = paymentModel.State.Equals("Invalid Card Number");
                                            if (flag38)
                                            {
                                                paymentModel.Message = " شماره کارت اشتباه است ";
                                            }
                                            else
                                            {
                                                bool flag39 = paymentModel.State.Equals("No Such Issuer");
                                                if (flag39)
                                                {
                                                    paymentModel.Message = " چنین صادر کننده کارتی وجود ندارد ";
                                                }
                                                else
                                                {
                                                    bool flag40 = paymentModel.State.Equals("Expired Card Pick Up");
                                                    if (flag40)
                                                    {
                                                        paymentModel.Message = " از تاریخ انقضای کارت گذشته است و کارت دیگر معتبر نیست ";
                                                    }
                                                    else
                                                    {
                                                        bool flag41 = paymentModel.State.Equals("Allowable PIN Tries Exceeded Pick Up");
                                                        if (flag41)
                                                        {
                                                            paymentModel.Message = " رمز کارت 3 مرتبه اشتباه وارد شده است در نتیجه کارت غیر فعال خواهد شد ";
                                                        }
                                                        else
                                                        {
                                                            bool flag42 = paymentModel.State.Equals("Incorrect PIN");
                                                            if (flag42)
                                                            {
                                                                paymentModel.Message = " خریدار رمز کارت را اشتباه وارد کرده است ";
                                                            }
                                                            else
                                                            {
                                                                bool flag43 = paymentModel.State.Equals("Exceeds Withdrawal Amount Limit");
                                                                if (flag43)
                                                                {
                                                                    paymentModel.Message = " مبلغ بیش از سقف برداشت می باشد ";
                                                                }
                                                                else
                                                                {
                                                                    bool flag44 = paymentModel.State.Equals("Transaction Cannot Be Completed");
                                                                    if (flag44)
                                                                    {
                                                                        paymentModel.Message = " امکان سند خوردن وجود ندارد ";
                                                                    }
                                                                    else
                                                                    {
                                                                        bool flag45 = paymentModel.State.Equals("Response Received Too Late");
                                                                        if (flag45)
                                                                        {
                                                                            paymentModel.Message = " جلسه کاری شما پایان یافته است ";
                                                                        }
                                                                        else
                                                                        {
                                                                            bool flag46 = paymentModel.State.Equals("Suspected Fraud Pick Up");
                                                                            if (flag46)
                                                                            {
                                                                                paymentModel.Message = " خریدار یا فیلد CVV2  ویا فیلد ExpDate را اشتباه زده است (یا اصلا وارد نکرده است) ";
                                                                            }
                                                                            else
                                                                            {
                                                                                bool flag47 = paymentModel.State.Equals("No Sufficient Funds");
                                                                                if (flag47)
                                                                                {
                                                                                    paymentModel.Message = " موجودی به اندازه کافی در حساب وجود ندارد ";
                                                                                }
                                                                                else
                                                                                {
                                                                                    bool flag48 = paymentModel.State.Equals("Issuer Down Slm");
                                                                                    if (flag48)
                                                                                    {
                                                                                        paymentModel.Message = " سیستم کارت بانک صادرکننده در وضعیت عملیاتی نیست ";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        bool flag49 = paymentModel.State.Equals("TME Error");
                                                                                        if (flag49)
                                                                                        {
                                                                                            paymentModel.Message = " خطای بانکی ";
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                Utility.SendSMS(new string[]
                                {
                                    "09362155476",
                                    respgetReserveInfo.response.mobile
                                }, "متاسفانه عملیات پرداخت یا رزرو شما ناموفق بود، با تشکر " + Environment.NewLine + "گروه هتلهای ایرانگردی و جهانگردی www.ittic.com");
                            }
                        }
                    }
                }
                bool flag50 = string.IsNullOrEmpty(paymentModel.Amount);
                if (flag50)
                {
                    paymentModel.Amount = "0";
                }
                VoucherModel voucherModel4 = new VoucherModel
                {
                    PaymentModel = paymentModel,
                    ReserveInfo = respgetReserveInfo,
                    lstPassangers = this.GetPassanger(ReserveID)
                };
                this.AddReservInfo(respgetReserveInfo);
                PdfLink = this.CreatPDF(voucherModel4, paymentModel.Successful);
                bool successful4 = paymentModel.Successful;
                if (successful4)
                {
                    this.sendEmial(new List<string>
                    {
                        base.PortalSettings.HomeDirectoryMapPath + PdfLink
                    }, voucherModel4);
                }
                this.AddLog(respgetReserveInfo, paymentModel, PdfLink, stringBuilder.ToString());
                result = base.View(voucherModel4);
            }
            else
            {
                string text4 = DateTime.Now.ToString("yMMddhhmmss");
                base.Session["OrderID"] = text4;
                bool flag51 = base.Request.Cookies["Prc"] != null;
                if (flag51)
                {
                    string PriceFromCookie = base.Request.Cookies["Prc"].Value;
                    decimal PriceFromReserve = Utility.getAmountFromReserveInfo(respgetReserveInfo);
                    decimal RackPriceFromReserve = Utility.GetRackAmount(respgetReserveInfo);
                    //DiscountModelForCard roomCountAfterCardDiscount = (DiscountModelForCard)Session["roomCountAfterCardDiscount"];
                    PriceFromReserve = PriceFromReserve - Utility.GetDiscountAmountGiftCard(respgetReserveInfo, roomCountAfterCardDiscount);
                    if (Session["ValidRetired"] != null && Session["RetiredNationCode"] != null && Session["DaftarNumber"] != null)
                    {
                        if (Utility.isValidRetired(Int64.Parse(Convert.ToString(Session["RetiredNationCode"])), Int64.Parse(Convert.ToString(Session["DaftarNumber"]))) == "1")
                            PriceFromReserve = RackPriceFromReserve / 2;
                    }
                    //var RetiredDiscount = Utility.GetRetiredDiscountofReserve(respgetReserveInfo.response.id);
                    //bool flag52;

                    //if (RetiredDiscount != null)
                    //{
                    //    PriceFromReserve = PriceFromReserve - RetiredDiscount.DiscountAmount;
                    //    flag52 = PriceFromReserve.ToString() == PriceFromCookie;
                    //}
                    //else
                      bool  flag52 = PriceFromReserve.ToString() == PriceFromCookie;
                    //if (roomCountAfterCardDiscount.Amount.ToString() != PriceFromCookie)
                    //{
                    //    PriceFromCookie = roomCountAfterCardDiscount.Amount.ToString();
                    //}
                    //No Need To Payment 
                    if (flag52 && PriceFromReserve == 0 /*&& roomCountAfterCardDiscount.Amount == 0 */)
                    {
                        stringBuilder2.AppendFormat("<script>");
                        stringBuilder2.AppendFormat("var form = document.createElement('form');");
                        stringBuilder2.AppendFormat("form.setAttribute('method', 'POST');");
                        stringBuilder2.AppendFormat("form.setAttribute('action', '{0}');", Globals.NavigateURL(base.PortalSettings.ActiveTab.TabID, "Applied", new string[]
                        {
                            "mid=" + base.ActiveModule.ModuleID,
                            "popUp=true",
                            "OrderID=" + text4
                        }));
                        stringBuilder2.AppendFormat("form.setAttribute('target', '_self');");
                        stringBuilder2.AppendFormat("var SecurePan  = document.createElement('input');");
                        stringBuilder2.AppendFormat("SecurePan.setAttribute('name', 'SecurePan');");
                        stringBuilder2.AppendFormat("SecurePan.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(SecurePan);");
                        stringBuilder2.AppendFormat("var RRN  = document.createElement('input');");
                        stringBuilder2.AppendFormat("RRN.setAttribute('name', 'RRN');");
                        stringBuilder2.AppendFormat("RRN.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(RRN);");
                        stringBuilder2.AppendFormat("var TRACENO  = document.createElement('input');");
                        stringBuilder2.AppendFormat("TRACENO.setAttribute('name', 'TRACENO');");
                        stringBuilder2.AppendFormat("TRACENO.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(TRACENO);");
                        stringBuilder2.AppendFormat("var CID  = document.createElement('input');");
                        stringBuilder2.AppendFormat("CID.setAttribute('name', 'CID');");
                        stringBuilder2.AppendFormat("CID.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(CID);");
                        stringBuilder2.AppendFormat("var RefNum  = document.createElement('input');");
                        stringBuilder2.AppendFormat("RefNum.setAttribute('name', 'RefNum');");
                        stringBuilder2.AppendFormat("RefNum.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(RefNum);");
                        stringBuilder2.AppendFormat("var ResNum  = document.createElement('input');");
                        stringBuilder2.AppendFormat("ResNum.setAttribute('name', 'ResNum');");
                        stringBuilder2.AppendFormat("ResNum.setAttribute('value', '{0}');", text4);
                        stringBuilder2.AppendFormat("form.appendChild(ResNum);");
                        stringBuilder2.AppendFormat("var State  = document.createElement('input');");
                        stringBuilder2.AppendFormat("State.setAttribute('name', 'State');");
                        stringBuilder2.AppendFormat("State.setAttribute('value', 'OK');");
                        stringBuilder2.AppendFormat("form.appendChild(State);");
                        stringBuilder2.AppendFormat("var StateCode  = document.createElement('input');");
                        stringBuilder2.AppendFormat("StateCode.setAttribute('name', 'StateCode');");
                        stringBuilder2.AppendFormat("StateCode.setAttribute('value', '0');");
                        stringBuilder2.AppendFormat("form.appendChild(StateCode);");
                        stringBuilder2.AppendFormat("var MID  = document.createElement('input');");
                        stringBuilder2.AppendFormat("MID.setAttribute('name', 'MID');");
                        stringBuilder2.AppendFormat("MID.setAttribute('value', '{0}');", "10127825");
                        stringBuilder2.AppendFormat("form.appendChild(MID);");

                        stringBuilder2.AppendFormat("document.body.appendChild(form);");
                        stringBuilder2.AppendFormat("form.submit();");
                        stringBuilder2.AppendFormat("document.body.removeChild(form);");
                        stringBuilder2.AppendFormat("</script>");
                    }
                    //Go To Bank
                    else if (flag52)
                    {
                        stringBuilder2.AppendFormat("<script>");
                        stringBuilder2.AppendFormat("var form = document.createElement('form');");
                        stringBuilder2.AppendFormat("form.setAttribute('method', 'POST');");
                        stringBuilder2.AppendFormat("form.setAttribute('action', '{0}');", "https://sep.shaparak.ir/payment.aspx");
                        stringBuilder2.AppendFormat("form.setAttribute('target', '_self');");
                        stringBuilder2.AppendFormat("var ResNum  = document.createElement('input');");
                        stringBuilder2.AppendFormat("ResNum.setAttribute('name', 'ResNum');");
                        stringBuilder2.AppendFormat("ResNum.setAttribute('value', '{0}');", text4);
                        stringBuilder2.AppendFormat("form.appendChild(ResNum);");
                        stringBuilder2.AppendFormat("var MID  = document.createElement('input');");
                        stringBuilder2.AppendFormat("MID.setAttribute('name', 'MID');");
                        stringBuilder2.AppendFormat("MID.setAttribute('value', '{0}');", "10127825");
                        stringBuilder2.AppendFormat("form.appendChild(MID);");
                        stringBuilder2.AppendFormat("var RedirectURL  = document.createElement('input');");
                        stringBuilder2.AppendFormat("RedirectURL.setAttribute('name', 'RedirectURL');");
                        stringBuilder2.AppendFormat("RedirectURL.setAttribute('value', '{0}');", Globals.NavigateURL(base.PortalSettings.ActiveTab.TabID, "Applied", new string[]
                        {
                            "mid=" + base.ActiveModule.ModuleID,
                            "popUp=true",
                            "OrderID=" + text4
                        }));
                        stringBuilder2.AppendFormat("form.appendChild(RedirectURL);");
                        stringBuilder2.AppendFormat("var Amount  = document.createElement('input');");
                        stringBuilder2.AppendFormat("Amount.setAttribute('name', 'Amount');");
                        stringBuilder2.AppendFormat("Amount.setAttribute('value', '{0}');", PriceFromCookie);
                        stringBuilder2.AppendFormat("form.appendChild(Amount);");
                        stringBuilder2.AppendFormat("document.body.appendChild(form);");

                        stringBuilder2.AppendFormat("form.submit();");
                        stringBuilder2.AppendFormat("document.body.removeChild(form);");
                        stringBuilder2.AppendFormat("</script>");
                    }
                    else
                    {
                        stringBuilder2.AppendFormat("<script>");
                        stringBuilder2.AppendFormat("alert('در محاسبه خطایی پیش آمده است.');");
                        stringBuilder2.AppendFormat("</script>");
                    }
                    ViewBag.BankScript = stringBuilder2.ToString();
                }
                ReserveObject reserveObject = new ReserveObject();
                reserveObject = (ReserveObject)base.Session["ReserveObject"];
                VoucherModel model2 = new VoucherModel
                {
                    PaymentModel = null,
                    ReserveInfo = respgetReserveInfo,
                    lstPassangers = this.GetPassanger(ReserveID)
                };
                Utility.runQuery(string.Concat(new string[]
                {
                    "INSERT INTO [dbo].[Pay]([Name],[Family],[Price],[PayStatus],[Mobile],[OrderId],[date]) VALUES (N'",
                    respgetReserveInfo.response.firstname,
                    "',N'",
                    respgetReserveInfo.response.lastname,
                    "',",
                    reserveObject.amount,
                    ",N'ارسال به بانک جهت پرداخت',N'",
                    respgetReserveInfo.response.mobile,
                    "',N'",
                    text4,
                    "',GETDATE())"
                }));
                result = base.View(model2);
            }
            return result;
        }
        public ActionResult GetApplied()
        {
            bool flag = base.Session["ViewRID"] != null;
            int reserveID;
            if (flag)
            {
                reserveID = Convert.ToInt32(base.Session["ViewRID"].ToString());
            }
            else
            {
                reserveID = Convert.ToInt32(base.Request["ViewRID"]);
            }
            List<ReservedRoom> list = new List<ReservedRoom>();
            respgetReserveInfo reservInfo = this.GetReservInfo(reserveID.ToString());
            using (List<PassengerReserveInfo>.Enumerator enumerator = reservInfo.response.passenger.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PassengerReserveInfo item = enumerator.Current;
                    bool flag2 = list.Any((ReservedRoom p) => p.Title == item.roomName);
                    if (flag2)
                    {
                        int num = 0;
                        int num2 = 0;
                        foreach (DayPriceReserveInfo dayPriceReserveInfo in item.dayPrice)
                        {
                            num += dayPriceReserveInfo.price;
                            num2 += dayPriceReserveInfo.rackRate;
                        }
                        (from p in list
                         where p.Title == item.roomName
                         select p).First<ReservedRoom>().Count = (from p in list
                                                                  where p.Title == item.roomName
                                                                  select p).First<ReservedRoom>().Count + 1;
                        (from p in list
                         where p.Title == item.roomName
                         select p).First<ReservedRoom>().Price = (from p in list
                                                                  where p.Title == item.roomName
                                                                  select p).First<ReservedRoom>().Price + num;
                        (from p in list
                         where p.Title == item.roomName
                         select p).First<ReservedRoom>().RackPrice = (from p in list
                                                                      where p.Title == item.roomName
                                                                      select p).First<ReservedRoom>().RackPrice + num2;
                    }
                    else
                    {
                        int num3 = 0;
                        int num4 = 0;
                        foreach (DayPriceReserveInfo dayPriceReserveInfo2 in item.dayPrice)
                        {
                            num3 += dayPriceReserveInfo2.price;
                            num4 += dayPriceReserveInfo2.rackRate;
                        }
                        list.Add(new ReservedRoom
                        {
                            Title = item.roomName,
                            Count = 1,
                            Price = num3,
                            RackPrice = num4
                        });
                    }
                }
            }
            resreserve reserve = this.GetReserve(reserveID.ToString());
            VoucherModel voucherModel = new VoucherModel
            {
                PaymentModel = this.GetPaymentModel(reserveID),
                ReserveInfo = reservInfo,
                lstPassangers = this.GetPassanger(reserveID),
                lstRooms = list,
                finalize = this.GetFinalizeInfo(reserveID.ToString()),
                RequestReserve = reserve
            };
            bool flag3 = voucherModel.finalize.response != null && string.IsNullOrEmpty(voucherModel.finalize.response.thirdPartyCode);
            if (flag3)
            {
                voucherModel.finalize.response.thirdPartyCode = reserve.response.reserve.info.thirdPartyCode.ToString();
            }
            this.CreatPDF(voucherModel, voucherModel.PaymentModel.Successful);
            return base.View("Applied", voucherModel);
        }
        private string GetErrorInfo(int ErrorNo)
        {
            string result = "";
            switch (ErrorNo)
            {
                case -18:
                    result = " IP Address فروشنده نا معتبر است -18";
                    break;
                case -17:
                    result = " برگشت زدن جزیی تراکنش مجاز نمی باشد -17";
                    break;
                case -16:
                    result = " خطای داخلی سیستم -16";
                    break;
                case -15:
                    result = " مبلغ برگشتی به صورت اعشاری داده شده است -15";
                    break;
                case -14:
                    result = " چنین تراکنشی تعریف نشده است -14";
                    break;
                case -13:
                    result = " مبلغ برگشتی برای برگشت جز ئی بیش از مبلغ برگشت نخورده ی رسید دیجیتالی است  -13";
                    break;
                case -12:
                    result = " مبلغ برگشتی منفی است -12";
                    break;
                case -11:
                    result = " طول ورودی ها کمتر از حد مجاز است -11";
                    break;
                case -10:
                    result = " رسید دیجیتالی به صورت Base64 نیست )حاوی کارکترهای غیرمجاز است( -10";
                    break;
                case -9:
                    result = " وجود کارکترهای غیرمجاز در مبلغ برگشتی -9";
                    break;
                case -8:
                    result = " طول ورودی ها بیشتر از حد مجاز است.-8";
                    break;
                case -7:
                    result = " رسید دیجیتالی تهی است -7";
                    break;
                case -5:
                    result = " سند قبلا برگشت کامل یافته است -5";
                    break;
                case -4:
                    result = " کلمه عبور یا کد فروشنده اشتباه است -4";
                    break;
                case -3:
                    result = " ورودی ها حاوی کارکترهای غیرمجاز می باشند -3";
                    break;
                case -2:
                    result = " بروز خطا در هنگام تاييد رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد -2";
                    break;
                case -1:
                    result = " خطای در پردازش اطلاعات ارسالی -1";
                    break;
            }
            return result;
        }
        private string sendEmial(List<string> Files, VoucherModel objVoucher)
        {
            try
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(objVoucher.ReserveInfo.response.email);
                }
                catch
                {
                    return "";
                }
                string subject = "گزارش رزرو مجموعه هتل های ایرانگردی و جهانگردی";
                string body = "درود، ضمن تشکر از حسن انتخاب شما و آرزوی بهترین ها برای شما - واچر رزرو هتل انتخابی را می توانید از پیوست دریافت نمایید  - مجموعه هتل های ایرانگردی و جهانگردی";
                foreach (string attachment in Files)
                {
                    string text = Mail.SendMail(base.PortalSettings.Email, "moslem7026@gmail.com", "", "", DotNetNuke.Services.Mail.MailPriority.High, subject, MailFormat.Html, Encoding.UTF8, body, attachment, "", "", "", "");
                    text = Mail.SendMail(base.PortalSettings.Email, objVoucher.ReserveInfo.response.email, "", "", DotNetNuke.Services.Mail.MailPriority.High, subject, MailFormat.Html, Encoding.UTF8, body, attachment, "", "", "", "");
                    text = Mail.SendMail(base.PortalSettings.Email, "reserve@ittic.com", "", "", DotNetNuke.Services.Mail.MailPriority.High, subject, MailFormat.Html, Encoding.UTF8, body, attachment, "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        private string LocalResourceFileNew
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
        private string CreatPDF(VoucherModel objVoucher, bool Finzlized)
        {
            string result;
            try
            {
                StiReport stiReport = new StiReport();
                List<ReserveController.stimul> list = new List<ReserveController.stimul>();
                foreach (ReservedRoom reservedRoom in objVoucher.lstRooms)
                {
                    list.Add(new ReserveController.stimul
                    {
                        Count = reservedRoom.Count,
                        Price = reservedRoom.Price,
                        Discount = Convert.ToInt32(100m - reservedRoom.Price * 100m / reservedRoom.RackPrice),
                        Title = reservedRoom.Title
                    });
                }
                stiReport.RegData("ReservedRoom", list);
                StiFontCollection.AddFontFile(base.Server.MapPath("~/DesktopModules/MVC/GDSModuleMVC/content/stimule/BYekan.ttf"));
                StiFontCollection.AddFontFile(base.Server.MapPath("~/DesktopModules/MVC/GDSModuleMVC/content/stimule/BTitrBold.ttf"));
                stiReport.Load(base.Server.MapPath("~/DesktopModules/MVC/GDSModuleMVC/content/stimule/Report.mrt"));
                stiReport.Dictionary.Variables["Amount"].Value = objVoucher.PaymentModel.Amount;
                stiReport.Dictionary.Variables["hotelTelForPassenger"].Value = objVoucher.ReserveInfo.response.hotelTelForPassenger;
                stiReport.Dictionary.Variables["hotelCheckInDate"].Value = Utility.MiladiToShamsi(objVoucher.ReserveInfo.response.passenger.First<PassengerReserveInfo>().checkin);
                stiReport.Dictionary.Variables["hotelCheckOutDate"].Value = DateTime.Parse(Utility.MiladiToShamsi(objVoucher.ReserveInfo.response.passenger.First<PassengerReserveInfo>().checkin)).AddDays((double)int.Parse(objVoucher.ReserveInfo.response.passenger.First<PassengerReserveInfo>().night)).ToString("yyyy/MM/dd");
                stiReport.Dictionary.Variables["firstnamelastname"].Value = objVoucher.ReserveInfo.response.firstname + " " + objVoucher.ReserveInfo.response.lastname;
                stiReport.Dictionary.Variables["tel"].Value = objVoucher.ReserveInfo.response.mobile;
                stiReport.Dictionary.Variables["id"].Value = objVoucher.ReserveInfo.response.id;
                stiReport.Dictionary.Variables["agency"].Value = objVoucher.ReserveInfo.response.agency;
                stiReport.Dictionary.Variables["Hotel"].Value = Localization.GetString(objVoucher.ReserveInfo.response.hotel, this.LocalResourceFileNew, false);
                stiReport.Dictionary.Variables["thirdPartyCode"].Value = objVoucher.finalize.response.thirdPartyCode;
                stiReport.Dictionary.Variables["refBank"].Value = objVoucher.PaymentModel.RefNum;
                stiReport.Render();
                string text;
                if (Finzlized)
                {
                    text = string.Concat(new object[]
                    {
                        "f_",
                        DateTime.Now.Ticks,
                        "_",
                        objVoucher.ReserveInfo.response.id,
                        ".pdf"
                    });
                }
                else
                {
                    text = string.Concat(new object[]
                    {
                        "n_",
                        DateTime.Now.Ticks,
                        "_",
                        objVoucher.ReserveInfo.response.id,
                        ".pdf"
                    });
                }
                stiReport.ExportDocument(StiExportFormat.Pdf, base.PortalSettings.HomeDirectoryMapPath + text);
                result = text;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }
        private void AddLog(respgetReserveInfo objReserveInfo, PaymentModel paymentModel, string PdfFileName, string roomIDS)
        {
            Utility.runQuery(string.Concat(new string[]
            {
                "INSERT INTO [dbo].[Pay]([Name],[Family],[Price],[PayStatus],[Mobile],[OrderId],[date],[hotelid],[roomid],[reserveID],[Link],[CheckinDate],[Night],[Agency],[CardNo],[ResCode]) VALUES (N'",
                objReserveInfo.response.firstname,
                "',N'",
                objReserveInfo.response.lastname,
                "',",
                paymentModel.Amount,
                ",N'",
                paymentModel.Message,
                "',N'",
                objReserveInfo.response.mobile,
                "',N'",
                paymentModel.ResNum,
                "',GETDATE(),N'",
                objReserveInfo.response.hotel,
                "',N'",
                roomIDS.ToString(),
                "',N'",
                objReserveInfo.response.id,
                "',N'",
                base.PortalSettings.HomeDirectory,
                PdfFileName,
                "',N'",
                objReserveInfo.response.passenger.FirstOrDefault<PassengerReserveInfo>().checkin,
                "',N'",
                objReserveInfo.response.passenger.FirstOrDefault<PassengerReserveInfo>().night,
                "',N'",
                objReserveInfo.response.agency,
                "',N'",
                paymentModel.SecurePan,
                "',N'",
                paymentModel.TRACENO,
                "')"
            }));
        }
        private List<ReservePassanger> GetPassanger(int reserveID)
        {
            List<ReservePassanger> result = new List<ReservePassanger>();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = dataContext.ExecuteQuery<ReservePassanger>(CommandType.Text, "SELECT * FROM [GDS_Passenger] where reserveid ='" + reserveID + "'").ToList<ReservePassanger>();
            }
            return result;
        }
        private void AddReservInfo(respgetReserveInfo ObjReserveInfo)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                bool flag = dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body] from [GDS_ReserveInfoResponse] where [ReserveID]= " + ObjReserveInfo.response.id).Count<string>() == 0;
                if (flag)
                {
                    dataContext.Execute(CommandType.Text, string.Concat(new string[]
                    {
                        "INSERT INTO [GDS_ReserveInfoResponse]([ReserveID],[Body]) VALUES ('",
                        ObjReserveInfo.response.id,
                        "',N'",
                        JsonConvert.SerializeObject(ObjReserveInfo).Replace("@", "@@"),
                        "')"
                    }));
                }
            }
        }
        private respgetReserveInfo GetReservInfo(string ReserveID)
        {
            respgetReserveInfo result = new respgetReserveInfo();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = JsonConvert.DeserializeObject<respgetReserveInfo>(dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body]  from [GDS_ReserveInfoResponse] where [ReserveID]= " + ReserveID).FirstOrDefault<string>());
            }
            return result;
        }
        private void AddFinalizeInfo(resfinalize ObjReserveInfo, int RID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                bool flag = dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body] from [GDS_FinalizeResponse] where [ReserveID]= " + RID).Count<string>() == 0;
                if (flag)
                {
                    dataContext.Execute(CommandType.Text, string.Concat(new object[]
                    {
                        "INSERT INTO [GDS_FinalizeResponse]([ReserveID],[Body]) VALUES ('",
                        RID,
                        "',N'",
                        JsonConvert.SerializeObject(ObjReserveInfo).Replace("@", "@@"),
                        "')"
                    }));
                }
            }
        }
        private resfinalize GetFinalizeInfo(string ReserveID)
        {
            resfinalize result = new resfinalize();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = JsonConvert.DeserializeObject<resfinalize>(dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body]  from [GDS_FinalizeResponse] where [ReserveID]= " + ReserveID).FirstOrDefault<string>());
            }
            return result;
        }
        private void AddPaymentModel(PaymentModel ObjPayment, int ReserveID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                bool flag = dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body] from [GDS_PaymentInfo] where [ReserveID]= " + ReserveID).Count<string>() == 0;
                if (flag)
                {
                    dataContext.Execute(CommandType.Text, string.Concat(new object[]
                    {
                        "INSERT INTO [GDS_PaymentInfo]([ReserveID],[Body]) VALUES ('",
                        ReserveID,
                        "',N'",
                        JsonConvert.SerializeObject(ObjPayment).Replace("@", "@@"),
                        "')"
                    }));
                }
            }
        }
        private PaymentModel GetPaymentModel(int ReserveID)
        {
            PaymentModel result = new PaymentModel();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = JsonConvert.DeserializeObject<PaymentModel>(dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body]  from [GDS_PaymentInfo] where [ReserveID]= " + ReserveID).FirstOrDefault<string>());
            }
            return result;
        }
        private resreserve GetReserve(string ReserveID)
        {
            resreserve result = new resreserve();
            using (IDataContext dataContext = DataContext.Instance())
            {
                result = JsonConvert.DeserializeObject<resreserve>(dataContext.ExecuteQuery<string>(CommandType.Text, "select [Body]  from [GDS_Reserve] where [ReserveID]= " + ReserveID).FirstOrDefault<string>());
            }
            return result;
        }
        public class Passangers
        {
            public string Name { get; set; }
            public string Date { get; set; }
            public string nigh { get; set; }
        }
        private class stimul
        {
            public string Title { get; set; }
            public int Count { get; set; }
            public decimal Price { get; set; }
            public int Discount { get; set; }
        }
    }
}
