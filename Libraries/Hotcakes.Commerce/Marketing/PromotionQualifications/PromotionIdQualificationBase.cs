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
using System.Linq;
    
namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Provides normalized comma-separated identifier settings for ID-based qualifications.</summary>
    /// <remarks>The configured setting name is supplied by <see cref="IdSettingName"/>.</remarks>
    public abstract class PromotionIdQualificationBase : PromotionQualificationBase
    {
        #region Properties

        /// <summary>Gets the configured identifiers, trimmed and normalized to lowercase.</summary>
        /// <returns>The configured identifiers, or an empty list.</returns>
        public List<string> CurrentIds()
        {
            var all = GetSetting(IdSettingName) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(all)) return new List<string>();

            return all
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => s.Length > 0)
                .ToList();
        }

        #endregion

        #region Helper Methods

        private void SaveIdsToSettings(List<string> typeIds)
        {
            var all = (typeIds == null || typeIds.Count == 0) ? string.Empty : string.Join(",", typeIds);
            SetSetting(IdSettingName, all);
        }

        #endregion

        #region Abstract

        /// <summary>Gets the settings key containing this qualification's identifiers.</summary>
        public abstract string IdSettingName { get; }

        /// <summary>Initializes derived qualification defaults before identifiers are added.</summary>
        protected virtual void OnInit()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>Adds a non-empty identifier after trimming and lowercasing it.</summary>
        /// <param name="typeId">The identifier to add.</param>
        public void AddNewId(string typeId)
        {
            if (string.IsNullOrWhiteSpace(typeId)) return;

            var ids = CurrentIds();
            var possible = typeId.Trim().ToLowerInvariant();
            if (ids.Contains(possible)) return;

            ids.Add(possible);
            SaveIdsToSettings(ids);
        }

        /// <summary>Removes the supplied identifier from the configured list.</summary>
        /// <param name="typeId">The identifier to remove.</param>
        public void RemoveId(string typeId)
        {
            var _Ids = CurrentIds();
            if (_Ids.Contains(typeId))
            {
                _Ids.Remove(typeId);
                SaveIdsToSettings(_Ids);
            }
        }

        #endregion

        #region Init

        /// <summary>Initializes an empty ID-based qualification.</summary>
        public PromotionIdQualificationBase() : this(string.Empty)
        {
        }

        /// <summary>Initializes the qualification and optionally adds an identifier.</summary>
        /// <param name="typeId">The initial identifier; blank values are ignored.</param>
        public PromotionIdQualificationBase(string typeId)
        {
            OnInit();
            AddNewId(typeId);
        }

        #endregion
    }
}