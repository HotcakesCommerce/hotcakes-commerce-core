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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Analytics
{
    public class AnalyticsEventsRepository : HccSimpleRepoBase<hcc_AnalyticsEvent, AnalyticsEvent>
    {
        public AnalyticsEventsRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override void CopyDataToModel(hcc_AnalyticsEvent data, AnalyticsEvent model)
        {
            model.AnalyticsEventId = data.AnalyticsEventId;
            model.UserId = data.UserId;
            model.SessionGuid = data.SessionGuid;
            model.ShoppingSessionGuid = data.ShoppingSessionGuid;
            model.StoreId = data.StoreId;
            model.Action = (ActionTypes) Enum.Parse(typeof (ActionTypes), data.Action);
            model.ObjectId = data.ObjectId;
            model.AdditionalData = data.AdditionalData;
            model.DateTime = data.DateTime;
        }

        protected override void CopyModelToData(hcc_AnalyticsEvent data, AnalyticsEvent model)
        {
            data.AnalyticsEventId = model.AnalyticsEventId;
            data.UserId = model.UserId;
            data.SessionGuid = model.SessionGuid;
            data.ShoppingSessionGuid = model.ShoppingSessionGuid;
            data.StoreId = model.StoreId;
            data.Action = model.Action.ToString();
            data.ObjectId = model.ObjectId;
            data.AdditionalData = model.AdditionalData;
            data.DateTime = model.DateTime;
        }

        public bool Delete(long analyticsEventId)
        {
            return Delete(a => a.AnalyticsEventId == analyticsEventId);
        }

        public bool DeleteByUserId(string userId)
        {
            return Delete(a => a.UserId == userId);
        }

        public bool DeleteAllForCurrentStore(long storID)
        {
            return Delete(a => a.StoreId == storID);
        }

        public List<AnalyticsEvent> FindBySessionGuid(Guid sessionGuid)
        {
            return FindListPoco(q => { return q.Where(a => a.SessionGuid == sessionGuid); });
        }

        public List<AnalyticsEvent> FindByUserId(string userId)
        {
            return FindListPoco(q => { return q.Where(a => a.UserId == userId); });
        }
    }
}