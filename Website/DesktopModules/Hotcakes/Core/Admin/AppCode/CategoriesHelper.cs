#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class CategoriesHelper
    {
        private const string HIDDEN_TEXT = " &nbsp;(hidden)";

        public static Collection<ListItem> ListFullTreeWithIndents(ICategoryRepository repo)
        {
            return ListFullTreeWithIndents(repo, false);
        }

        public static Collection<ListItem> ListFullTreeWithIndents(ICategoryRepository repo, bool showHidden)
        {
            var allCats = repo.FindAllSnapshotsPaged(1, int.MaxValue);
            return ListFullTreeWithIndents(allCats, showHidden);
        }

        public static Collection<ListItem> ListFullTreeWithIndents(List<CategorySnapshot> allCats)
        {
            return ListFullTreeWithIndents(allCats, false);
        }

        public static Collection<ListItem> ListFullTreeWithIndents(List<CategorySnapshot> allCats, bool showHidden)
        {
            var result = new Collection<ListItem>();
            AddIndentedChildren(ref result, string.Empty, 0, ref allCats, showHidden);
            return result;
        }

        private static void AddIndentedChildren(ref Collection<ListItem> result, string parentId, int currentDepth,
            ref List<CategorySnapshot> allCats, bool showHidden)
        {
            var children = Category.FindChildrenInList(allCats, parentId, showHidden);
            if (children != null)
            {
                foreach (var c in children)
                {
                    var spacer = new StringBuilder();

                    for (var i = 0; i <= currentDepth - 1; i++)
                    {
                        spacer.Append("...");
                    }

                    var li = new ListItem();
                    li.Value = c.Bvin;

                    if (showHidden && c.Hidden)
                    {
                        li.Text = spacer + c.Name + HIDDEN_TEXT;
                    }
                    else
                    {
                        li.Text = spacer + c.Name;
                    }

                    result.Add(li);

                    AddIndentedChildren(ref result, c.Bvin, currentDepth + 1, ref allCats, showHidden);
                }
            }
        }


        public static Collection<ListItem> ListFullTreeWithIndentsForComboBox(List<CategorySnapshot> allCats,
            bool showHidden)
        {
            var result = new Collection<ListItem>();
            AddIndentedChildrenForComboBox(ref result, string.Empty, 0, ref allCats, showHidden);
            return result;
        }

        private static void AddIndentedChildrenForComboBox(ref Collection<ListItem> result, string parentId,
            int currentDepth, ref List<CategorySnapshot> allCats, bool showHidden)
        {
            var children = Category.FindChildrenInList(allCats, parentId, showHidden);
            if (children != null)
            {
                foreach (var c in children)
                {
                    var spacer = new StringBuilder();

                    for (var i = 0; i <= currentDepth - 1; i++)
                    {
                        spacer.Append("...");
                    }

                    var li = new ListItem();
                    li.Value = c.RewriteUrl;

                    if (showHidden && c.Hidden)
                    {
                        li.Text = spacer + c.Name + " &nbsp;(hidden)";
                    }
                    else
                    {
                        li.Text = spacer + c.Name;
                    }

                    result.Add(li);

                    AddIndentedChildrenForComboBox(ref result, c.Bvin, currentDepth + 1, ref allCats, showHidden);
                }
            }
        }
    }
}