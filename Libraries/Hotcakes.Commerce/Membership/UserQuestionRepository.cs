#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Membership
{
    public class UserQuestionRepository : HccSimpleRepoBase<hcc_UserQuestions, UserQuestion>
    {
        public UserQuestionRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_UserQuestions data, UserQuestion model)
        {
            model.Bvin = data.bvin;
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.Name = data.QuestionName;
            model.Order = data.Order;
            model.Type = (UserQuestionType) data.QuestionType;
            model.ReadValuesFromXml(data.Values);
        }

        protected override void CopyModelToData(hcc_UserQuestions data, UserQuestion model)
        {
            data.bvin = model.Bvin;
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdated;
            data.QuestionName = model.Name;
            data.Order = model.Order;
            data.QuestionType = (int) model.Type;
            data.Values = model.WriteValuesToXml();
        }

        public UserQuestion Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public UserQuestion FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public override bool Create(UserQuestion item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        public bool Update(UserQuestion c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            return Delete(y => y.bvin == bvin && y.StoreId == storeId);
        }

        public List<UserQuestion> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        public List<UserQuestion> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery().AsNoTracking().Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Order);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPoco(items);
            }
        }

        public bool MoveUp(string bvin)
        {
            var questions = FindAll();
            for (var i = 0; i <= questions.Count - 1; i++)
            {
                if (questions[i].Bvin == bvin)
                {
                    if (i != questions.Count - 1)
                    {
                        questions[i].Order += 1;
                        questions[i + 1].Order -= 1;
                        Update(questions[i]);
                        Update(questions[i + 1]);
                    }
                }
            }
            return true;
        }

        public bool MoveDown(string bvin)
        {
            var questions = FindAll();
            for (var i = 0; i <= questions.Count - 1; i++)
            {
                if (questions[i].Bvin == bvin)
                {
                    if (i > 0)
                    {
                        questions[i].Order -= 1;
                        questions[i - 1].Order += 1;
                        Update(questions[i]);
                        Update(questions[i - 1]);
                    }
                }
            }
            return true;
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}