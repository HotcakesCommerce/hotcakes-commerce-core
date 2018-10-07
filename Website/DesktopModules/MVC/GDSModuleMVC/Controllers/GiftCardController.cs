using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Moslem.Modules.GDSModuleMVC.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Moslem.Modules.GDSModuleMVC.Controllers
{
    public class GiftCardController : DnnController
    {
        // GET: GiftCard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckGiftCard(string PassKey)
        {
            Cards objCards = new Cards();
            Card ObjCard = objCards.GetCards(PassKey).FirstOrDefault();
            if (ObjCard== null)
            {
                //Response.StatusCode = (int)HttpStatusCode.NotFound;
             
                return  Content(LocalizeString("CardNotFound"));
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                Session["GiftCardValue"] = PassKey;
                return Content(LocalizeString("CardFound"));
            }
        }
    }
}