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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    public class ContentBlockRepository :
        HccLocalizationRepoBase<hcc_ContentBlock, hcc_ContentBlockTranslation, ContentBlock, Guid>
    {
        public ContentBlockRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override Expression<Func<hcc_ContentBlock, Guid>> ItemKeyExp
        {
            get { return cb => cb.bvin; }
        }

        protected override Expression<Func<hcc_ContentBlockTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pot => pot.ContentBlockId; }
        }

        protected override Func<JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation>, bool> MatchItems(
            ContentBlock item)
        {
            var itemGuid = DataTypeHelper.BvinToGuid(item.Bvin);
            return t => t.Item.bvin == itemGuid;
        }

        protected override Func<JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation>, bool> NotMatchItems(
            List<ContentBlock> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return cb => !itemGuids.Contains(cb.Item.bvin);
        }

        protected override void CopyItemToModel(hcc_ContentBlock data, ContentBlock model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.ColumnId = DataTypeHelper.GuidToBvin(data.ColumnID);
            model.ControlName = data.ControlName;
            model.SortOrder = data.SortOrder;

            model.BaseSettings = Json.ObjectFromJson<ContentBlockSettings>(data.SerializedSettings);
            if (model.BaseSettings == null)
                model.BaseSettings = new ContentBlockSettings();

            model.Lists = Json.ObjectFromJson<ContentBlockSettingList>(data.SerializedLists);
            if (model.Lists == null)
                model.Lists = new ContentBlockSettingList();
        }

        protected override void CopyTransToModel(hcc_ContentBlockTranslation data, ContentBlock model)
        {
            model.TextSettings = Json.ObjectFromJson<ContentBlockSettings>(data.TextSettings);
            if (model.TextSettings == null)
                model.TextSettings = new ContentBlockSettings();
        }

        protected override void CopyModelToItem(JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation> data,
            ContentBlock model)
        {
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.StoreId = model.StoreId;
            data.Item.LastUpdated = model.LastUpdated;
            data.Item.ColumnID = DataTypeHelper.BvinToGuid(model.ColumnId);
            data.Item.ControlName = model.ControlName;
            data.Item.SortOrder = model.SortOrder;
            data.Item.SerializedSettings = Json.ObjectToJson(model.BaseSettings);
            data.Item.SerializedLists = Json.ObjectToJson(model.Lists);
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation> data,
            ContentBlock model)
        {
            data.ItemTranslation.ContentBlockId = data.Item.bvin;

            data.ItemTranslation.TextSettings = Json.ObjectToJson(model.TextSettings);
        }

        public ContentBlock Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(c => c.Item.bvin == guid);
        }

        public List<ContentBlock> FindManyForContentColumns(List<string> bvins)
        {
            var guids = bvins.Select(b => DataTypeHelper.BvinToGuid(b)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(cb => guids.Contains(cb.Item.ColumnID))
                    .OrderBy(cb => cb.Item.SortOrder);
            });
        }

        public override bool Create(ContentBlock item, bool useModelCulture)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.LastUpdated = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(ContentBlock item)
        {
            item.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return Update(item, cb => cb.bvin == guid);
        }

        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(cb => cb.bvin == guid);
        }

        public List<ContentBlock> FindForColumn(string columnId, long storeId)
        {
            var columnGuid = DataTypeHelper.BvinToGuid(columnId);

            return FindListPoco(q =>
            {
                return q.Where(cb => cb.Item.ColumnID == columnGuid)
                    .Where(cb => cb.Item.StoreId == storeId)
                    .OrderBy(cb => cb.Item.SortOrder);
            });
        }

        public void DeleteForColumn(Guid columnId, long storeId)
        {
            Delete(cb => cb.ColumnID == columnId && cb.StoreId == storeId);
        }

        public bool Resort(string columnId, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForBlock(columnId, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        public bool UpdateSortOrderForBlock(string columnId, string blockId, int newSortOrder)
        {
            var item = Find(blockId);
            if (item == null) return false;
            if (item.ColumnId != columnId) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        public void MergeList(string columnId, long storeId, List<ContentBlock> subitems)
        {
            // Set base Key Field
            foreach (var item in subitems)
            {
                item.StoreId = storeId;
                item.ColumnId = columnId;
                item.LastUpdated = DateTime.UtcNow;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var columnGuid = DataTypeHelper.BvinToGuid(columnId);
            MergeList(subitems, cb => cb.ColumnID == columnGuid && cb.StoreId == storeId);
        }
    }
}