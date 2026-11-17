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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    /// <summary>Persists localized, store-scoped <see cref="HtmlTemplate"/> records.</summary>
    public class HtmlTemplateRepository :
        HccLocalizationRepoBase<hcc_HtmlTemplates, hcc_HtmlTemplateTranslation, HtmlTemplate, long>
    {
        /// <summary>Initializes the repository for the specified commerce context.</summary>
        public HtmlTemplateRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>Gets the expression that identifies a template record.</summary>
        protected override Expression<Func<hcc_HtmlTemplates, long>> ItemKeyExp
        {
            get { return ht => ht.Id; }
        }

        /// <summary>Gets the expression that identifies a template translation by template identifier.</summary>
        protected override Expression<Func<hcc_HtmlTemplateTranslation, long>> ItemTranslationKeyExp
        {
            get { return htt => htt.HtmlTemplateId; }
        }

        /// <summary>Copies persisted template fields into the model.</summary>
        protected override void CopyItemToModel(hcc_HtmlTemplates data, HtmlTemplate model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.DisplayName = data.DisplayName;
            model.From = data.FromEmail;
            model.TemplateType = (HtmlTemplateType) data.TemplateType;
        }

        /// <summary>Copies persisted translation fields into the model.</summary>
        protected override void CopyTransToModel(hcc_HtmlTemplateTranslation data, HtmlTemplate model)
        {
            model.ContentCulture = data.Culture;
            model.Subject = data.Subject;
            model.Body = data.Body;
            model.RepeatingSection = data.RepeatingSection;
        }

        /// <summary>Copies nonlocalized model fields into the persistence item.</summary>
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

        /// <summary>Copies localized model fields into the persistence translation item.</summary>
        protected override void CopyModelToTrans(JoinedItem<hcc_HtmlTemplates, hcc_HtmlTemplateTranslation> data,
            HtmlTemplate model)
        {
            data.ItemTranslation.HtmlTemplateId = data.Item.Id;

            data.ItemTranslation.Subject = model.Subject;
            data.ItemTranslation.Body = model.Body;
            data.ItemTranslation.RepeatingSection = model.RepeatingSection;
        }

        /// <summary>Finds a template by identifier in the current store.</summary>
        public HtmlTemplate Find(long Id)
        {
            return FindFirstPoco(ht => ht.Item.Id == Id && ht.Item.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds a current-store template by display name.</summary>
        public HtmlTemplate Find(string displayName)
        {
            return FindFirstPoco(ht => ht.Item.DisplayName == displayName && ht.Item.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds a template by identifier without applying current-store filtering.</summary>
        public HtmlTemplate FindForAllStores(long Id)
        {
            return FindFirstPoco(ht => ht.Item.Id == Id);
        }

        /// <summary>Finds a template by store and template type.</summary>
        public HtmlTemplate FindByStoreAndType(long storeId, HtmlTemplateType templateType)
        {
            var typeId = (int) templateType;
            return FindFirstPoco(ht => ht.Item.StoreId == storeId && ht.Item.TemplateType == typeId);
        }

        /// <summary>Creates a template, stamping the current UTC time and current store.</summary>
        public override bool Create(HtmlTemplate item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Updates a current-store template and refreshes its UTC timestamp.</summary>
        /// <returns><see langword="false"/> when the template belongs to another store or persistence fails.</returns>
        public bool Update(HtmlTemplate c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdatedUtc = DateTime.UtcNow;
            return Update(c, ht => ht.Id == c.Id);
        }

        /// <summary>Deletes a current-store template by identifier.</summary>
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

        /// <summary>Gets all templates for the current store, ordered by display name.</summary>
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
