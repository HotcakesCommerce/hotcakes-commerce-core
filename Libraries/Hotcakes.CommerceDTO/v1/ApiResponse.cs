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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1
{
    /// <summary>
    /// Represents the standard API response envelope for content and any returned <see cref="ApiError"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    [DataContract]
    [Serializable]
    public class ApiResponse<T> : IApiResponse
    {
        /// <summary>
        /// Initializes an API response with an empty <see cref="Errors"/> collection.
        /// </summary>
        public ApiResponse()
        {
            Errors = new List<ApiError>();
        }

        /// <summary>
        /// Gets or sets the content returned by the API operation.
        /// </summary>
        [DataMember]
        public T Content { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ApiError"/> instances returned by the API operation.
        /// </summary>
        /// <remarks>The collection is initialized by the default constructor and may be empty when the operation succeeds.</remarks>
        [DataMember]
        public List<ApiError> Errors { get; set; }
    }
}
