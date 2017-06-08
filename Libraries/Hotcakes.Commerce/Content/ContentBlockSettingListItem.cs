#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    public class ContentBlockSettingListItem
    {
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

        public string Id { get; set; }
        public string ListName { get; set; }
        public string Setting1 { get; set; }
        public string Setting2 { get; set; }
        public string Setting3 { get; set; }
        public string Setting4 { get; set; }
        public string Setting5 { get; set; }
        public string Setting6 { get; set; }
        public string Setting7 { get; set; }
        public string Setting8 { get; set; }
        public string Setting9 { get; set; }
        public string Setting10 { get; set; }
        public int SortOrder { get; set; }

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