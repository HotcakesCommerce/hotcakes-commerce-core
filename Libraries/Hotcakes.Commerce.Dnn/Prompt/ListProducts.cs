#region License

// Distributed under the MIT License
// ============================================================
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
using System.Linq;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using DotNetNuke.Instrumentation;

namespace Hotcakes.Commerce.Dnn.Prompt
{
    /// <summary>
    /// Implements the prompt command that lists the products in the current Hotcakes Commerce store.
    /// </summary>
    [ConsoleCommand("list-products", Constants.Namespace, "PromptListProducts")]
    public class ListProducts: PromptBase, IConsoleCommand
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ListProducts));

        /*
        [ConsoleCommandParameter("categoryid", "ListCategories_FlagCategoryId", "String", false)]
        public string CategoryId { get; set; } = "";

        [ConsoleCommandParameter("page", "ListCategories_FlagPage", "1")]
        public int Page { get; set; } = 1;

        [ConsoleCommandParameter("max", "ListCategories_FlagMax", "10")]
        public int Max { get; set; } = 10;

        [ConsoleCommandParameter("name", "ListCategories_FlagCategoryName")]
        public string CategoryName { get; set; }

        [ConsoleCommandParameter("deleted", "ListCategories_FlagDisabled")]
        public bool? Disabled { get; set; }
        */

        /// <summary>
        /// Retrieves products from the current store and returns them ordered by product name.
        /// </summary>
        /// <returns>A prompt result containing <see cref="ProductOutput"/> data and the number of products found, or an error result.</returns>
        public override ConsoleResultModel Run()
        {
            try
            {
                var products = HccApp.CatalogServices.Products.FindAllPaged(1, Int32.MaxValue);

                if (products == null || !products.Any())
                {
                    return new ConsoleResultModel
                    {
                        Output = string.Concat(Constants.OutputPrefix, string.Format(LocalizeString("ProductsFound"), 0))
                    };
                }
                
                var list = products.OrderBy(c => c.ProductName).Select(product => new ProductOutput
                {
                    ProductName = product.ProductName,
                    Sku = product.Sku,
                    Price = product.SitePrice,
                    Slug = product.UrlSlug,
                    ProductType = HccApp.CatalogServices.ProductTypes.Find(product.ProductTypeId) != null ? HccApp.CatalogServices.ProductTypes.Find(product.ProductTypeId).ProductTypeName : LocalizeString("ProductTypeDefault"),
                    TemplateName = product.TemplateName,
                    ProductID = product.Bvin
                }).ToList();
                var count = list.Count;

                /*
                var max = Max <= 0 ? 10 : (Max > 500 ? 500 : Max);
                var pageIndex = (Page > 0 ? Page - 1 : 0);
                pageIndex = pageIndex < 0 ? 0 : pageIndex;
                var pageSize = Max;
                pageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
                var totalPages = total / max + (total % max == 0 ? 0 : 1);
                var pageNo = Page > 0 ? Page : 1;
                */

                return new ConsoleResultModel
                {
                    Data = list,
                    Records = count,
                    Output = string.Concat(Constants.OutputPrefix, string.Format(LocalizeString("ProductsFound"), count)) 
                };
            }
            catch (Exception e)
            {
                LogError(e);
                return new ConsoleErrorResultModel(string.Concat(Constants.OutputPrefix, LocalizeString("ErrorOccurred")));
            }
        }

        protected override void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);
                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }
    }

    /// <summary>
    /// Contains the product fields returned by the list-products prompt command.
    /// </summary>
    public class ProductOutput
    {
        /// <summary>
        /// Gets or sets the product display name.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the product SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the product site price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product URL slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the product type name.
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the name of the product template.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the product business identifier.
        /// </summary>
        public string ProductID { get; set; }
    }
}

