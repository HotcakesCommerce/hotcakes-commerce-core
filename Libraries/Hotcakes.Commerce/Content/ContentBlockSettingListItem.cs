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

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Represents one ordered item in a <see cref="ContentBlockSettingList"/>.
    /// </summary>
    /// <remarks>The ten setting slots retain their compatibility-sensitive serialized property names.</remarks>
    public class ContentBlockSettingListItem
    {
        /// <summary>
        /// Initializes an item with a new identifier, empty values, and sort position one.
        /// </summary>
        public ContentBlockSettingListItem()
        {
            Id = Guid.NewGuid().ToString();
            ListName = string.Empty;
            Setting1 = string.Empty;
            Setting2 = string.Empty;
            Setting3 = string.Empty;
            Setting4 = string.Empty;
            Setting5 = string.Empty;
            Setting6 = string.Empty;
            Setting7 = string.Empty;
            Setting8 = string.Empty;
            Setting9 = string.Empty;
            Setting10 = string.Empty;
            SortOrder = 1;
        }

        /// <summary>Gets or sets the item identifier.</summary>
        public string Id { get; set; }
        /// <summary>Gets or sets the name of the containing list.</summary>
        public string ListName { get; set; }
        /// <summary>Gets or sets the first caller-defined setting value.</summary>
        public string Setting1 { get; set; }
        /// <summary>Gets or sets the second caller-defined setting value.</summary>
        public string Setting2 { get; set; }
        /// <summary>Gets or sets the third caller-defined setting value.</summary>
        public string Setting3 { get; set; }
        /// <summary>Gets or sets the fourth caller-defined setting value.</summary>
        public string Setting4 { get; set; }
        /// <summary>Gets or sets the fifth caller-defined setting value.</summary>
        public string Setting5 { get; set; }
        /// <summary>Gets or sets the sixth caller-defined setting value.</summary>
        public string Setting6 { get; set; }
        /// <summary>Gets or sets the seventh caller-defined setting value.</summary>
        public string Setting7 { get; set; }
        /// <summary>Gets or sets the eighth caller-defined setting value.</summary>
        public string Setting8 { get; set; }
        /// <summary>Gets or sets the ninth caller-defined setting value.</summary>
        public string Setting9 { get; set; }
        /// <summary>Gets or sets the tenth caller-defined setting value.</summary>
        public string Setting10 { get; set; }
        /// <summary>Gets or sets the item's ordering position within its list.</summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Creates a copy of this item with a newly generated identifier.
        /// </summary>
        /// <returns>A copy of the item values and sort position.</returns>
        public ContentBlockSettingListItem Clone()
        {
            var clone = new ContentBlockSettingListItem();

            //clone.Id = Guid.NewGuid().ToString();
            clone.ListName = ListName;
            clone.Setting1 = Setting1;
            clone.Setting2 = Setting2;
            clone.Setting3 = Setting3;
            clone.Setting4 = Setting4;
            clone.Setting5 = Setting5;
            clone.Setting6 = Setting6;
            clone.Setting7 = Setting7;
            clone.Setting8 = Setting8;
            clone.Setting9 = Setting9;
            clone.Setting10 = Setting10;
            clone.SortOrder = SortOrder;

            return clone;
        }
    }
}
