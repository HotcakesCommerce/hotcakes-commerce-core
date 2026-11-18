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
using System.Runtime.Serialization;
using Hotcakes.Web;

namespace Hotcakes.Shipping.Services
{
    /// <summary>
    /// Stores the ordered levels used by a table-based shipping-rate service.
    /// </summary>
    /// <remarks>
    /// Levels are serialized in the <c>Levels</c> setting as JSON and are returned in ascending
    /// order by <see cref="GetLevels"/>.
    /// </remarks>
    /// <seealso cref="ServiceSettings"/>
    /// <seealso cref="RateTableLevel"/>
    [Serializable]
    public class RateTableSettings : ServiceSettings
    {
        /// <summary>
        /// Initializes table settings from serialized dictionary data.
        /// </summary>
        /// <param name="info">The serialization data containing the setting entries.</param>
        /// <param name="context">The source and destination context for the serialization.</param>
        /// <seealso cref="ServiceSettings(SerializationInfo, StreamingContext)"/>
        public RateTableSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes an empty set of rate-table settings.
        /// </summary>
        public RateTableSettings()
        {
        }

        /// <summary>
        /// Gets all configured rate-table levels in ascending level order.
        /// </summary>
        /// <returns>A sorted list of configured <see cref="RateTableLevel"/> values.</returns>
        /// <seealso cref="AddLevel(RateTableLevel)"/>
        /// <seealso cref="RemoveLevel(RateTableLevel)"/>
        public List<RateTableLevel> GetLevels()
        {
            var result = LoadLevelsFromSettings();
            result.Sort();
            return result;
        }

        /// <summary>
        /// Adds a rate-table level or updates the rate of an existing level with the same level value.
        /// </summary>
        /// <param name="r">The level to add or update.</param>
        /// <returns><see langword="true"/> after the level collection is saved.</returns>
        /// <seealso cref="GetLevels"/>
        /// <seealso cref="RemoveLevel(RateTableLevel)"/>
        public bool AddLevel(RateTableLevel r)
        {
            var levels = GetLevels();

            var found = false;
            foreach (var sl in levels)
            {
                if (sl.Level == r.Level)
                {
                    sl.Rate = r.Rate;
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                levels.Add(r);
            }
            SaveLevelsToSettings(levels);
            return true;
        }

        /// <summary>
        /// Removes a matching rate-table level and saves the resulting collection.
        /// </summary>
        /// <param name="r">The level to remove.</param>
        /// <returns><see langword="true"/> when a matching level was removed; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="GetLevels"/>
        /// <seealso cref="AddLevel(RateTableLevel)"/>
        public bool RemoveLevel(RateTableLevel r)
        {
            var result = false;
            var levels = GetLevels();
            result = levels.Remove(r);
            SaveLevelsToSettings(levels);
            return result;
        }

        private List<RateTableLevel> LoadLevelsFromSettings()
        {
            var result = new List<RateTableLevel>();

            var json = GetSettingOrEmpty("Levels");
            if (string.IsNullOrEmpty(json)) return result;

            result = Json.ObjectFromJson<List<RateTableLevel>>(json);

            return result;
        }

        private void SaveLevelsToSettings(List<RateTableLevel> levels)
        {
            var json = levels.ObjectToJson();
            AddOrUpdate("Levels", json);
        }
    }
}
