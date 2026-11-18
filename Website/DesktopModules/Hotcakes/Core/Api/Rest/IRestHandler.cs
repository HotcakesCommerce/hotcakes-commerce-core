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

using System.Collections.Specialized;
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>Handles REST API requests for IRestHandler.</summary>
    /// <remarks>Responses are serialized using the established REST API response contract.</remarks>
    public interface IRestHandler
    {
        /// <summary>Gets or sets the commerce application context used by this handler.</summary>
        HotcakesApplication HccApp { get; set; }

        /// <summary>Executes the REST API GetAction operation.</summary>
        /// <param name="parameters">The value supplied for this operation.</param>
        /// <param name="querystring">The value supplied for this operation.</param>
        /// <returns>The serialized REST API response.</returns>
        string GetAction(string parameters, NameValueCollection querystring);
        /// <summary>Executes the REST API PostAction operation.</summary>
        /// <param name="parameters">The value supplied for this operation.</param>
        /// <param name="querystring">The value supplied for this operation.</param>
        /// <param name="postdata">The value supplied for this operation.</param>
        /// <returns>The serialized REST API response.</returns>
        string PostAction(string parameters, NameValueCollection querystring, string postdata);
        /// <summary>Executes the REST API PutAction operation.</summary>
        /// <param name="parameters">The value supplied for this operation.</param>
        /// <param name="querystring">The value supplied for this operation.</param>
        /// <param name="postdata">The value supplied for this operation.</param>
        /// <returns>The serialized REST API response.</returns>
        string PutAction(string parameters, NameValueCollection querystring, string postdata);
        /// <summary>Executes the REST API DeleteAction operation.</summary>
        /// <param name="parameters">The value supplied for this operation.</param>
        /// <param name="querystring">The value supplied for this operation.</param>
        /// <param name="postdata">The value supplied for this operation.</param>
        /// <returns>The serialized REST API response.</returns>
        string DeleteAction(string parameters, NameValueCollection querystring, string postdata);
    }
}
