#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Web.Http;
using System.Web.Http.Controllers;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hotcakes.Modules.Core.Api.Mobile
{
    [Serializable]
    [VerifyLicenseActionFilter]
    public abstract class HccApiController : ApiController
    {
        private static readonly DefaultContractResolver Resolver = new DefaultContractResolver
        {
            IgnoreSerializableAttribute = true
        };

        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);

            var settings = Configuration.Formatters.JsonFormatter.SerializerSettings;
            if (settings.NullValueHandling != NullValueHandling.Ignore)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
            }

            settings.ContractResolver = Resolver;
        }

        protected static DateRange GetDateRange(string rangeType)
        {
            var rangeData = new DateRange();

            switch (rangeType)
            {
                case "all":
                    rangeData.RangeType = DateRangeType.AllDates;
                    break;
                case "day":
                    rangeData.RangeType = DateRangeType.Today;
                    break;
                case "month":
                    rangeData.RangeType = DateRangeType.ThisMonth;
                    break;
                case "year":
                    rangeData.RangeType = DateRangeType.YearToDate;
                    break;
                case "week":
                default:
                    rangeData.RangeType = DateRangeType.ThisWeek;
                    break;
            }

            return rangeData;
        }
    }
}