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
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;

namespace Hotcakes.Commerce.Dnn
{
    /// <summary>
    /// Reads host-level configuration values from DNN through <see cref="IHostSettingsProvider"/>.
    /// </summary>
    [Serializable]
    public class DnnHostSettingsProvider : IHostSettingsProvider
    {
        /// <summary>
        /// Gets the file extensions allowed by the DNN host configuration.
        /// </summary>
        /// <returns>The host's allowed extension whitelist.</returns>
        public IEnumerable<string> GetAllowedExtensions()
        {
            return Host.AllowedExtensionWhitelist.AllowedExtensions;
        }

        /// <summary>
        /// Gets whether DNN host debug mode is enabled.
        /// </summary>
        /// <returns><see langword="true"/> when host debug mode is enabled; otherwise, <see langword="false"/>.</returns>
        public bool DebugModeEnabled()
        {
            return Host.DebugMode;
        }

        /// <summary>
        /// Gets a host setting converted to the requested type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting is converted.</typeparam>
        /// <param name="key">The host setting key.</param>
        /// <param name="defVal">The value returned when the setting is missing or cannot be converted.</param>
        /// <returns>The converted setting value, or <paramref name="defVal"/> when unavailable or invalid.</returns>
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

        protected object GetSettingValue(string key)
        {
            return HostController.Instance.GetString(key);
        }
    }
}
