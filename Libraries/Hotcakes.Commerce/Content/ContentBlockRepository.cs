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
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Provides store-scoped persistence for <see cref="ContentBlock"/> records and their localized settings.
    /// </summary>
    public class ContentBlockRepository :
        HccLocalizationRepoBase<hcc_ContentBlock, hcc_ContentBlockTranslation, ContentBlock, Guid>
    {
        /// <summary>
        /// Initializes a repository using the supplied commerce request context.
        /// </summary>
        /// <param name="context">The request context used for store and localization operations.</param>
        public ContentBlockRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>Gets the database key expression for content blocks.</summary>
        protected override Expression<Func<hcc_ContentBlock, Guid>> ItemKeyExp
        {
            get { return cb => cb.bvin; }
        }

        /// <summary>Gets the database key expression for localized content-block rows.</summary>
        protected override Expression<Func<hcc_ContentBlockTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pot => pot.ContentBlockId; }
        }

        /// <summary>Matches a joined data row to a content block by its business identifier.</summary>
        protected override Func<JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation>, bool> MatchItems(
            ContentBlock item)
        {
            var itemGuid = DataTypeHelper.BvinToGuid(item.Bvin);
            return t => t.Item.bvin == itemGuid;
        }

        /// <summary>Builds a predicate for joined rows not represented by the supplied blocks.</summary>
        protected override Func<JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation>, bool> NotMatchItems(
            List<ContentBlock> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return cb => !itemGuids.Contains(cb.Item.bvin);
        }

        /// <summary>Copies persisted base data and JSON settings into a block model.</summary>
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

        /// <summary>Copies localized text settings into a block model.</summary>
        protected override void CopyTransToModel(hcc_ContentBlockTranslation data, ContentBlock model)
        {
            model.TextSettings = Json.ObjectFromJson<ContentBlockSettings>(data.TextSettings);
            if (model.TextSettings == null)
                model.TextSettings = new ContentBlockSettings();
        }

        /// <summary>Copies a block model into its persisted base row, retaining JSON serialization.</summary>
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

        /// <summary>Copies a block model into its localized persisted row.</summary>
        protected override void CopyModelToTrans(JoinedItem<hcc_ContentBlock, hcc_ContentBlockTranslation> data,
            ContentBlock model)
        {
            data.ItemTranslation.ContentBlockId = data.Item.bvin;

            data.ItemTranslation.TextSettings = Json.ObjectToJson(model.TextSettings);
        }

        /// <summary>Finds a block by business identifier.</summary>
        /// <param name="bvin">The block identifier.</param>
        /// <returns>The matching block, or <see langword="null"/> when it is not found.</returns>
        public ContentBlock Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(c => c.Item.bvin == guid);
        }

        /// <summary>Finds blocks belonging to the specified columns in sort order.</summary>
        /// <param name="bvins">Column business identifiers.</param>
        /// <returns>The matching blocks ordered by sort order.</returns>
        public List<ContentBlock> FindManyForContentColumns(List<string> bvins)
        {
            var guids = bvins.Select(b => DataTypeHelper.BvinToGuid(b)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(cb => guids.Contains(cb.Item.ColumnID))
                    .OrderBy(cb => cb.Item.SortOrder);
            });
        }

        /// <summary>Creates a block in the current store and updates its UTC timestamp.</summary>
        /// <param name="item">The block to persist.</param>
        /// <param name="useModelCulture">Retained for the localization repository contract.</param>
        /// <returns><see langword="true"/> when persistence succeeds.</returns>
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

        /// <summary>Updates a block and refreshes its UTC timestamp.</summary>
        /// <param name="item">The block to persist.</param>
        /// <returns><see langword="true"/> when persistence succeeds.</returns>
        public bool Update(ContentBlock item)
        {
            item.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return Update(item, cb => cb.bvin == guid);
        }

        /// <summary>Deletes a block by business identifier.</summary>
        /// <param name="bvin">The block identifier.</param>
        /// <returns><see langword="true"/> when deletion succeeds.</returns>
        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(cb => cb.bvin == guid);
        }

        /// <summary>Finds blocks for a column and store in sort order.</summary>
        /// <param name="columnId">The column business identifier.</param>
        /// <param name="storeId">The owning store identifier.</param>
        /// <returns>The matching blocks ordered by sort order.</returns>
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

        /// <summary>Deletes all blocks for a database column identifier and store.</summary>
        /// <param name="columnId">The database column identifier.</param>
        /// <param name="storeId">The owning store identifier.</param>
        public void DeleteForColumn(Guid columnId, long storeId)
        {
            Delete(cb => cb.ColumnID == columnId && cb.StoreId == storeId);
        }

        /// <summary>Assigns sequential sort positions to blocks in the supplied order.</summary>
        /// <param name="columnId">The column business identifier.</param>
        /// <param name="sortedIds">Block identifiers in the desired order.</param>
        /// <returns><see langword="true"/> after processing.</returns>
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

        /// <summary>Updates a block's sort position when it belongs to the specified column.</summary>
        /// <param name="columnId">The column business identifier.</param>
        /// <param name="blockId">The block business identifier.</param>
        /// <param name="newSortOrder">The new sort position.</param>
        /// <returns><see langword="false"/> when the block is missing or belongs to another column.</returns>
        public bool UpdateSortOrderForBlock(string columnId, string blockId, int newSortOrder)
        {
            var item = Find(blockId);
            if (item == null) return false;
            if (item.ColumnId != columnId) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        /// <summary>Merges blocks for a column and store, assigning missing identifiers and ownership values.</summary>
        /// <param name="columnId">The column business identifier.</param>
        /// <param name="storeId">The owning store identifier.</param>
        /// <param name="subitems">The blocks to merge.</param>
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
