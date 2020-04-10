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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.TestStubs
{
    internal class ExampleSubRepository : ConvertingRepositoryBase<ExampleSubObjectDb, ExampleSubObject>
    {
        public ExampleSubRepository(IRepositoryStrategy<ExampleSubObjectDb> strategy, ILogger log)
        {
            repository = strategy;
            logger = log;
            repository.Logger = logger;
        }

        protected override void CopyModelToData(ExampleSubObjectDb data, ExampleSubObject model)
        {
            data.BaseIdDb = model.BaseId;
            data.Id = model.Id;
            data.NameDb = model.Name;
            data.SortOrderDb = model.SortOrder;
        }

        protected override void CopyDataToModel(ExampleSubObjectDb data, ExampleSubObject model)
        {
            model.BaseId = data.BaseIdDb;
            model.Id = data.Id;
            model.Name = data.NameDb;
            model.SortOrder = data.SortOrderDb;
        }

        public override bool Create(ExampleSubObject item)
        {
            item.SortOrder = FindMaxSort(item.BaseId) + 1;
            return base.Create(item);
        }

        private int FindMaxSort(string baseId)
        {
            var result = new List<ExampleSubObject>();
            var maxSort = repository.Find()
                .Where(y => y.BaseIdDb == baseId)
                .Max(y => (int?) y.SortOrderDb);

            return maxSort ?? 0;
        }

        public bool Update(ExampleSubObject item)
        {
            return base.Update(item, new PrimaryKey(item.Id));
        }

        public bool Delete(long id)
        {
            return Delete(new PrimaryKey(id));
        }

        public List<ExampleSubObject> FindForBase(string baseBvin)
        {
            var items = repository.Find().Where(y => y.BaseIdDb == baseBvin)
                .OrderBy(y => y.SortOrderDb);
            return ListPoco(items);
        }

        public void DeleteForBase(string baseBvin)
        {
            var existing = FindForBase(baseBvin);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
        }

        public void MergeList(string baseBvin, List<ExampleSubObject> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.BaseId = baseBvin;
            }

            // Create or Update
            foreach (var itemnew in subitems)
            {
                if (itemnew.Id < 1)
                {
                    Create(itemnew);
                }
                else
                {
                    Update(itemnew);
                }
            }

            // Delete missing
            var existing = FindForBase(baseBvin);
            foreach (var ex in existing)
            {
                var count = (from sub in subitems
                    where sub.Id == ex.Id
                    select sub).Count();
                if (count < 1)
                {
                    Delete(ex.Id);
                }
            }
        }
    }
}