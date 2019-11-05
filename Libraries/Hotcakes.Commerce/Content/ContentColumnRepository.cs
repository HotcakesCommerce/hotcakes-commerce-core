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
    public class ContentColumnRepository : HccSimpleRepoBase<hcc_ContentColumn, ContentColumn>
    {
        public ContentColumnRepository(HccRequestContext context)
            : base(context)
        {
            BlockRepository = new ContentBlockRepository(context);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ContentColumnRepository(HccRequestContext context, bool isForMemoryOnly)
            : this(context)
        {
        }

        #endregion

        private ContentBlockRepository BlockRepository { get; set; }

        protected override void CopyDataToModel(hcc_ContentColumn data, ContentColumn model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.DisplayName = data.DisplayName;
            model.SystemColumn = data.SystemColumn == 1;
        }

        protected override void CopyModelToData(hcc_ContentColumn data, ContentColumn model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdated;
            data.DisplayName = model.DisplayName;
            data.SystemColumn = model.SystemColumn ? 1 : 0;
        }

        protected override void GetSubItems(List<ContentColumn> models)
        {
            var contentColumnBvins = models.Select(cc => cc.Bvin).ToList();
            var contentBlocks = BlockRepository.FindManyForContentColumns(contentColumnBvins);

            foreach (var model in models)
            {
                model.Blocks = contentBlocks.Where(cb => cb.ColumnId == model.Bvin).ToList();
            }
        }

        protected override void MergeSubItems(ContentColumn model)
        {
            BlockRepository.MergeList(model.Bvin, model.StoreId, model.Blocks);
        }

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

        public ContentColumn FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(c => c.bvin == guid);
        }

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

        public List<ContentColumn> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        public List<ContentColumn> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;

            using (var strategy = CreateStrategy())
            {
                var query = strategy
                    .GetQuery(c => c.StoreId == storeId)
                    .OrderBy(c => c.DisplayName);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        public ContentColumn FindByDisplayName(string displayName)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(c => c.StoreId == storeId && c.DisplayName == displayName);
        }

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

        public bool DeleteBlock(string blockId)
        {
            return BlockRepository.Delete(blockId);
        }

        public bool ResortBlocksItems(string columnId, List<string> sortedItemIds)
        {
            return BlockRepository.Resort(columnId, sortedItemIds);
        }

        public ContentBlock FindBlock(string blockId)
        {
            return BlockRepository.Find(blockId);
        }

        public bool UpdateBlock(ContentBlock item)
        {
            return BlockRepository.Update(item);
        }

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