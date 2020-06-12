#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
    [Serializable]
    public class CultureSwitch : IDisposable
    {
        private readonly CultureInfo oldCulture;
        private readonly CultureInfo oldUICulture;

        public CultureSwitch(Store store)
        {
            oldCulture = Thread.CurrentThread.CurrentCulture;
            oldUICulture = Thread.CurrentThread.CurrentUICulture;

            SetCulture(store);
        }

        public static void SetCulture(Store store)
        {
            SetCulture(store, PortalSettings.Current);
        }

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

        public void RollbackCulture()
        {
            Thread.CurrentThread.CurrentCulture = oldCulture;
            Thread.CurrentThread.CurrentUICulture = oldUICulture;
        }

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CultureSwitch()
        {
            Dispose(false);
        }

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