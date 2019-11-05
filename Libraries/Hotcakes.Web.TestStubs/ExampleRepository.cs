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
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.TestStubs
{
    public class ExampleRepository : ConvertingRepositoryBase<ExampleBaseDb, ExampleBase>
    {
        private readonly ExampleSubRepository subrepository;

        private ExampleRepository(IRepositoryStrategy<ExampleBaseDb> r, IRepositoryStrategy<ExampleSubObjectDb> subr)
        {
            repository = r;
            logger = new NullLogger();
            repository.Logger = logger;

            subrepository = new ExampleSubRepository(subr, logger);
        }

        public static ExampleRepository InstantiateForMemory()
        {
            ExampleRepository result = null;

            result = new ExampleRepository(new MemoryStrategy<ExampleBaseDb>(PrimaryKeyType.Bvin),
                new MemoryStrategy<ExampleSubObjectDb>(PrimaryKeyType.Long));

            return result;
        }

        public static ExampleRepository InstantiateForDatabase()
        {
            throw new NotImplementedException();
        }

        protected override void CopyModelToData(ExampleBaseDb data, ExampleBase model)
        {
            data.bvin = model.bvin;
            data.DescriptionDb = model.Description;
            data.IsActiveDb = model.IsActive;
            data.LastUpdatedUtcDb = model.LastUpdatedUtc;
        }

        protected override void CopyDataToModel(ExampleBaseDb data, ExampleBase model)
        {
            model.bvin = data.bvin;
            model.Description = data.DescriptionDb;
            model.IsActive = data.IsActiveDb;
            model.LastUpdatedUtc = data.LastUpdatedUtcDb;
        }

        protected override void DeleteAllSubItems(ExampleBase model)
        {
            subrepository.DeleteForBase(model.bvin);
        }

        protected override void GetSubItems(ExampleBase model)
        {
            model.SubObjects = subrepository.FindForBase(model.bvin);
        }

        protected override void MergeSubItems(ExampleBase model)
        {
            subrepository.MergeList(model.bvin, model.SubObjects);
        }


        public override bool Create(ExampleBase item)
        {
            if (item == null) return false;

            if (item.bvin == string.Empty)
                item.bvin = Guid.NewGuid().ToString();
            item.LastUpdatedUtc = DateTime.UtcNow;

            return base.Create(item);
        }

        public bool Update(ExampleBase item)
        {
            return base.Update(item, new PrimaryKey(item.bvin));
        }

        public ExampleBase Find(string bvin)
        {
            return Find(new PrimaryKey(bvin));
        }

        public bool Delete(string bvin)
        {
            return Delete(new PrimaryKey(bvin));
        }

        public List<ExampleSubObject> PeakIntoSubObjects(string baseBvin)
        {
            return subrepository.FindForBase(baseBvin);
        }
    }
}