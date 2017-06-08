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

using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This enumeration is used to give a meaningful value to product reviews.
    /// </summary>
    [DataContract]
    public enum ProductReviewRatingDTO
    {
        /// <summary>
        ///     Zero stars is effectively a non-rating
        /// </summary>
        [EnumMember] ZeroStars = 0,

        /// <summary>
        ///     One star is the lowest rating a customer can give to a product.
        /// </summary>
        [EnumMember] OneStar = 1,

        /// <summary>
        ///     Two stars means that the product was not horrible, but it wasn't good either.
        /// </summary>
        [EnumMember] TwoStars = 2,

        /// <summary>
        ///     Three stars reflects that the product was average. It was no better or worse than a similar product.
        /// </summary>
        [EnumMember] ThreeStars = 3,

        /// <summary>
        ///     Four starts is a great rating, meaning that the product was nearly perfect.
        /// </summary>
        [EnumMember] FourStars = 4,

        /// <summary>
        ///     Five stars are generally only awarded by customers when the product meets 100% of their expectations, including the
        ///     process to get the product.
        /// </summary>
        [EnumMember] FiveStars = 5
    }
}