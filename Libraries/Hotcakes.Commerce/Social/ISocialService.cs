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

using System.Collections.Generic;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Social
{
    /// <summary>
    /// Provides the social-journal and taxonomy operations for catalog products, product reviews, and categories.
    /// </summary>
    /// <remarks>
    /// Journal operations synchronize catalog records with the social platform's content representation. Taxonomy
    /// operations associate and retrieve terms for the corresponding journal record.
    /// </remarks>
    public interface ISocialService
    {
        /// <summary>
        /// Updates the social journal record for a <see cref="Product"/>.
        /// </summary>
        /// <param name="product">The product whose journal record is updated.</param>
        void UpdateJournalRecord(Product product);

        /// <summary>
        /// Updates the social journal record for a <see cref="ProductReview"/> associated with a product.
        /// </summary>
        /// <param name="review">The product review whose journal record is updated.</param>
        /// <param name="product">The product associated with the review.</param>
        void UpdateJournalRecord(ProductReview review, Product product);

        /// <summary>
        /// Updates the social journal record for a <see cref="Category"/>.
        /// </summary>
        /// <param name="cat">The category whose journal record is updated.</param>
        void UpdateJournalRecord(Category cat);

        /// <summary>
        /// Replaces or updates the taxonomy terms associated with a product's social journal record.
        /// </summary>
        /// <param name="product">The product whose taxonomy is updated.</param>
        /// <param name="taxonomyTags">The taxonomy terms to associate with the product.</param>
        void UpdateProductTaxonomy(Product product, IEnumerable<string> taxonomyTags);

        /// <summary>
        /// Replaces or updates the taxonomy terms associated with a category's social journal record.
        /// </summary>
        /// <param name="cat">The category whose taxonomy is updated.</param>
        /// <param name="taxonomyTags">The taxonomy terms to associate with the category.</param>
        void UpdateCategoryTaxonomy(Category cat, IEnumerable<string> taxonomyTags);

        /// <summary>
        /// Gets the taxonomy terms associated with a product's social journal record.
        /// </summary>
        /// <param name="product">The product whose taxonomy terms are returned.</param>
        /// <returns>The taxonomy terms associated with the product.</returns>
        IEnumerable<string> GetTaxonomyTerms(Product product);

        /// <summary>
        /// Gets the taxonomy terms associated with a category's social journal record.
        /// </summary>
        /// <param name="category">The category whose taxonomy terms are returned.</param>
        /// <returns>The taxonomy terms associated with the category.</returns>
        IEnumerable<string> GetTaxonomyTerms(Category category);

        /// <summary>
        /// Saves a product as a social journal record.
        /// </summary>
        /// <param name="product">The product to save to the social journal.</param>
        void SaveProductToJournal(Product product);

        /// <summary>
        /// Saves a category as a social journal record.
        /// </summary>
        /// <param name="cat">The category to save to the social journal.</param>
        void SaveCategoryToJournal(Category cat);
    }
}
