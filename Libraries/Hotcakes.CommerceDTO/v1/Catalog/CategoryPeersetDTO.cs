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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     Get list of the categories details
    ///     grouped in different type like parents, peeers and children
    /// </summary>
    [DataContract]
    [Serializable]
    public class CategoryPeerSetDTO
    {
        /// <summary>
        ///     List of all children categories
        /// </summary>
        /// <remarks>
        ///     Private local variable
        /// </remarks>
        private List<CategorySnapshotDTO> _Children = new List<CategorySnapshotDTO>();

        /// <summary>
        ///     List of all parent categories
        /// </summary>
        /// <remarks>
        ///     private local variable
        /// </remarks>
        private List<CategorySnapshotDTO> _Parents = new List<CategorySnapshotDTO>();

        /// <summary>
        ///     List of all peers categories
        /// </summary>
        /// <remarks>
        ///     Private local variable
        /// </remarks>
        private List<CategorySnapshotDTO> _Peers = new List<CategorySnapshotDTO>();

        /// <summary>
        ///     List of parents category snapshot read from
        ///     database
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This variables used inside <c>CategoryPeerSet's</c> <c> ToDo </c> method
        ///     </para>
        /// </remarks>
        [DataMember]
        public List<CategorySnapshotDTO> Parents
        {
            get { return _Parents; }
            set { _Parents = value; }
        }

        /// <summary>
        ///     List of peers category snapshot read from
        ///     database
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This variables used inside <c>CategoryPeerSet's</c>  <c>ToDo</c> method
        ///     </para>
        /// </remarks>
        [DataMember]
        public List<CategorySnapshotDTO> Peers
        {
            get { return _Peers; }
            set { _Peers = value; }
        }

        /// <summary>
        ///     List of children category snapshot read from
        ///     database
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This variables used inside <c>CategoryPeerSet's</c>  <c>ToDo</c> method
        ///     </para>
        /// </remarks>
        [DataMember]
        public List<CategorySnapshotDTO> Children
        {
            get { return _Children; }
            set { _Children = value; }
        }
    }
}