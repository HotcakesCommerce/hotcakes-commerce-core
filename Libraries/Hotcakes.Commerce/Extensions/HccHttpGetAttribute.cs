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
using System.Reflection;
using System.Web.Mvc;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Selects an MVC action for an HTTP GET or an HCC virtual GET form request.
    /// </summary>
    /// <remarks>
    /// For a virtual HCC form, the <c>hccrequesttype</c> route value must equal
    /// <c>hccget</c>. Otherwise selection delegates to MVC's standard
    /// <see cref="AcceptVerbsAttribute"/> GET behavior.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HccHttpGetAttribute : ActionMethodSelectorAttribute
    {
        // Fields
        private static readonly AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute(HttpVerbs.Get);

        // Methods
        /// <summary>
        /// Determines whether the attributed action is eligible for the current request.
        /// </summary>
        /// <param name="controllerContext">The current MVC controller context.</param>
        /// <param name="methodInfo">The action method being tested.</param>
        /// <returns>
        /// <see langword="true"/> for an HCC virtual GET request, or for a standard
        /// HTTP GET request when no virtual form request type is present.
        /// </returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var hccRequestType = (string) controllerContext.RequestContext.RouteData.Values["hccrequesttype"];
            if (Factory.CreateHccFormRenderer().VirtualFormUsed && !string.IsNullOrEmpty(hccRequestType))
            {
                return hccRequestType == "hccget";
            }
            return _innerAttribute.IsValidForRequest(controllerContext, methodInfo);
        }
    }
}
