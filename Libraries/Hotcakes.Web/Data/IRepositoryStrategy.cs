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

using System.Linq;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Defines persistence operations implemented by a repository strategy.
    /// </summary>
    /// <typeparam name="T">The model type managed by the strategy.</typeparam>
    public interface IRepositoryStrategy<T>
        where T : class, new()
    {
        /// <summary>
        /// Gets or sets a value indicating whether successful mutations are submitted immediately.
        /// </summary>
        bool AutoSubmit { get; set; }

        /// <summary>
        /// Gets or sets the logger used to record persistence diagnostics.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Persists pending changes.
        /// </summary>
        /// <returns><see langword="true"/> when the changes are submitted successfully; otherwise, <see langword="false"/>.</returns>
        bool SubmitChanges();

        /// <summary>
        /// Finds a model by its <see cref="PrimaryKey"/>.
        /// </summary>
        /// <param name="id">The key identifying the model.</param>
        /// <returns>The matching model, or <see langword="null"/> when no model is found.</returns>
        T FindByPrimaryKey(PrimaryKey id);

        /// <summary>
        /// Gets a queryable sequence of all models.
        /// </summary>
        /// <returns>A queryable model sequence.</returns>
        IQueryable<T> Find();

        /// <summary>
        /// Adds a model to the repository.
        /// </summary>
        /// <param name="item">The model to add.</param>
        /// <returns><see langword="true"/> when the model is added successfully; otherwise, <see langword="false"/>.</returns>
        bool Create(T item);

        /// <summary>
        /// Deletes the model identified by a <see cref="PrimaryKey"/>.
        /// </summary>
        /// <param name="id">The key identifying the model to delete.</param>
        /// <returns><see langword="true"/> when a model is deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(PrimaryKey id);

        /// <summary>
        /// Detaches a model from the strategy's tracking context.
        /// </summary>
        /// <param name="item">The model to detach.</param>
        void Detach(T item);

        /// <summary>
        /// Gets the number of models currently available in the repository.
        /// </summary>
        /// <returns>The total model count.</returns>
        int CountOfAll();
    }
}
