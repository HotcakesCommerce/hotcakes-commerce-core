#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Provides persistence and reporting queries for <see cref="HccNewsMessage"/> records.</summary>
    public class HccNewsMessageManager : HccSimpleRepoBase<hcc_News, HccNewsMessage>
    {
        /// <summary>Initializes a manager for the supplied commerce request context.</summary>
        /// <param name="context">The request context that scopes repository operations.</param>
        public HccNewsMessageManager(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>Copies a data record into its reporting model.</summary>
        /// <param name="data">The persistence record.</param>
        /// <param name="model">The destination model.</param>
        protected override void CopyDataToModel(hcc_News data, HccNewsMessage model)
        {
            model.Id = data.Id;
            model.TimeStampUtc = data.TimeStampUtc;
            model.Message = data.Message;
        }

        /// <summary>Copies a reporting model into its persistence record.</summary>
        /// <param name="data">The persistence record.</param>
        /// <param name="model">The source model.</param>
        protected override void CopyModelToData(hcc_News data, HccNewsMessage model)
        {
            data.Id = model.Id;
            data.TimeStampUtc = model.TimeStampUtc;
            data.Message = model.Message;
        }

        /// <summary>Returns the newest news messages, ordered by descending UTC timestamp.</summary>
        /// <param name="maxItems">The maximum number of messages to return.</param>
        /// <returns>A list containing at most <paramref name="maxItems"/> messages.</returns>
        public List<HccNewsMessage> GetLatestNews(int maxItems)
        {
            return FindListPoco(q => q.OrderByDescending(y => y.TimeStampUtc).Take(maxItems));
        }
    }
}
