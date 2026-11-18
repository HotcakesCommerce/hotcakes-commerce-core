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
using System.Linq;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Framework;
using DotNetNuke.Services.Journal;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Social;

namespace Hotcakes.Commerce.Dnn.Social
{
    // TODO: Use either this class or SocialServiceBase moving forward - not both
    /// <summary>
    /// Implements DNN journal taxonomy operations for Hotcakes products and categories.
    /// </summary>
    [Serializable]
    public class SocialServiceImpl : SocialServiceBase, ISocialService
    {
        /// <summary>
        /// Initializes the DNN social service implementation.
        /// </summary>
        /// <param name="context">The current Hotcakes request context.</param>
        public SocialServiceImpl(HccRequestContext context)
            : base(context)
        {
        }

        #region Public methods

        /// <summary>
        /// Saves a product to the DNN journal.
        /// </summary>
        /// <param name="product">The product to save.</param>
        public override void SaveProductToJournal(Product product)
        {
            EnsureJournalItem(product);
        }

        /// <summary>
        /// Saves a category to the DNN journal.
        /// </summary>
        /// <param name="cat">The category to save.</param>
        public override void SaveCategoryToJournal(Category cat)
        {
            EnsureJournalItem(cat);
        }

        /// <summary>
        /// Replaces the DNN taxonomy terms associated with a product.
        /// </summary>
        /// <param name="product">The product whose taxonomy should be updated.</param>
        /// <param name="taxonomyTags">The taxonomy term names to associate with the product.</param>
        public override void UpdateProductTaxonomy(Product product, IEnumerable<string> taxonomyTags)
        {
            var journalItem = EnsureJournalItem(product);
            UpdateTaxonomy(journalItem, taxonomyTags);
        }

        /// <summary>
        /// Replaces the DNN taxonomy terms associated with a category.
        /// </summary>
        /// <param name="cat">The category whose taxonomy should be updated.</param>
        /// <param name="taxonomyTags">The taxonomy term names to associate with the category.</param>
        public override void UpdateCategoryTaxonomy(Category cat, IEnumerable<string> taxonomyTags)
        {
            var journalItem = EnsureJournalItem(cat);
            UpdateTaxonomy(journalItem, taxonomyTags);
        }

        /// <summary>
        /// Gets the DNN taxonomy terms associated with a product.
        /// </summary>
        /// <param name="product">The product whose taxonomy terms should be returned.</param>
        /// <returns>The associated taxonomy term names.</returns>
        public override IEnumerable<string> GetTaxonomyTerms(Product product)
        {
            return GetTaxonomyTerms(product.Bvin);
        }

        /// <summary>
        /// Gets the DNN taxonomy terms associated with a category.
        /// </summary>
        /// <param name="category">The category whose taxonomy terms should be returned.</param>
        /// <returns>The associated taxonomy term names.</returns>
        public override IEnumerable<string> GetTaxonomyTerms(Category category)
        {
            return GetTaxonomyTerms(category.Bvin);
        }

        #endregion

        #region Implementation

        private IEnumerable<string> GetTaxonomyTerms(string journalObjectKey)
        {
            var portalId = DnnGlobal.Instance.GetPortalId();
            var journalItem =
                ServiceLocator<IJournalController, JournalController>.Instance.GetJournalItemByKey(portalId,
                    journalObjectKey);
            if (journalItem != null)
            {
                var contentItem = Util.GetContentController().GetContentItem(journalItem.ContentItemId);
                if (contentItem != null)
                    return contentItem.Terms.Select(t => t.Name);
            }
            return new List<string>();
        }

        private void UpdateTaxonomy(JournalItem journalItem, IEnumerable<string> taxonomyTags)
        {
            var contentController = Util.GetContentController();
            var contentItem = contentController.GetContentItem(journalItem.ContentItemId);

            var termController = Util.GetTermController();
            var vocabulary = Util.GetVocabularyController().GetVocabularies().FirstOrDefault(v => v.Name == "Tags");
            var terms = termController.GetTermsByVocabulary(vocabulary.VocabularyId);
            termController.RemoveTermsFromContent(contentItem);
            var clearedTerms = taxonomyTags.Select(t => t.Trim().ToLower()).Distinct();
            foreach (var termName in clearedTerms)
            {
                var term = terms.FirstOrDefault(t => termName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (term == null)
                {
                    term = new Term(termName, null, vocabulary.VocabularyId);
                    term.TermId = termController.AddTerm(term);
                }
                termController.AddTermToContent(term, contentItem);
            }
        }

        #endregion
    }
}
