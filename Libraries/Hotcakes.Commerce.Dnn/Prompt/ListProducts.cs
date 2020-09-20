#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 Upendo Ventures, LLC
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
    [ConsoleCommand("list-products", Constants.Namespace, "Lists the products in the current Hotcakes Commerce store.")]
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

        public override ConsoleResultModel Run()
        {
            try
            {
                var products = HccApp.CatalogServices.Products.FindAllPaged(1, Int32.MaxValue);

                if (products == null || !products.Any())
                {
                    return new ConsoleResultModel
                    {
                        Output = string.Format(LocalizeString("ProductsFound"), 0)
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
                    Output = string.Format(LocalizeString("ProductsFound"), count) 
                };
            }
            catch (Exception e)
            {
                LogError(e);
                return new ConsoleErrorResultModel(LocalizeString("ErrorOccurred"));
            }
        }

        private void LogError(Exception ex)
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

    public class ProductOutput
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string TemplateName { get; set; }
        public string Slug { get; set; }
        public string Sku { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
    }
}
