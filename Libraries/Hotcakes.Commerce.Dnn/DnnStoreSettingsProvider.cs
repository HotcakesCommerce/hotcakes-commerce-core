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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Services.Localization;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnStoreSettingsProvider : IStoreSettingsProvider
    {
        public T GetSettingValue<T>(string key, T defVal)
        {
            var val = GetSettingValue(key);

            if (val == null)
                return defVal;

            var res = defVal;
            try
            {
                res = (T) Convert.ChangeType(val, typeof (T));
            }
            catch
            {
                res = defVal;
            }

            return res;
        }


        public List<HccLocale> GetLocales()
        {
            var dic = DnnGlobal.Instance.GetLocales();
            return dic.Values.Select(v => ConvertLocale(v)).ToList();
        }

        public HccLocale GetLocale(string code)
        {
            var locale = DnnGlobal.Instance.GetLocale(code);
            return ConvertLocale(locale);
        }

        public CultureInfo GetPageLocale()
        {
            return DnnGlobal.Instance.GetPageLocale();
        }

        public string GetDefaultLocale()
        {
            var portalSettings = DnnGlobal.Instance.GetCurrentPortalSettings();
            if (!string.IsNullOrEmpty(portalSettings.DefaultLanguage))
            {
                return portalSettings.DefaultLanguage;
            }
            return "en-US";
        }

        protected object GetSettingValue(string key)
        {
            return HostController.Instance.GetString(key);
        }

        protected HccLocale ConvertLocale(Locale locale)
        {
            if (locale == null)
                return null;
            var systemLocale = Localization.SystemLocale;
            return new HccLocale
            {
                Code = locale.Code,
                Fallback = string.IsNullOrWhiteSpace(locale.Fallback) ? systemLocale : locale.Fallback,
                EnglishName = locale.EnglishName,
                NativeName = locale.NativeName,
                Text = locale.Text
            };
        }
    }
}