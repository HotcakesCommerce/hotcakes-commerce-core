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

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Describes an entity containing culture-specific translation data.
    /// </summary>
    public interface ITranslationEntity
    {
        /// <summary>
        /// Gets or sets the culture associated with the translation.
        /// </summary>
        string Culture { get; set; }
    }

    /// <summary>
    /// Describes a model whose content can be associated with a culture.
    /// </summary>
    public interface ILocalizableModel
    {
        /// <summary>
        /// Gets or sets the culture associated with the model content.
        /// </summary>
        string ContentCulture { get; set; }
    }

    /// <summary>
    /// Holds a primary entity together with its translation entity.
    /// </summary>
    /// <typeparam name="T">The primary entity type.</typeparam>
    /// <typeparam name="TT">The translation entity type.</typeparam>
    [Serializable]
    public class JoinedItem<T, TT>
        where T : class, new()
        where TT : class, ITranslationEntity, new()
    {
        /// <summary>
        /// Initializes a new instance with a new primary entity and translation entity.
        /// </summary>
        public JoinedItem()
        {
            Item = new T();
            ItemTranslation = new TT();
        }

        /// <summary>
        /// Gets or sets the primary entity.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Gets or sets the translation entity.
        /// </summary>
        public TT ItemTranslation { get; set; }
    }
}
