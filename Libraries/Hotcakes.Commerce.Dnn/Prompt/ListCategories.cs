#region License

// Distributed under the MIT License
// ============================================================
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
using System.Linq;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using DotNetNuke.Instrumentation;

namespace Hotcakes.Commerce.Dnn.Prompt
{
    [ConsoleCommand("list-categories", Constants.Namespace, "PromptListCategories")]
    public class ListCategories: PromptBase, IConsoleCommand
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ListCategories));

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
                var categories = HccApp.CatalogServices.Categories.FindAll();

                if (categories == null || !categories.Any())
                {
                    return new ConsoleResultModel
                    {
                        Output = string.Concat(Constants.OutputPrefix, string.Format(LocalizeString("CategoriesFound"), 0))
                    };
                }
                
                var list = categories.OrderBy(c => c.Name).Select(category => new CategoryOutput
                {
                    CategoryName = category.Name,
                    ProductCount = HccApp.CatalogServices.CategoriesXProducts.FindForCategory(category.Bvin, 1, int.MaxValue).Count,
                    Slug = category.RewriteUrl,
                    TemplateName = HccApp.CatalogServices.Categories.FindBySlug(category.RewriteUrl).TemplateName,
                    CategoryID = category.Bvin
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
                    Output = string.Concat(Constants.OutputPrefix, string.Format(LocalizeString("CategoriesFound"), count)) 
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

    public class CategoryOutput
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public string Slug { get; set; }
        public string TemplateName { get; set; }
        public string CategoryID { get; set; }
    }
}
