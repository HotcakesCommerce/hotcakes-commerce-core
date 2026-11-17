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
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Provides store-scoped persistence for <see cref="ContentColumn"/> records and their blocks.
    /// </summary>
    public class ContentColumnRepository : HccSimpleRepoBase<hcc_ContentColumn, ContentColumn>
    {
        /// <summary>Initializes a repository using the supplied commerce request context.</summary>
        /// <param name="context">The request context used for store-scoped operations.</param>
        public ContentColumnRepository(HccRequestContext context)
            : base(context)
        {
            BlockRepository = new ContentBlockRepository(context);
        }

        private ContentBlockRepository BlockRepository { get; set; }

        /// <summary>Copies a persisted column row into a column model.</summary>
        protected override void CopyDataToModel(hcc_ContentColumn data, ContentColumn model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.DisplayName = data.DisplayName;
            model.SystemColumn = data.SystemColumn == 1;
        }

        /// <summary>Copies a column model into its persisted row.</summary>
        protected override void CopyModelToData(hcc_ContentColumn data, ContentColumn model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdated;
            data.DisplayName = model.DisplayName;
            data.SystemColumn = model.SystemColumn ? 1 : 0;
        }

        /// <summary>Loads blocks for the supplied columns and attaches them to their models.</summary>
        protected override void GetSubItems(List<ContentColumn> models)
        {
            var contentColumnBvins = models.Select(cc => cc.Bvin).ToList();
            var contentBlocks = BlockRepository.FindManyForContentColumns(contentColumnBvins);

            foreach (var model in models)
            {
                model.Blocks = contentBlocks.Where(cb => cb.ColumnId == model.Bvin).ToList();
            }
        }

        /// <summary>Persists the blocks associated with a column.</summary>
        protected override void MergeSubItems(ContentColumn model)
        {
            BlockRepository.MergeList(model.Bvin, model.StoreId, model.Blocks);
        }

        /// <summary>Finds a column only when it belongs to the current store.</summary>
        /// <param name="bvin">The column business identifier.</param>
        /// <returns>The current-store column, or <see langword="null"/>.</returns>
        public ContentColumn Find(string bvin)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>Finds a column without applying the current-store filter.</summary>
        /// <param name="bvin">The column business identifier.</param>
        /// <returns>The matching column, or <see langword="null"/>.</returns>
        public ContentColumn FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(c => c.bvin == guid);
        }

        /// <summary>Creates a column in the current store and initializes its identifier and UTC timestamp.</summary>
        /// <param name="item">The column to persist.</param>
        /// <returns><see langword="true"/> when persistence succeeds.</returns>
        public override bool Create(ContentColumn item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        /// <summary>Updates a current-store column and its blocks.</summary>
        /// <param name="column">The column to persist.</param>
        /// <returns><see langword="false"/> when the column belongs to another store; otherwise the persistence result.</returns>
        public bool Update(ContentColumn column)
        {
            if (column.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            column.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(column.Bvin);
            return Update(column, c => c.bvin == guid);
        }

        /// <summary>Deletes a non-system column from the current store.</summary>
        /// <param name="bvin">The column business identifier.</param>
        /// <returns><see langword="true"/> when deletion succeeds.</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var item = Find(bvin);
            if (item == null) return false;
            if (item.SystemColumn) return false;
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(c => c.bvin == guid && c.StoreId == storeId);
        }

        /// <summary>Finds all columns in the current store, ordered by display name.</summary>
        /// <returns>All current-store columns.</returns>
        public List<ContentColumn> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        /// <summary>Finds a page of current-store columns ordered by display name.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of columns in the page.</param>
        /// <param name="totalCount">Receives the total matching column count.</param>
        /// <returns>The requested page of columns.</returns>
        public List<ContentColumn> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;

            using (var strategy = CreateReadStrategy())
            {
                var query = strategy
                    .GetQuery(c => c.StoreId == storeId)
                    .AsNoTracking()
                    .OrderBy(c => c.DisplayName);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>Finds a current-store column by exact display name.</summary>
        /// <param name="displayName">The display name to match.</param>
        /// <returns>The matching column, or <see langword="null"/>.</returns>
        public ContentColumn FindByDisplayName(string displayName)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(c => c.StoreId == storeId && c.DisplayName == displayName);
        }

        /// <summary>Clones the blocks of a current-store column into a new non-system column.</summary>
        /// <param name="sourceBvin">The source column business identifier.</param>
        /// <param name="newColumnName">The display name for the new column.</param>
        /// <returns>The newly created column.</returns>
        public ContentColumn Clone(string sourceBvin, string newColumnName)
        {
            var c = new ContentColumn();
            c.DisplayName = newColumnName;
            c.SystemColumn = false;

            var source = Find(sourceBvin);

            if (source != null)
            {
                foreach (var b in source.Blocks)
                {
                    c.Blocks.Add(b.Clone());
                }
            }

            Create(c);
            return c;
        }

        /// <summary>Deletes a block by business identifier.</summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns>The block repository deletion result.</returns>
        public bool DeleteBlock(string blockId)
        {
            return BlockRepository.Delete(blockId);
        }

        /// <summary>Resorts blocks in a column using the supplied identifier order.</summary>
        /// <param name="columnId">The column identifier.</param>
        /// <param name="sortedItemIds">Block identifiers in desired order.</param>
        /// <returns>The block repository result.</returns>
        public bool ResortBlocksItems(string columnId, List<string> sortedItemIds)
        {
            return BlockRepository.Resort(columnId, sortedItemIds);
        }

        /// <summary>Finds a block by business identifier.</summary>
        /// <param name="blockId">The block identifier.</param>
        /// <returns>The matching block, or <see langword="null"/>.</returns>
        public ContentBlock FindBlock(string blockId)
        {
            return BlockRepository.Find(blockId);
        }

        /// <summary>Updates a block through the block repository.</summary>
        /// <param name="item">The block to update.</param>
        /// <returns>The block repository update result.</returns>
        public bool UpdateBlock(ContentBlock item)
        {
            return BlockRepository.Update(item);
        }

        /// <summary>Moves a block toward the beginning of its column by exchanging sort positions.</summary>
        /// <param name="blockBvin">The block identifier.</param>
        /// <param name="columnId">The column identifier.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns><see langword="true"/> when both sort updates succeed.</returns>
        public bool MoveBlockUp(string blockBvin, string columnId, long storeId)
        {
            var bRet = false;


            try
            {
                var blocks = BlockRepository.FindForColumn(columnId, storeId);

                if (blocks.Count > 0)
                {
                    // we have more than 1 item so there is a chance to switch
                    var CurrentSort = -1;
                    var LowestSort = 0;
                    var NewSort = 1;
                    var SwitchID = string.Empty;

                    // Find lowest sort order for type
                    LowestSort = blocks[0].SortOrder;

                    int iCount;
                    for (iCount = 0; iCount <= blocks.Count - 1; iCount++)
                    {
                        if (blocks[iCount].Bvin == blockBvin)
                        {
                            CurrentSort = blocks[iCount].SortOrder;
                        }

                        // no match yet
                        if (CurrentSort == -1)
                        {
                            NewSort = blocks[iCount].SortOrder;
                            SwitchID = blocks[iCount].Bvin;
                        }
                    }

                    // If we can move up, find the row to switch with
                    if (CurrentSort > LowestSort)
                    {
                        if (BlockRepository.UpdateSortOrderForBlock(columnId, blockBvin, NewSort))
                        {
                            if (BlockRepository.UpdateSortOrderForBlock(columnId, SwitchID, CurrentSort))
                            {
                                bRet = true;
                            }
                            else
                            {
                                bRet = false;
                                // Return current
                                BlockRepository.UpdateSortOrderForBlock(columnId, blockBvin, CurrentSort);
                            }
                        }
                        else
                        {
                            bRet = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
                bRet = false;
            }

            return bRet;
        }

        /// <summary>Moves a block toward the end of its column by exchanging sort positions.</summary>
        /// <param name="blockBvin">The block identifier.</param>
        /// <param name="columnId">The column identifier.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <returns><see langword="true"/> when both sort updates succeed.</returns>
        public bool MoveBlockDown(string blockBvin, string columnId, long storeId)
        {
            var bRet = false;

            try
            {
                List<ContentBlock> blocks;
                blocks = BlockRepository.FindForColumn(columnId, storeId);

                if (blocks.Count > 0)
                {
                    // we have more than 1 item so there is a chance to switch
                    var CurrentSort = -1;
                    var MaxSort = 0;
                    var NewSort = -1;
                    var SwitchId = string.Empty;

                    // Find highest sort order for type
                    MaxSort = blocks[blocks.Count - 1].SortOrder;

                    int iCount;
                    for (iCount = 0; iCount <= blocks.Count - 1; iCount++)
                    {
                        // got the match so grab this one
                        if (NewSort == -2)
                        {
                            NewSort = blocks[iCount].SortOrder;
                            SwitchId = blocks[iCount].Bvin;
                        }

                        if (blocks[iCount].Bvin == blockBvin)
                        {
                            CurrentSort = blocks[iCount].SortOrder;
                            // Trigger New Sort
                            NewSort = -2;
                        }
                    }

                    // If we can move up, find the row to switch with
                    if (CurrentSort < MaxSort)
                    {
                        if (BlockRepository.UpdateSortOrderForBlock(columnId, blockBvin, NewSort))
                        {
                            if (BlockRepository.UpdateSortOrderForBlock(columnId, SwitchId, CurrentSort))
                            {
                                bRet = true;
                            }
                            else
                            {
                                bRet = false;
                                // Return current
                                BlockRepository.UpdateSortOrderForBlock(columnId, blockBvin, CurrentSort);
                            }
                        }
                        else
                        {
                            bRet = false;
                        }
                    }
                }
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        /// <summary>Copies a block into another current-store column.</summary>
        /// <param name="blockBvin">The source block identifier.</param>
        /// <param name="columnId">The destination column identifier.</param>
        /// <returns><see langword="true"/> when the destination column is updated.</returns>
        public bool CopyBlockToColumn(string blockBvin, string columnId)
        {
            var b = BlockRepository.Find(blockBvin);
            if (b != null)
            {
                var clone = b.Clone();
                clone.ColumnId = columnId;
                clone.Bvin = string.Empty;
                var newColumn = Find(columnId);
                if (newColumn != null)
                {
                    newColumn.Blocks.Add(clone);
                    return Update(newColumn);
                }
                return false;
            }
            return false;
        }

        /// <summary>Moves a block by changing its column identifier.</summary>
        /// <param name="blockBvin">The block identifier.</param>
        /// <param name="columnId">The destination column identifier.</param>
        /// <returns>The block repository update result.</returns>
        public bool MoveBlockToColumn(string blockBvin, string columnId)
        {
            var b = BlockRepository.Find(blockBvin);
            if (b != null)
            {
                b.ColumnId = columnId;
                return BlockRepository.Update(b);
            }
            return false;
        }

        internal void DestroyForStore(long storeId)
        {
            Delete(c => c.StoreId == storeId);
        }

        /// <summary>Loads column XML from a virtual template file and creates or updates its columns.</summary>
        /// <param name="virtualFilePath">The virtual path of the template file.</param>
        /// <remarks>Existing identifiers are updated; columns without identifiers are created in the current store.</remarks>
        public void CreateFromTemplateFile(string virtualFilePath)
        {
            var filePath = HostingEnvironment.MapPath(virtualFilePath);
            if (File.Exists(filePath))
            {
                var xdoc = new XmlDocument();
                xdoc.Load(filePath);

                XmlNodeList columnNodes;
                columnNodes = xdoc.SelectNodes("/ColumnData/ContentColumn");

                if (columnNodes != null)
                {
                    for (var i = 0; i < columnNodes.Count; i++)
                    {
                        var column = new ContentColumn();
                        column.FromXmlString(columnNodes[i].OuterXml);

                        // If column does not exists then create it
                        if (string.IsNullOrEmpty(column.Bvin))
                        {
                            Create(column);
                        }
                        else
                        {
                            Update(column);
                        }
                    }
                }
            }
        }
    }
}
