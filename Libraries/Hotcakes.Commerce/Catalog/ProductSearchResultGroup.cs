#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Collections.ObjectModel;

namespace Hotcakes.Commerce.Catalog
{
    [Serializable]
    public class ProductSearchResultGroup
    {
        private string _GroupName = string.Empty;
        private string _InfoMessage = string.Empty;
        private Collection<Product> _Products = new Collection<Product>();

        public ProductSearchResultGroup()
        {
        }

        public ProductSearchResultGroup(string name, Collection<Product> productResults)
        {
            _GroupName = name;
            _Products = productResults;
        }

        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; }
        }

        public Collection<Product> Products
        {
            get { return _Products; }
            set { _Products = value; }
        }

        public string InfoMessage
        {
            get { return _InfoMessage; }
            set { _InfoMessage = value; }
        }
    }
}