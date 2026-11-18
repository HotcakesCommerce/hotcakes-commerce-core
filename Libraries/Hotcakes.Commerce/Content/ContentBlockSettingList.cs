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
using System.Linq;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Stores the ordered, named list items used by a <see cref="ContentBlock"/>.
    /// </summary>
    public class ContentBlockSettingList
    {
        /// <summary>
        /// Initializes an empty list collection.
        /// </summary>
        public ContentBlockSettingList()
        {
            Items = new List<ContentBlockSettingListItem>();
        }

        /// <summary>
        /// Gets or sets all list items, including items from every list name.
        /// </summary>
        public List<ContentBlockSettingListItem> Items { get; set; }

        /// <summary>
        /// Finds items in a named list ordered by <see cref="ContentBlockSettingListItem.SortOrder"/>.
        /// </summary>
        /// <param name="listName">The list name to match.</param>
        /// <returns>A new list containing matching items in sort order.</returns>
        public List<ContentBlockSettingListItem> FindList(string listName)
        {
            return Items.Where(y => y.ListName == listName).OrderBy(y => y.SortOrder).ToList();
        }

        /// <summary>
        /// Finds the single item with the specified identifier.
        /// </summary>
        /// <param name="bvin">The item identifier.</param>
        /// <returns>The matching item, or <see langword="null"/> when none exists.</returns>
        /// <remarks>The underlying query expects at most one matching item.</remarks>
        public ContentBlockSettingListItem FindSingleItem(string bvin)
        {
            return Items.Where(y => y.Id == bvin).SingleOrDefault();
        }

        /// <summary>
        /// Adds an item after assigning it the next sort position in its named list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><see langword="true"/> after the item is added.</returns>
        public bool AddItem(ContentBlockSettingListItem item)
        {
            var maxSort = FindMaxSort(item.ListName);
            item.SortOrder = maxSort + 1;
            Items.Add(item);
            return true;
        }

        /// <summary>
        /// Removes the item with the specified identifier.
        /// </summary>
        /// <param name="bvin">The item identifier.</param>
        /// <returns><see langword="true"/> when an item was found and removed.</returns>
        public bool RemoveItem(string bvin)
        {
            var item = FindSingleItem(bvin);
            if (item != null)
            {
                return Items.Remove(item);
            }

            return false;
        }

        /// <summary>
        /// Removes every item belonging to a named list.
        /// </summary>
        /// <param name="listName">The list name to remove.</param>
        /// <returns><see langword="true"/> after removal is attempted.</returns>
        public bool DeleteList(string listName)
        {
            Items.RemoveAll(y => y.ListName == listName);
            return true;
        }

        /// <summary>
        /// Gets the highest sort position currently used by a named list.
        /// </summary>
        /// <param name="listName">The list name to inspect.</param>
        /// <returns>The maximum sort position, or zero when the list is empty.</returns>
        public int FindMaxSort(string listName)
        {
            var result = 0;

            var current = FindList(listName);
            if (current != null)
            {
                if (current.Count > 0)
                {
                    var max = current.Max(y => y.SortOrder);
                    return max;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates named-list sort positions from the supplied identifier order.
        /// </summary>
        /// <param name="ids">Identifiers in the desired order.</param>
        /// <param name="listName">The list name to update.</param>
        /// <remarks>Positions are assigned from <see cref="IList{T}.IndexOf(T)"/>, so the first item receives zero.</remarks>
        public void UpdateSortOrder(IList<string> ids, string listName)
        {
            var list = FindList(listName);
            foreach (var item in list)
            {
                item.SortOrder = ids.IndexOf(item.Id);
            }
        }

        /// <summary>
        /// Moves an item toward the beginning of its named list by exchanging sort positions.
        /// </summary>
        /// <param name="bvin">The item identifier.</param>
        /// <param name="listName">The list name containing the item.</param>
        /// <returns><see langword="true"/> after the operation is attempted.</returns>
        public bool MoveItemUp(string bvin, string listName)
        {
            var current = FindList(listName);

            var previous = 1;
            var currentSort = 0;
            var previousId = string.Empty;
            foreach (var item in current)
            {
                if (item.Id == bvin)
                {
                    currentSort = item.SortOrder;
                    item.SortOrder = previous;
                    break;
                }
                previous = item.SortOrder;
                previousId = item.Id;
            }

            var prev = FindSingleItem(previousId);
            if (prev != null)
            {
                prev.SortOrder = currentSort;
            }

            return true;
        }

        /// <summary>
        /// Moves an item toward the end of its named list by exchanging sort positions.
        /// </summary>
        /// <param name="bvin">The item identifier.</param>
        /// <param name="listName">The list name containing the item.</param>
        /// <returns><see langword="true"/> after the operation is attempted.</returns>
        public bool MoveItemDown(string bvin, string listName)
        {
            var current = FindList(listName);

            var next = 1;
            var currentSort = 0;
            var foundCurrent = false;

            foreach (var item in current)
            {
                if (foundCurrent)
                {
                    next = item.SortOrder;
                    item.SortOrder = currentSort;
                    break;
                }

                if (item.Id == bvin)
                {
                    currentSort = item.SortOrder;
                    foundCurrent = true;
                }
            }

            if (next > 1)
            {
                var cur = FindSingleItem(bvin);
                if (cur != null)
                {
                    cur.SortOrder = next;
                }
            }

            return true;
        }
    }
}
