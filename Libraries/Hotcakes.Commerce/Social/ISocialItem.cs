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

namespace Hotcakes.Commerce.Social
{
    /// <summary>
    /// Defines the content identity and JSON serialization contract for an item stored in a social journal.
    /// </summary>
    /// <remarks>
    /// Implementations provide the platform content-item identifier used to connect the social record to its
    /// content metadata, and provide a JSON representation for consumers that persist or transport the item.
    /// </remarks>
    public interface ISocialItem
    {
        /// <summary>
        /// Gets the platform content-item identifier associated with this social item.
        /// </summary>
        /// <value>The content-item identifier used by the social integration.</value>
        int ContentItemId { get; }

        /// <summary>
        /// Gets the JSON representation of this social item.
        /// </summary>
        /// <returns>A JSON string containing the serialized social item data.</returns>
        string ToJson();
    }
}
