#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Hotcakes.Commerce
{
    /// <summary>
    ///     Used throughout the application to read/write custom properties for an order. This is ideal for not only HCC
    ///     properties, but integrations by third parties to other systems, such as an ERP.
    /// </summary>
    [Serializable]
    public class CustomPropertyCollection : Collection<CustomProperty>
    {
        private const string HCC_KEY = "hcc";

        /// <summary>
        ///     If true, there is 1 reward point issued.
        /// </summary>
        internal bool HccRewardsPointsIssued
        {
            get { return GetProperty(HCC_KEY, "rewardspointsissued") == "1"; }
            set { SetProperty(HCC_KEY, "rewardspointsissued", value ? "1" : null); }
        }

        /// <summary>
        ///     Sets or gets the custom property using the specified key for HCC custom properties only.
        /// </summary>
        /// <param name="val">The value to read/write</param>
        /// <returns>CustomProperty</returns>
        public CustomProperty this[string val]
        {
            get
            {
                foreach (var value in Items)
                {
                    if (string.Compare(value.Key, val, true) == 0)
                    {
                        if (string.Compare(value.DeveloperId, HCC_KEY, true) == 0)
                        {
                            return value;
                        }
                    }
                }
                return null;
            }
            set
            {
                foreach (var item in Items)
                {
                    if (string.Compare(item.Key, val, true) == 0)
                    {
                        if (string.Compare(item.DeveloperId, HCC_KEY, true) == 0)
                        {
                            item.Value = value.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Sets or gets the custom property using the specified key for custom properties by any developer.
        /// </summary>
        /// <value>
        ///     The <see cref="CustomProperty" />.
        /// </value>
        /// <param name="val">The value to read/write</param>
        /// <param name="developerId">The developer identifier.</param>
        /// <returns>
        ///     CustomProperty
        /// </returns>
        public CustomProperty this[string val, string developerId]
        {
            get
            {
                foreach (var value in Items)
                {
                    if (string.Compare(value.Key, val, true) == 0)
                    {
                        if (string.Compare(value.DeveloperId, developerId, true) == 0)
                        {
                            return value;
                        }
                    }
                }
                return null;
            }
            set
            {
                foreach (var item in Items)
                {
                    if (string.Compare(item.Key, val, true) == 0)
                    {
                        if (string.Compare(item.DeveloperId, developerId, true) == 0)
                        {
                            item.Value = value.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new custom property using the specified values.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <param name="value">The value you wish to save for use later.</param>
        public void Add(string devId, string key, string value)
        {
            var item = new CustomProperty(devId, key, value);
            if (this[key, devId] == null)
            {
                Items.Add(item);
            }
            else
            {
                this[key, devId].Value = value;
            }
        }

        /// <summary>
        ///     Used to determine if a custom property already exists or not.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="propertyKey">The unique ID of the custom property used to look up the value.</param>
        /// <returns>Boolean - if true, a custom property was found matching both the developer ID and the property key.</returns>
        public bool Exists(string devId, string propertyKey)
        {
            var result = false;
            for (var i = 0; i <= Count - 1; i++)
            {
                if (this[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (this[i].Key.Trim().ToLower() == propertyKey.Trim().ToLower())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Adds or updates a custom property using the given values.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <param name="value">The value you wish to save for use later.</param>
        public void SetProperty(string devId, string key, string value)
        {
            var found = false;

            for (var i = 0; i <= Count - 1; i++)
            {
                if (this[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (this[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        this[i].Value = value;
                        found = true;
                        break;
                    }
                }
            }

            if (found == false)
            {
                Add(new CustomProperty(devId, key, value));
            }
        }

        /// <summary>
        ///     Adds or updates a custom property using the given values.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <param name="value">The value you wish to save for use later.</param>
        public void SetProperty(string devId, string key, int value)
        {
            SetProperty(devId, key, value.ToString());
        }

        /// <summary>
        ///     Queries for and returns the requested custom property value.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <returns>String - if found, a string version of the custom property value will be returned.</returns>
        public string GetProperty(string devId, string key)
        {
            var result = string.Empty;

            for (var i = 0; i <= Count - 1; i++)
            {
                if (this[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (this[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        result = this[i].Value;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Queries for and returns the requested custom property value.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <returns>Integer - if found, the integer version of the custom property value will be returned.</returns>
        public int GetPropertyAsInt(string devId, string key)
        {
            var result = 0;

            for (var i = 0; i <= Count - 1; i++)
            {
                if (this[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (this[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        result = this[i].GetValueAsInt();
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Permanently deletes the custom property from the store.
        /// </summary>
        /// <param name="devId">The developer ID that created and is managing the custom property.</param>
        /// <param name="key">The unique ID of the custom property used to delete the custom property from the store.</param>
        /// <returns>Boolean - if true, the custom property was found and successfully deleted.</returns>
        public bool Remove(string devId, string key)
        {
            var result = false;

            for (var i = 0; i <= Count - 1; i++)
            {
                if (this[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (this[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        Remove(this[i]);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Parses and returns the current custom property collection object as an XML serialized string.
        /// </summary>
        /// <returns>String - an XML representation of the current custom property collection object</returns>
        public string ToXml()
        {
            var result = string.Empty;

            try
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(GetType());
                xs.Serialize(sw, this);
                result = sw.ToString();
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        ///     Accepts an XML serialized version of a custom property collection and returns the deserialized version of the XML.
        /// </summary>
        /// <param name="data">The XML representation of a custom property collection.</param>
        /// <returns>A deserialized version of a custom property collection object.</returns>
        public static CustomPropertyCollection FromXml(string data)
        {
            var result = new CustomPropertyCollection();

            try
            {
                var tr = new StringReader(data);
                var xs = new XmlSerializer(result.GetType());
                result = (CustomPropertyCollection) xs.Deserialize(tr);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = new CustomPropertyCollection();
            }
            return result;
        }
    }
}