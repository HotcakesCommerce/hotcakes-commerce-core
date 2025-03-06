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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1
{
    /// <summary>
    ///     Custom properties are used to store settings for objects that cannot be put in the user interface.
    /// </summary>
    [DataContract]
    [Serializable]
    public class CustomPropertyDTO
    {
        public CustomPropertyDTO()
        {
            DeveloperId = string.Empty;
            Key = string.Empty;
            Value = string.Empty;
        }

        /// <summary>
        ///     This is a unique ID that you use to group your own properties together.
        /// </summary>
        /// <remarks>Use any value that you wish here as long as it is unique to your company and you use it consistently.</remarks>
        [DataMember]
        public string DeveloperId { get; set; }

        /// <summary>
        ///     This is the unique name that is used to store and retrieve the custom property.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        ///     This is the information that you are saving to later retrieve.
        /// </summary>
        /// <remarks>You are not limited in the number of characters that you can use.</remarks>
        [DataMember]
        public string Value { get; set; }
    }
}