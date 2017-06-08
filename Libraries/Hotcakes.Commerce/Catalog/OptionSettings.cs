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
using Hotcakes.Web;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is a general class used to hold a collection of settings for an Option.
    /// </summary>
    [Serializable]
    public class OptionSettings : Dictionary<string, string>
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        /// <summary>
        ///     Allows you to save or update a setting for the current Option.
        /// </summary>
        /// <param name="name">The unique key or name of the Setting to update/save.</param>
        /// <param name="value">The value that should be returned when the setting is asked for by the key/name.</param>
        public void AddOrUpdate(string name, string value)
        {
            if (ContainsKey(name))
            {
                this[name] = value;
            }
            else
            {
                Add(name, value);
            }
        }

        /// <summary>
        ///     Returns the setting asked for if it exists. Otherwise an empty string is returned.
        /// </summary>
        /// <param name="name">The unique key or name used to retrieve the Setting.</param>
        /// <returns>Returns the setting or an empty string.</returns>
        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        /// <summary>
        ///     Returns the appropriate boolean value of a setting when it is saved as a 0 or 1.
        /// </summary>
        /// <param name="name">The unique key or name used to retrieve the Setting.</param>
        /// <returns>If the saved setting is a 1, True is returned. Otherwise, False is returned.</returns>
        public bool GetBoolSetting(string name)
        {
            if (ContainsKey(name))
            {
                if (this[name] == ONE)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Allows you to safely save a boolean value in the settings.
        /// </summary>
        /// <param name="name">The unique key or name of the Setting to update/save.</param>
        /// <param name="value">The value that should be returned when the setting is asked for by the key/name.</param>
        /// <remarks>This method converts the boolean value to the respective string representation of 0 or 1.</remarks>
        public void SetBoolSetting(string name, bool value)
        {
            if (value)
            {
                AddOrUpdate(name, ONE);
            }
            else
            {
                AddOrUpdate(name, ZERO);
            }
        }

        /// <summary>
        ///     Allows you to merge another setting of settings into the current settings.
        /// </summary>
        /// <param name="otherSettings">A populated instance of the other settings to add to the current settings.</param>
        public void Merge(OptionSettings otherSettings)
        {
            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }

        /// <summary>
        ///     Converts the settings to a JSON friendly string for use in the REST API.
        /// </summary>
        /// <returns>A JSON string representation of the settings.</returns>
        public string ToJson()
        {
            return Json.ObjectToJson(this);
        }

        /// <summary>
        ///     Converts the JSON version of the settings to an OptionSettings object.
        /// </summary>
        /// <param name="jsonValues">The JSON string to convert to an OptionSettings object.</param>
        /// <returns>Returns a populated instance of the OptionSettings object.</returns>
        public static OptionSettings FromJson(string jsonValues)
        {
            var result = Json.ObjectFromJson<OptionSettings>(jsonValues);
            if (result == null)
            {
                result = new OptionSettings();
            }
            return result;
        }
    }
}