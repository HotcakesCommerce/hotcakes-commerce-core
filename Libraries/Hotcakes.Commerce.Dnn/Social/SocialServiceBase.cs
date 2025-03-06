#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Journal;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Dnn.Social
{
    [Serializable]
    public class SocialServiceBase : ISocialService
    {
        public SocialServiceBase(HccRequestContext context)
        {
            Context = context;
        }

        protected JournalItem EnsureJournalItem(Product product, bool updateExisting = false)
        {
            var journalTypeId = EnsureJournalType(JORNALTYPE_PRODUCTADD);
            var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
            var dnnUserId = dnnUser != null ? dnnUser.UserID : CurrentPortalSettings.AdministratorId;

            var jItem =
                ServiceLocator<IJournalController, JournalController>.Instance.GetJournalItemByKey(
                    CurrentPortalSettings.PortalId, product.Bvin);

            if (jItem != null && !updateExisting)
                return jItem;

            var isCreateRequired = false;
            if (jItem == null)
            {
                jItem = new JournalItem
                {
                    JournalTypeId = journalTypeId,
                    ObjectKey = product.Bvin,
                    PortalId = CurrentPortalSettings.PortalId
                };
                isCreateRequired = true;
            }

            var contentItemId = EnsureProductContentItem(product, jItem.ContentItemId);

            jItem.ContentItemId = contentItemId;
            jItem.Title = product.ProductName;
            jItem.Summary = product.MetaDescription;
            jItem.UserId = dnnUserId;
            jItem.ItemData = new ItemData
            {
                Title = product.ProductName,
                Url = HccUrlBuilder.RouteHccUrl(HccRoute.Product, new {slug = product.UrlSlug})
            };

#pragma warning disable 618
            if (isCreateRequired)
                ServiceLocator<IJournalController, JournalController>.Instance.SaveJournalItem(jItem, Null.NullInteger);
            else
                ServiceLocator<IJournalController, JournalController>.Instance.UpdateJournalItem(jItem, Null.NullInteger);
#pragma warning restore 618

            return jItem;
        }

        protected JournalItem EnsureJournalItem(ProductReview review, Product product, bool updateExisting = false)
        {
            if (!review.Approved)
                return null;

            var jItem =
                ServiceLocator<IJournalController, JournalController>.Instance.GetJournalItemByKey(
                    CurrentPortalSettings.PortalId, review.Bvin);

            if (jItem != null && !updateExisting)
                return jItem;

            var journalTypeId = EnsureJournalType(JORNALTYPE_PRODUCTREVIEWADD);

            int dnnUserId;
            if (!int.TryParse(review.UserID, out dnnUserId))
            {
                var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
                dnnUserId = dnnUser != null ? dnnUser.UserID : CurrentPortalSettings.AdministratorId;
            }

            var isCreateRequired = false;
            if (jItem == null)
            {
                jItem = new JournalItem
                {
                    JournalTypeId = journalTypeId,
                    ObjectKey = review.Bvin,
                    PortalId = CurrentPortalSettings.PortalId
                };
                isCreateRequired = true;
            }

            var contentItemId = EnsureProductReviewContentItem(review, product, jItem.ContentItemId);

            jItem.ContentItemId = contentItemId;
            jItem.Title = product.ProductName;
            jItem.Summary = review.Description;
            jItem.UserId = dnnUserId;
            jItem.ItemData = new ItemData
            {
                Title = product.ProductName,
                Url = HccUrlBuilder.RouteHccUrl(HccRoute.Product, new {slug = product.UrlSlug})
            };

            if (isCreateRequired)
            {
#pragma warning disable 618
                ServiceLocator<IJournalController, JournalController>.Instance.SaveJournalItem(jItem, Null.NullInteger);
#pragma warning restore 618
            }
            else
            {
#pragma warning disable 618
                ServiceLocator<IJournalController, JournalController>.Instance.UpdateJournalItem(jItem, Null.NullInteger);
#pragma warning restore 618
            }

            return jItem;
        }

        protected JournalItem EnsureJournalItem(Category cat, bool updateExisting = false)
        {
            var journalTypeId = EnsureJournalType(JORNALTYPE_CATEGORYADD);
            var dnnUser = DnnUserController.Instance.GetCurrentUserInfo();
            var dnnUserId = dnnUser != null ? dnnUser.UserID : CurrentPortalSettings.AdministratorId;

            var jItem =
                ServiceLocator<IJournalController, JournalController>.Instance.GetJournalItemByKey(
                    CurrentPortalSettings.PortalId, cat.Bvin);

            if (jItem != null && !updateExisting)
                return jItem;

            var isCreateRequired = false;
            if (jItem == null)
            {
                jItem = new JournalItem
                {
                    JournalTypeId = journalTypeId,
                    ObjectKey = cat.Bvin,
                    PortalId = CurrentPortalSettings.PortalId
                };
                isCreateRequired = true;
            }

            var contentItemId = EnsureCategoryContentItem(cat, jItem.ContentItemId);

            jItem.ContentItemId = contentItemId;
            jItem.Title = cat.Name;
            jItem.Summary = cat.MetaDescription;
            jItem.UserId = dnnUserId;
            jItem.ItemData = new ItemData
            {
                Title = cat.Name,
                Url = HccUrlBuilder.RouteHccUrl(HccRoute.Category, new {slug = cat.RewriteUrl})
            };

#pragma warning disable 618
            if (isCreateRequired)
                JournalController.Instance.SaveJournalItem(jItem, Null.NullInteger);
            else
                JournalController.Instance.UpdateJournalItem(jItem, Null.NullInteger);
#pragma warning restore 618

            return jItem;
        }

        private static int EnsureContentType(string contentTypeName)
        {
            var contentTypeController = (IContentTypeController) new ContentTypeController();
            var contentType =
                contentTypeController.GetContentTypes().FirstOrDefault(type => type.ContentType == contentTypeName);
            if (contentType == null)
            {
                contentType = new ContentType(contentTypeName);
                contentType.ContentTypeId = contentTypeController.AddContentType(contentType);
            }
            return contentType.ContentTypeId;
        }

        private static int EnsureJournalType(string journalType)
        {
            var jType = JournalController.Instance.GetJournalType(journalType);

            if (jType == null)
            {
                return JournalDataService.Instance.Journal_Types_Save(
                    -1,
                    journalType,
                    string.Empty, //icon
                    -1, //portalId
                    true, //isEnabled
                    true, //appliesToProfile
                    true, //appliesToGroup
                    true, //appliesToStream
                    null, //options
                    true); //supportsNotify
            }

            return jType.JournalTypeId;
        }

        private int EnsureProductContentItem(Product product, int contentItemId = -1)
        {
            var urlSettings = Context.CurrentStore.Settings.Urls;
            var ci = contentItemId > 0 ? Util.GetContentController().GetContentItem(contentItemId) : null;

            if (ci != null)
            {
                ci.Content = product.ProductName;
                ci.ContentKey = string.Format(CONTENT_KEY, product.UrlSlug);
                ci.ContentTitle = product.ProductName;
                ci.TabID = urlSettings.ProductTabId;

                Util.GetContentController().UpdateContentItem(ci);
                return ci.ContentItemId;
            }
            return AddContentItem(CONTENTTYPE_PRODUCT, product.ProductName, product.ProductName,
                string.Format(CONTENT_KEY, product.UrlSlug), urlSettings.ProductTabId);
        }

        private int EnsureProductReviewContentItem(ProductReview review, Product product, int contentItemId = -1)
        {
            var urlSettings = Context.CurrentStore.Settings.Urls;
            var ci = contentItemId > 0 ? Util.GetContentController().GetContentItem(contentItemId) : null;

            if (ci != null)
            {
                ci.Content = product.ProductName;
                ci.ContentKey = string.Format(CONTENT_KEY, product.UrlSlug);
                ci.ContentTitle = product.ProductName;
                ci.TabID = urlSettings.ProductReviewTabId;

                Util.GetContentController().UpdateContentItem(ci);
                return ci.ContentItemId;
            }
            return AddContentItem(CONTENTTYPE_PRODUCT_REVIEW, product.ProductName, product.ProductName,
                string.Format(CONTENT_KEY, product.UrlSlug), urlSettings.ProductReviewTabId);
        }

        private int EnsureCategoryContentItem(Category cat, int contentItemId = -1)
        {
            var urlSettings = Context.CurrentStore.Settings.Urls;
            var ci = contentItemId > 0 ? Util.GetContentController().GetContentItem(contentItemId) : null;

            if (ci != null)
            {
                ci.Content = cat.Name;
                ci.ContentKey = string.Format(CONTENT_KEY, cat.RewriteUrl);
                ci.ContentTitle = cat.Name;
                ci.TabID = urlSettings.CategoryTabId;

                Util.GetContentController().UpdateContentItem(ci);
                return ci.ContentItemId;
            }
            return AddContentItem(CONTENTTYPE_CATEGORY, cat.Name, cat.Name, string.Format(CONTENT_KEY, cat.RewriteUrl),
                urlSettings.CategoryTabId);
        }

        private static int AddContentItem(string contentTypeName, string content, string title, string contentKey,
            int tabId)
        {
            var contentTypeId = EnsureContentType(contentTypeName);

            var ci = new ContentItem
            {
                Content = content,
                ContentTitle = title,
                ContentTypeId = contentTypeId,
                Indexed = false,
                ContentKey = contentKey,
                ModuleID = -1,
                TabID = tabId
            };

            return Util.GetContentController().AddContentItem(ci);
        }

        #region Fields

        private const string JORNALTYPE_PRODUCTADD = "productadd";
        private const string JORNALTYPE_PRODUCTREVIEWADD = "productreviewadd";
        private const string JORNALTYPE_CATEGORYADD = "categoryadd";

        private const string CONTENTTYPE_PRODUCT = "Hotcakes_Product";
        private const string CONTENTTYPE_PRODUCT_REVIEW = "Hotcakes_Product_Review";
        private const string CONTENTTYPE_CATEGORY = "Hotcakes_Category";

        private const string CONTENT_KEY = "slug={0}";

        #endregion

        #region Properties

        protected HccRequestContext Context { get; set; }

        protected PortalSettings CurrentPortalSettings
        {
            get { return DnnGlobal.Instance.GetCurrentPortalSettings(); }
        }

        #endregion

        #region ISocialService

        public virtual void UpdateJournalRecord(Product product)
        {
            EnsureJournalItem(product, true);
        }

        public virtual void UpdateJournalRecord(ProductReview review, Product product)
        {
            EnsureJournalItem(review, product, true);
        }

        public virtual void UpdateJournalRecord(Category cat)
        {
            EnsureJournalItem(cat, true);
        }

        public virtual void UpdateProductTaxonomy(Product product, IEnumerable<string> taxonomyTags)
        {
        }

        public virtual void UpdateCategoryTaxonomy(Category cat, IEnumerable<string> taxonomyTags)
        {
        }

        public virtual IEnumerable<string> GetTaxonomyTerms(Product product)
        {
            return new List<string>();
        }

        public virtual IEnumerable<string> GetTaxonomyTerms(Category category)
        {
            return new List<string>();
        }

        public virtual void SaveProductToJournal(Product product)
        {
        }

        public virtual void SaveCategoryToJournal(Category cat)
        {
        }

        #endregion
    }
}