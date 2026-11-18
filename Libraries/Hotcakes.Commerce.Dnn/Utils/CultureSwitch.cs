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
using System.Globalization;
using System.Threading;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Dnn.Utils
{
    /// <summary>
    /// Temporarily applies a store's culture settings to the current thread and restores the previous cultures when disposed.
    /// </summary>
    [Serializable]
    public class CultureSwitch : IDisposable
    {
        private readonly CultureInfo oldCulture;
        private readonly CultureInfo oldUICulture;

        /// <summary>
        /// Initializes a culture scope for the specified <see cref="Store"/>.
        /// </summary>
        /// <param name="store">The store whose culture and currency settings should be applied.</param>
        public CultureSwitch(Store store)
        {
            oldCulture = Thread.CurrentThread.CurrentCulture;
            oldUICulture = Thread.CurrentThread.CurrentUICulture;

            SetCulture(store);
        }

        /// <summary>
        /// Applies the specified store's culture settings using the current DNN portal settings.
        /// </summary>
        /// <param name="store">The store whose culture and currency settings should be applied.</param>
        public static void SetCulture(Store store)
        {
            SetCulture(store, PortalSettings.Current);
        }

        /// <summary>
        /// Applies the specified store's culture settings using the supplied DNN portal settings.
        /// </summary>
        /// <param name="store">The store whose culture and currency settings should be applied.</param>
        /// <param name="portalSettings">The portal settings used to resolve the portal culture.</param>
        public static void SetCulture(Store store, PortalSettings portalSettings)
        {
            // Get culture settings from DNN
            var portalCulture = Localization.GetPageLocale(portalSettings);
            if (!string.IsNullOrEmpty(store.Settings.CurrencyCultureCode))
            {
                var currencyCulture = new CultureInfo(store.Settings.CurrencyCultureCode);
                portalCulture.NumberFormat = currencyCulture.NumberFormat;
            }

            Thread.CurrentThread.CurrentCulture = portalCulture;
            Thread.CurrentThread.CurrentUICulture = portalCulture;
        }

        /// <summary>
        /// Restores the cultures that were active before this scope was created.
        /// </summary>
        public void RollbackCulture()
        {
            Thread.CurrentThread.CurrentCulture = oldCulture;
            Thread.CurrentThread.CurrentUICulture = oldUICulture;
        }

        #region IDisposable

        private bool disposed;

        /// <summary>
        /// Restores the original cultures and releases this culture scope.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CultureSwitch()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the resources owned by this culture scope.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> when called explicitly; <see langword="false"/> when called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    RollbackCulture();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
