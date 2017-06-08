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

using System;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Extensions
{
    [Obsolete(
        "Obsolete in 2.0.0. This class functionality was replaced with HccRequestContextUtils class and HotcakesApplication.Current static property"
        )]
    public class HccAppHelper
    {
        [Obsolete(
            "Obsolete in 2.0.0. HotcakesApplication doesn't require preservation any longer. Only HccRequestContext.Current have to be initialized properly"
            )] public static bool IsForMemoryOnly = false;

        /// <summary>
        ///     Initializes the HCC application. It should NEVER be stored in static variable.
        /// </summary>
        /// <param name="forceRecreate">if set to <c>true</c> [force recreate].</param>
        /// <param name="overideContentCulture">if set to <c>true</c> [overides content culture].</param>
        /// <returns></returns>
        [Obsolete(
            "Obsolete in 2.0.0. HotcakesApplication doesn't require such complex initialization. Only HccRequestContext.Current have to be initialized properly"
            )]
        public static HotcakesApplication InitHccApp(bool forceRecreate = false, bool overideContentCulture = false)
        {
            var hccContext = HccRequestContext.Current;
            if (overideContentCulture)
                UpdateAdminContentCulture(hccContext);

            var hccApp = new HotcakesApplication(hccContext);
            return hccApp;
        }

        /// <summary>
        ///     Initializes the empty HCC application. It should NEVER be stored in object property or static variable.
        /// </summary>
        /// <returns></returns>
        [Obsolete(
            "Obsolete in 2.0.0. HotcakesApplication doesn't require such complex initialization. Only HccRequestContext.Current have to be initialized properly"
            )]
        public static HotcakesApplication InitEmptyHccApp()
        {
            var context = new HccRequestContext();
            return new HotcakesApplication(context);
        }

        [Obsolete(
            "Obsolete in 2.0.0. HotcakesApplication doesn't require such complex initialization. Only HccRequestContext.Current have to be initialized properly. "
            )]
        public static HotcakesApplication InitLocalizedEmptyHccApp(string cultureCode)
        {
            var context = new HccRequestContext(cultureCode);
            return new HotcakesApplication(context);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of HccRequestContextUtils class")]
        public static HccLocale GetAdminContentCulture()
        {
            return HccRequestContextUtils.GetAdminContentCulture();
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of HccRequestContextUtils class")]
        public static void UpdateAdminContentCulture(HccRequestContext requestContext)
        {
            UpdateAdminContentCulture(requestContext);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of HccRequestContextUtils class")]
        public static void UpdateUserContentCulture(HccRequestContext requestContext)
        {
            UpdateUserContentCulture(requestContext);
        }
    }
}