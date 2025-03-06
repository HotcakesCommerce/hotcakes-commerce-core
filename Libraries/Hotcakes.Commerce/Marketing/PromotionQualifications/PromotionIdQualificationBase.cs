#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public abstract class PromotionIdQualificationBase : PromotionQualificationBase
    {
        #region Properties

        public List<string> CurrentIds()
        {
            var result = new List<string>();
            var all = GetSetting(IdSettingName);
            result = all.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

            return result;
        }

        #endregion

        #region Helper Methods

        private void SaveIdsToSettings(List<string> typeIds)
        {
            var all = string.Join(",", typeIds);
            SetSetting(IdSettingName, all);
        }

        #endregion

        #region Abstract

        public abstract string IdSettingName { get; }

        protected virtual void OnInit()
        {
        }

        #endregion

        #region Public Methods

        public void AddNewId(string typeId)
        {
            var _Ids = CurrentIds();

            var possible = typeId.Trim().ToLowerInvariant();
            if (possible == string.Empty) return;
            if (_Ids.Contains(possible)) return;
            _Ids.Add(possible);
            SaveIdsToSettings(_Ids);
        }

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

        public PromotionIdQualificationBase() : this(string.Empty)
        {
        }

        public PromotionIdQualificationBase(string typeId)
        {
            OnInit();
            AddNewId(typeId);
        }

        #endregion
    }
}