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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    public class HtmlTemplateRepository :
        HccLocalizationRepoBase<hcc_HtmlTemplates, hcc_HtmlTemplateTranslation, HtmlTemplate, long>
    {
        public HtmlTemplateRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override Expression<Func<hcc_HtmlTemplates, long>> ItemKeyExp
        {
            get { return ht => ht.Id; }
        }

        protected override Expression<Func<hcc_HtmlTemplateTranslation, long>> ItemTranslationKeyExp
        {
            get { return htt => htt.HtmlTemplateId; }
        }

        protected override void CopyItemToModel(hcc_HtmlTemplates data, HtmlTemplate model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.DisplayName = data.DisplayName;
            model.From = data.FromEmail;
            model.TemplateType = (HtmlTemplateType) data.TemplateType;
        }

        protected override void CopyTransToModel(hcc_HtmlTemplateTranslation data, HtmlTemplate model)
        {
            model.ContentCulture = data.Culture;
            model.Subject = data.Subject;
            model.Body = data.Body;
            model.RepeatingSection = data.RepeatingSection;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_HtmlTemplates, hcc_HtmlTemplateTranslation> data,
            HtmlTemplate model)
        {
            data.Item.Id = model.Id;
            data.Item.StoreId = model.StoreId;
            data.Item.LastUpdatedUtc = model.LastUpdatedUtc;
            data.Item.DisplayName = model.DisplayName;
            data.Item.FromEmail = model.From;
            data.Item.TemplateType = (int) model.TemplateType;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_HtmlTemplates, hcc_HtmlTemplateTranslation> data,
            HtmlTemplate model)
        {
            data.ItemTranslation.HtmlTemplateId = data.Item.Id;

            data.ItemTranslation.Subject = model.Subject;
            data.ItemTranslation.Body = model.Body;
            data.ItemTranslation.RepeatingSection = model.RepeatingSection;
        }

        public HtmlTemplate Find(long Id)
        {
            return FindFirstPoco(ht => ht.Item.Id == Id && ht.Item.StoreId == Context.CurrentStore.Id);
        }

        public HtmlTemplate Find(string displayName)
        {
            return FindFirstPoco(ht => ht.Item.DisplayName == displayName && ht.Item.StoreId == Context.CurrentStore.Id);
        }

        public HtmlTemplate FindForAllStores(long Id)
        {
            return FindFirstPoco(ht => ht.Item.Id == Id);
        }

        public HtmlTemplate FindByStoreAndType(long storeId, HtmlTemplateType templateType)
        {
            var typeId = (int) templateType;
            return FindFirstPoco(ht => ht.Item.StoreId == storeId && ht.Item.TemplateType == typeId);
        }

        public override bool Create(HtmlTemplate item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(HtmlTemplate c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdatedUtc = DateTime.UtcNow;
            return Update(c, ht => ht.Id == c.Id);
        }

        public bool Delete(long Id)
        {
            var t = Find(Id);
            if (t == null) return false;
            if (t.StoreId != Context.CurrentStore.Id) return false;
            return Delete(ht => ht.Id == Id);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            var t = FindForAllStores(id);
            if (t == null) return false;
            if (t.StoreId != storeId) return false;
            return Delete(ht => ht.Id == id);
        }

        public List<HtmlTemplate> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.Item.DisplayName);
            });
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(ht => ht.StoreId == storeId);
        }
    }
}