#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Web.Mvc;
using System.Web.Security;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Controllers;
using Hotcakes.Modules.Core.Controllers.Shared;

namespace Hotcakes.Modules.Core.Areas.Account.Controllers
{
    [Serializable]
    public class AuthenticationController : BaseStoreController
    {
        [HccHttpPost]
        public ActionResult AjaxSignIn()
        {
            var username = Request.Form["username"] ?? string.Empty;
            var password = Request.Form["password"] ?? string.Empty;

            var validated = new ValidateModelResponse();
            var errorMessage = string.Empty;
            string userId = null;

            if (HccApp.MembershipServices.LoginUser(username, password, out errorMessage, out userId))
            {
                if (CurrentCart != null)
                {
                    var custAcc = HccApp.MembershipServices.Customers.Find(userId);
                    CurrentCart.UserEmail = custAcc != null ? custAcc.Email.Trim() : null;
                    CurrentCart.UserID = userId;
                    HccApp.CalculateOrderAndSave(CurrentCart);
                }

                validated.Success = true;
            }
            else
            {
                validated.ResultMessages.Add(errorMessage);
                validated.Success = false;
            }

            return new PreJsonResult(Web.Json.ObjectToJson(validated));
        }

        [HccHttpPost]
        public ActionResult SetFirstPassword()
        {
            var email = Request.Form["email"] ?? string.Empty;
            var password = Request.Form["password"] ?? string.Empty;
            var orderbvin = Request.Form["orderbvin"] ?? string.Empty;

            var resp = new SimpleResponse {Success = true};

            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderbvin);
            if (order == null)
            {
                resp.Success = false;
                resp.Messages += "Order id was invalid for password reset. ";
            }
            else
            {
                if (order.CustomProperties.Where(y => (y.DeveloperId == "hcc")
                                                      && (y.Key == "allowpasswordreset")
                                                      && (y.Value == "1")).Count() < 1)
                {
                    resp.Success = false;
                    resp.Messages +=
                        "This order does not allow password reset anymore. Please use the 'Forgot Password' link when signing in. ";
                }
            }

            if (!MembershipUtils.CheckPasswordComplexity(Membership.Provider, password.Trim()))
            {
                resp.Success = false;
                resp.Messages += "Password must be at least " + WebAppSettings.PasswordMinimumLength +
                                 " characters long. ";
            }

            if (resp.Success)
            {
                try
                {
                    var userId = Convert.ToInt32(order.UserID);
                    DnnUserController.Instance.ResetPassword(userId, password, "");

                    // Turn off reset key so that this can only happen once.
                    var prop = order.CustomProperties.FirstOrDefault(y => (y.DeveloperId == "hcc")
                                                                 && (y.Key == "allowpasswordreset")
                                                                 && (y.Value == "1"));
                    if (prop != null)
                    {
                        prop.Value = "0";
                    }
                    HccApp.OrderServices.Orders.Update(order);
                }
                catch (Exception ex)
                {
                    resp.Success = false;
                    resp.Messages = ex.Message;
                }
            }

            return new PreJsonResult(Web.Json.ObjectToJson(resp));
        }

        #region Internal declaration

        private class ValidateModelResponse
        {
            public ValidateModelResponse()
            {
                Success = false;
                ResultMessages = new List<string>();
            }

            public bool Success { get; set; }
            public List<string> ResultMessages { get; set; }
        }

        private class SimpleResponse
        {
            public SimpleResponse()
            {
                Success = false;
                Messages = string.Empty;
            }

            public bool Success { get; set; }
            public string Messages { get; set; }
        }

        #endregion
    }
}