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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSettingsRepository :
        HccLocalizationRepoBase<hcc_StoreSettings, hcc_StoreSettingsTranslations, StoreSetting, long>
    {
        #region constructor

        public StoreSettingsRepository(HccRequestContext c)
            : base(c)
        {
        }

        #endregion

        #region public methods

        public StoreSetting FindSingleSetting(long storeId, string settingName)
        {
            return FindFirstPoco(y => y.Item.StoreId == storeId && y.Item.SettingName == settingName);
        }

        public bool Update(StoreSetting item)
        {
            if (item.Id < 1)
            {
                return Create(item);
            }
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id, bool isLocalized = false)
        {
            return Delete(y => y.Id == id);
        }

        public List<StoreSetting> FindManySetting(List<long> storeIds)
        {
            return FindListPoco(q => q.Where(cb => storeIds.Contains(cb.Item.StoreId)));
        }

        public List<StoreSetting> FindForStore(long storeId)
        {
            return FindListPoco(q => { return q.Where(y => y.Item.StoreId == storeId); });
        }

        public void DeleteForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        public void MergeList(long storeId, List<StoreSetting> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.StoreId = storeId;
            }

            MergeList(subitems, ss => ss.StoreId == storeId);
        }

        #endregion

        #region LocalizationRepoBase Implementation

        protected override Expression<Func<hcc_StoreSettings, long>> ItemKeyExp
        {
            get { return s => s.Id; }
        }

        protected override Expression<Func<hcc_StoreSettingsTranslations, long>> ItemTranslationKeyExp
        {
            get { return st => st.StoreSettingsId; }
        }

        protected override Func<JoinedItem<hcc_StoreSettings, hcc_StoreSettingsTranslations>, bool> MatchItems(
            StoreSetting item)
        {
            return ss => ss.Item.Id == item.Id;
        }

        protected override Func<JoinedItem<hcc_StoreSettings, hcc_StoreSettingsTranslations>, bool> NotMatchItems(
            List<StoreSetting> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return ss => !itemIds.Contains(ss.Item.Id);
        }

        protected override void CopyItemToModel(hcc_StoreSettings data, StoreSetting model)
        {
            model.Id = data.Id;
            model.SettingValue = data.SettingValue;
            model.SettingName = data.SettingName;
            model.StoreId = data.StoreId;
        }

        protected override void CopyTransToModel(hcc_StoreSettingsTranslations data, StoreSetting model)
        {
            model.ContentCulture = data.Culture;
            model.LocalizedSettingValue = data.LocalizedSettingValue;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_StoreSettings, hcc_StoreSettingsTranslations> data,
            StoreSetting model)
        {
            data.Item.Id = model.Id;
            data.Item.SettingName = model.SettingName;
            data.Item.SettingValue = model.SettingValue;
            data.Item.StoreId = model.StoreId;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_StoreSettings, hcc_StoreSettingsTranslations> data,
            StoreSetting model)
        {
            data.ItemTranslation.StoreSettingsId = data.Item.Id;

            data.ItemTranslation.Culture = model.ContentCulture;
            data.ItemTranslation.LocalizedSettingValue = model.LocalizedSettingValue;
        }

        protected override bool HasTranslation(hcc_StoreSettingsTranslations trans)
        {
            return !string.IsNullOrEmpty(trans.LocalizedSettingValue);
        }

        #endregion
    }
}