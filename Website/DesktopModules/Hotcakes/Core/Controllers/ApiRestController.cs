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
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.Modules.Core.Api.Rest;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class ApiRestController : Controller
    {
        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }

        // Authenticate API Key Methods
        public bool AuthenticateKey(string apikey, IApiResponse response)
        {
            if (string.IsNullOrWhiteSpace(apikey))
            {
                response.Errors.Add(new ApiError("KEY", "Api Key is missing."));
                return false;
            }

            var key = HccApp.AccountServices.ApiKeys.FindByKey(apikey);
            if (key == null || key.Key != apikey || key.StoreId < 0)
            {
                response.Errors.Add(new ApiError("KEY", "Api Key is invalid"));
                return false;
            }

            var storeID = key.StoreId;
            HccApp.CurrentStore = HccApp.AccountServices.Stores.FindById(storeID);
            if (HccApp.CurrentStore == null || HccApp.CurrentStore.Id < 0)
            {
                response.Errors.Add(new ApiError("STORENOTFOUND", "No store was found at this URL."));
                return false;
            }

            if (HccApp.CurrentStore.Settings.ForceAdminSSL && !Request.IsSecureConnection)
            {
                response.Errors.Add(new ApiError("SSLCONNECTION", "Store API require SSL connection."));
                return false;
            }

            return true;
        }

        // Actual Responder
        public ActionResult Index(string version, string modelname, string parameters)
        {
            var data = string.Empty;

            // Key Api Key First
            var key = Request.QueryString["key"];
            var FailedKeyResponse = new ApiResponse<object>();
            
            if (!AuthenticateKey(key, FailedKeyResponse))
            {
                data = Web.Json.ObjectToJson(FailedKeyResponse);
            }
            else
            {
                // Create Handler

                if (HccApp.CurrentRequestContext.CurrentAccount == null)
                {
                    HccApp.CurrentRequestContext.CurrentAccount = new CustomerAccount {Bvin = "-1"};
                }

                var handler = RestHandlerFactory.Instantiate(version, modelname, HccApp);

                // Read Posted JSON
                var postedString = string.Empty;
                var inputStream = Request.InputStream;
                if (inputStream != null)
                {
                    Request.InputStream.Position = 0;
                    var rdr = new StreamReader(inputStream);
                    postedString = rdr.ReadToEnd();
                }

                switch (Request.HttpMethod.ToUpperInvariant())
                {
                    case "GET":
                        data = handler.GetAction(parameters, Request.QueryString);
                        break;
                    case "POST":
                        data = handler.PostAction(parameters, Request.QueryString, postedString);
                        break;
                    case "PUT":
                        data = handler.PutAction(parameters, Request.QueryString, postedString);
                        break;
                    case "DELETE":
                        data = handler.DeleteAction(parameters, Request.QueryString, postedString);
                        break;
                }
            }

            // Return Result (or text formatted result)
            if (Request.QueryString["apiformat"] == "text")
            {
                return new RawResult(data, "text/html");
            }
            return new PreJsonResult(data);
        }
    }
}
