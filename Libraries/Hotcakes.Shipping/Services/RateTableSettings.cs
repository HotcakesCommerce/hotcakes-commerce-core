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
using System.Runtime.Serialization;
using Hotcakes.Web;

namespace Hotcakes.Shipping.Services
{
    [Serializable]
    public class RateTableSettings : ServiceSettings
    {
        public RateTableSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public RateTableSettings()
        {
        }

        public List<RateTableLevel> GetLevels()
        {
            var result = LoadLevelsFromSettings();
            result.Sort();
            return result;
        }

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