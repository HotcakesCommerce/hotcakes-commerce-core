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

using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Defines the basic persistence operations exposed by a repository for a model type.
    /// </summary>
    /// <typeparam name="T">The model type managed by the repository.</typeparam>
    public interface IRepository<T>
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
        /// Adds a model to the repository.
        /// </summary>
        /// <param name="item">The model to add.</param>
        /// <returns><see langword="true"/> when the model is added successfully; otherwise, <see langword="false"/>.</returns>
        bool Create(T item);

        /// <summary>
        /// Gets the number of models currently available in the repository.
        /// </summary>
        /// <returns>The total model count.</returns>
        int CountOfAll();
    }
}
