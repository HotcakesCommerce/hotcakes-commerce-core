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

using System.Collections.Generic;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Category peers information model.
    ///     This is used in the category viewer to
    ///     show the details related to category.
    /// </summary>
    public class CategoryPeerSet
    {
        private List<CategorySnapshot> _Children = new List<CategorySnapshot>();

        private List<CategorySnapshot> _Parents = new List<CategorySnapshot>();


        private List<CategorySnapshot> _Peers = new List<CategorySnapshot>();

        /// <summary>
        ///     List of <see cref="Catalog.CategorySnapshot" /> as parent categories of category on category viewer page.
        /// </summary>
        public List<CategorySnapshot> Parents
        {
            get { return _Parents; }
            set { _Parents = value; }
        }

        /// <summary>
        ///     List of <see cref="Catalog.CategorySnapshot" /> as peers of category on category viewer page.
        /// </summary>
        public List<CategorySnapshot> Peers
        {
            get { return _Peers; }
            set { _Peers = value; }
        }

        /// <summary>
        ///     List of <see cref="Catalog.CategorySnapshot" /> as childerns of category on category viewer page.
        /// </summary>
        public List<CategorySnapshot> Children
        {
            get { return _Children; }
            set { _Children = value; }
        }

        /// <summary>
        ///     Convert model class to database objcets.
        /// </summary>
        /// <returns></returns>
        public CategoryPeerSetDTO ToDto()
        {
            var result = new CategoryPeerSetDTO();

            foreach (var c in _Children)
            {
                result.Children.Add(c.ToDto());
            }
            foreach (var c2 in _Parents)
            {
                result.Parents.Add(c2.ToDto());
            }
            foreach (var c3 in _Peers)
            {
                result.Peers.Add(c3.ToDto());
            }

            return result;
        }
    }
}