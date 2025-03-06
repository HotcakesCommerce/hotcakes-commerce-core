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
using System.Threading;
using System.Web;
using System.Web.Routing;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce
{
    public class HccRequestContext
    {
        private const string DEFAULT_CULTURE_CODE = "en-US";
        private const string CONTEXT_KEY = "HccRequestContext";

        /// <summary>
        ///     Field that stores HccRequestContext when HttpContext.Current is null
        /// </summary>
        [ThreadStatic] private static HccRequestContext _current;

        public HccRequestContext(string cultureCode = "")
        {
            var httpContext = Factory.HttpContext;
            if (httpContext != null && httpContext.Request != null)
            {
                RoutingContext = httpContext.Request.RequestContext;
            }

            CurrentStore = new Store();
            IntegrationEvents = new Integration();

            var currentCulture = DEFAULT_CULTURE_CODE;
            if (!string.IsNullOrEmpty(cultureCode))
            {
                currentCulture = cultureCode;
            }

            // Give higher precedence to thread culture
            var threadCulture = Thread.CurrentThread.CurrentCulture.Name;
            if (string.IsNullOrEmpty(cultureCode) && !string.IsNullOrEmpty(threadCulture))
            {
                currentCulture = threadCulture;
            }

            MainContentCulture = currentCulture;
            if (MainContentCulture != DEFAULT_CULTURE_CODE)
            {
                FallbackContentCulture = DEFAULT_CULTURE_CODE;
            }
        }

        /// <summary>
        ///     Gets or sets the current HccRequestContext.
        /// </summary>
        /// <value>
        ///     The current HccRequestContext object.
        /// </value>
        public static HccRequestContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    _current = new HccRequestContext();
                    return _current;
                }
                return (HccRequestContext) HttpContext.Current.Items[CONTEXT_KEY];
            }
            set
            {
                if (HttpContext.Current == null)
                    _current = value;
                else
                    HttpContext.Current.Items[CONTEXT_KEY] = value;
            }
        }

        /// <summary>
        ///     Store that is currently being loaded and used.
        /// </summary>
        /// <value>
        ///     A Store object. By default, this will be a "new" empty store object, to be populated later in other
        ///     methods/classes.
        /// </value>
        public Store CurrentStore { get; set; }

        /// <summary>
        ///     User that is visiting the store at this specific moment.
        /// </summary>
        /// <value>
        ///     A CustomerAccount object. By default, this will be a NULL object, to be populated later in other methods/classes.
        /// </value>
        public CustomerAccount CurrentAccount { get; set; }

        /// <summary>
        ///     Gets or sets culture of item translations that will be retrived from the database.
        ///     This culture is not used for retrieving resources from resource files. Thread UI culture is used for that in that
        ///     case.
        ///     Separation of those concepts is an intentional step since it makes sense to distinguish Content culture and UI
        ///     culture on admin.
        ///     This can provide admins with ability to have UI in their native languge while copy/pasting content translations
        ///     provided by translator.
        /// </summary>
        /// <value>
        ///     The main content culture.
        /// </value>
        public string MainContentCulture { get; set; }

        /// <summary>
        ///     Gets or sets culture of item translations that will be retrived from the database.
        ///     This property specifies what culture have to be used if items has no translations in MainContentCulture.
        ///     This culture is not used for retrieving resources from resource files. Thread UI culture is used for that in that
        ///     case.
        ///     Separation of those concepts is an intentional step since it makes sense to distinguish Content culture and UI
        ///     culture on admin.
        ///     This can provide admins with ability to have UI in their native languge while copy/pasting content translations
        ///     provided by translator.
        /// </summary>
        /// <value>
        ///     The fallback content culture.
        /// </value>
        public string FallbackContentCulture { get; set; }

        //TODO: Maybe make sense to revert later and write initialization somewhere....
        //public Orders.Order CurrentCart { get; set; }

        public Integration IntegrationEvents { get; set; }

        // We need avoid using of this property
        public RequestContext RoutingContext { get; set; }
    }
}