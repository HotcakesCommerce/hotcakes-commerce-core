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
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using Hotcakes.Modules.Core.Settings;

namespace Hotcakes.Commerce.Dnn
{
    /// <summary>
    /// Reads and writes settings for a DNN module instance through <see cref="IModuleSettingsProvider"/>.
    /// </summary>
    [Serializable]
    public class DnnModuleSettingsProvider : IModuleSettingsProvider
    {
        /// <summary>
        /// Initializes a provider for a DNN module.
        /// </summary>
        /// <param name="moduleId">The DNN module identifier whose settings are managed.</param>
        public DnnModuleSettingsProvider(int moduleId)
        {
            _moduleId = moduleId;
        }

        protected Hashtable Settings
        {
            get
            {
                if (_settings == null)
                {
                    var moduleInfo = _moduleController.GetModule(_moduleId, Null.NullInteger, true);
                    _settings = moduleInfo.ModuleSettings;

                    if (_settings == null)
                    {
                        _settings = new Hashtable();
                    }
                }

                return _settings;
            }
        }

        /// <summary>
        /// Gets a module setting converted to the requested type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting is converted.</typeparam>
        /// <param name="key">The module setting key.</param>
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

        /// <summary>
        /// Stores a module setting and refreshes this provider's cached settings.
        /// </summary>
        /// <param name="key">The module setting key.</param>
        /// <param name="value">The value to store.</param>
        public void SetSettingValue(string key, object value)
        {
            _moduleController.UpdateModuleSetting(_moduleId, key, value.ToString());
            // Refresh settings
            _settings = null;
        }

        protected object GetSettingValue(string key)
        {
            if (Settings.ContainsKey(key))
            {
                return Settings[key];
            }
            return null;
        }

        #region Fields

        protected ModuleController _moduleController = new ModuleController();
        protected int _moduleId;
        private Hashtable _settings;

        #endregion
    }
}
