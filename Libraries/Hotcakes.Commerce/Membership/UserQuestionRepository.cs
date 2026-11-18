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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Provides persistence for store-scoped <see cref="UserQuestion"/> records and their XML-serialized options.
    /// </summary>
    public class UserQuestionRepository : HccSimpleRepoBase<hcc_UserQuestions, UserQuestion>
    {
        /// <summary>
        /// Initializes the repository with the commerce request context.
        /// </summary>
        /// <param name="c">The request context used to resolve the current store and data access services.</param>
        public UserQuestionRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Copies persistence fields into a question model, including deserializing its option XML.
        /// </summary>
        /// <param name="data">The persistence record.</param>
        /// <param name="model">The model to populate.</param>
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

        /// <summary>
        /// Copies a question model into a persistence record, including serializing its options to XML.
        /// </summary>
        /// <param name="data">The persistence record to populate.</param>
        /// <param name="model">The model supplying the values.</param>
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

        /// <summary>
        /// Finds a question by identifier.
        /// </summary>
        /// <param name="bvin">The question identifier.</param>
        /// <returns>The first matching question, or <see langword="null"/> when none is found.</returns>
        /// <remarks>Despite the repository's store-scoped create, update, delete, and list operations, this lookup does not add a store predicate.</remarks>
        public UserQuestion Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>
        /// Finds a question by identifier without adding a store predicate.
        /// </summary>
        /// <param name="bvin">The question identifier.</param>
        /// <returns>The first matching question across all stores, or <see langword="null"/> when none is found.</returns>
        public UserQuestion FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>
        /// Creates a question for the current store, assigning an identifier when needed and refreshing its UTC timestamp.
        /// </summary>
        /// <param name="item">The question to persist.</param>
        /// <returns>The result returned by the base repository create operation.</returns>
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

        /// <summary>
        /// Updates a question only when it belongs to the current store.
        /// </summary>
        /// <param name="c">The question to update.</param>
        /// <returns><see langword="false"/> when the question belongs to another store; otherwise, the base update result.</returns>
        public bool Update(UserQuestion c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        /// <summary>
        /// Deletes a question from the current store.
        /// </summary>
        /// <param name="bvin">The question identifier.</param>
        /// <returns>The result of the store-scoped delete operation.</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            return Delete(y => y.bvin == bvin && y.StoreId == storeId);
        }

        /// <summary>
        /// Gets all questions for the current store in ascending <see cref="UserQuestion.Order"/> order.
        /// </summary>
        /// <returns>All questions for the current store.</returns>
        public List<UserQuestion> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        /// <summary>
        /// Gets one page of questions for the current store in ascending <see cref="UserQuestion.Order"/> order.
        /// </summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of questions in the page.</param>
        /// <param name="totalCount">Receives the total number of questions for the current store before paging.</param>
        /// <returns>The requested page of questions.</returns>
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

        /// <summary>
        /// Moves a question toward the end of the store's ordered list by exchanging its order with the following item.
        /// </summary>
        /// <param name="bvin">The question identifier.</param>
        /// <returns>Always <see langword="true"/>, including when the identifier is not found or the question is already last.</returns>
        /// <remarks>The method name is retained for compatibility. With the ascending order used by <see cref="FindAll"/>, its implementation increments the matching question's order and decrements the next question's order.</remarks>
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

        /// <summary>
        /// Moves a question toward the beginning of the store's ordered list by exchanging its order with the preceding item.
        /// </summary>
        /// <param name="bvin">The question identifier.</param>
        /// <returns>Always <see langword="true"/>, including when the identifier is not found or the question is already first.</returns>
        /// <remarks>The method name is retained for compatibility. With the ascending order used by <see cref="FindAll"/>, its implementation decrements the matching question's order and increments the previous question's order.</remarks>
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
