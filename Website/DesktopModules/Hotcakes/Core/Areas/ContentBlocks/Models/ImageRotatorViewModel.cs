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

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Models
{
    [Serializable]
    public class ImageRotatorImageViewModel
    {
        public ImageRotatorImageViewModel()
        {
            ImageUrl = string.Empty; // 1            
            Url = string.Empty; // 2
            NewWindow = false; // 3
            Caption = string.Empty; // 4
        }

        public string ImageUrl { get; set; }
        public string Caption { get; set; }
        public string Url { get; set; }
        public bool NewWindow { get; set; }
    }

    [Serializable]
    public class ImageRotatorViewModel
    {
        public ImageRotatorViewModel()
        {
            Pause = 3;
            Images = new List<ImageRotatorImageViewModel>();
            CssClass = string.Empty;
            CssId = string.Empty;
            Height = 0;
            Width = 0;
        }

        public List<ImageRotatorImageViewModel> Images { get; set; }
        public string CssId { get; set; }
        public string CssClass { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Pause { get; set; }
    }
}