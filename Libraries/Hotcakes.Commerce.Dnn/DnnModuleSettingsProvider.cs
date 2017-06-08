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
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using Hotcakes.Modules.Core.Settings;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnModuleSettingsProvider : IModuleSettingsProvider
    {
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