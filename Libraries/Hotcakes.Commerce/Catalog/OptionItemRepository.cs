#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Peform datbase operation on hcc_ProductOptionsItems and hcc_ProductOptionItemTranslation tables.
    /// </summary>
    internal class OptionItemRepository :
        HccLocalizationRepoBase<hcc_ProductOptionsItems, hcc_ProductOptionItemTranslation, OptionItem, Guid>
    {
        public OptionItemRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Expression<Func<hcc_ProductOptionsItems, Guid>> ItemKeyExp
        {
            get { return poi => poi.bvin; }
        }

        protected override Expression<Func<hcc_ProductOptionItemTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return poit => poit.ProductOptionItemId; }
        }

        protected override Func<JoinedItem<hcc_ProductOptionsItems, hcc_ProductOptionItemTranslation>, bool> MatchItems(
            OptionItem item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return poij => poij.Item.bvin == guid;
        }

        protected override Func<JoinedItem<hcc_ProductOptionsItems, hcc_ProductOptionItemTranslation>, bool>
            NotMatchItems(List<OptionItem> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return poi => !itemGuids.Contains(poi.Item.bvin);
        }

        /// <summary>
        ///     Copy database object model to view model.
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductOptionsItems" /> instance</param>
        /// <param name="model"><see cref="OptionItem" /> instance</param>
        protected override void CopyItemToModel(hcc_ProductOptionsItems data, OptionItem model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.IsLabel = data.IsLabel;
            model.OptionBvin = DataTypeHelper.GuidToBvin(data.OptionBvin);
            model.PriceAdjustment = data.PriceAdjustment;
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
            model.WeightAdjustment = data.WeightAdjustment;
            model.IsDefault = data.IsDefault;
        }

        /// <summary>
        ///     Copy name from database object model to view model.
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductOptionsItems" /> instance</param>
        /// <param name="model"><see cref="OptionItem" /> instance</param>
        protected override void CopyTransToModel(hcc_ProductOptionItemTranslation data, OptionItem model)
        {
            model.Name = data.Name;
        }

        /// <summary>
        ///     Copy data from view model to database object model.
        /// </summary>
        /// <param name="data">Joined database object model</param>
        /// <param name="model"><see cref="OptionItem" /> instance</param>
        protected override void CopyModelToItem(
            JoinedItem<hcc_ProductOptionsItems, hcc_ProductOptionItemTranslation> data, OptionItem model)
        {
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.IsLabel = model.IsLabel;
            data.Item.OptionBvin = DataTypeHelper.BvinToGuid(model.OptionBvin);
            data.Item.PriceAdjustment = model.PriceAdjustment;
            data.Item.SortOrder = model.SortOrder;
            data.Item.StoreId = model.StoreId;
            data.Item.WeightAdjustment = model.WeightAdjustment;
            data.Item.IsDefault = model.IsDefault;
        }

        /// <summary>
        ///     Copy name from view model to database object model.
        /// </summary>
        /// <param name="data">Joined database object model</param>
        /// <param name="model"><see cref="OptionItem" /> instance</param>
        protected override void CopyModelToTrans(
            JoinedItem<hcc_ProductOptionsItems, hcc_ProductOptionItemTranslation> data, OptionItem model)
        {
            data.ItemTranslation.ProductOptionItemId = data.Item.bvin;

            data.ItemTranslation.Name = model.Name;
        }

        /// <summary>
        ///     Find optionitem by unique identifier
        /// </summary>
        /// <param name="bvin">OptionItem unique identifier</param>
        /// <returns>Returns <see cref="OptionItem" /> instance</returns>
        public OptionItem Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        /// <summary>
        ///     Create optionitem.
        /// </summary>
        /// <param name="item"><see cref="OptionItem" /> instance</param>
        /// <returns>Returns true if new optionitem created otherwise returns false.</returns>
        public override bool Create(OptionItem item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;

            if (item.IsDefault)
            {
                ClearOtherDefaultItems(item);
            }

            return base.Create(item);
        }

        /// <summary>
        ///     Update optionitem
        /// </summary>
        /// <param name="item"><see cref="OptionItem" /> reference</param>
        /// <returns>Returns true if optionitem updated successfully.</returns>
        public bool Update(OptionItem item)
        {
            if (item.IsDefault)
            {
                ClearOtherDefaultItems(item);
            }
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return base.Update(item, y => y.bvin == guid);
        }

        /// <summary>
        ///     Clear isdefault for all other optionitem than the current one.
        /// </summary>
        /// <param name="item"><see cref="OptionItem" /> instance</param>
        private void ClearOtherDefaultItems(OptionItem item)
        {
            var items = FindForOption(item.OptionBvin).Where(i => i.IsDefault && i.Bvin != item.Bvin);
            foreach (var oi in items)
            {
                oi.IsDefault = false;
                var guid = DataTypeHelper.BvinToGuid(oi.Bvin);
                base.Update(oi, y => y.bvin == guid);
            }
        }

        /// <summary>
        ///     Delete optionitem
        /// </summary>
        /// <param name="bvin">OptionItem unique identifier</param>
        /// <returns></returns>
        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        /// <summary>
        ///     Find list of option items by given option id
        /// </summary>
        /// <param name="optionId">option unique identifier</param>
        /// <returns></returns>
        public List<OptionItem> FindForOption(string optionId)
        {
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.OptionBvin == optionGuid)
                    .OrderBy(y => y.Item.SortOrder);
            });
        }

        /// <summary>
        ///     Find list of optionitems for given option ids
        /// </summary>
        /// <param name="optionIds">List of Options ids</param>
        /// <returns>Returns list of <see cref="OptionItem" /> instances</returns>
        public List<OptionItem> FindForManyOption(List<string> optionIds)
        {
            var optionGuids = optionIds.Select(o => DataTypeHelper.BvinToGuid(o)).ToList();
            return FindListPoco(q =>
            {
                return q
                    .Where(oi => optionGuids.Contains(oi.Item.OptionBvin))
                    .OrderBy(oi => oi.Item.SortOrder);
            });
        }

        /// <summary>
        ///     Delete all matching optionitems
        /// </summary>
        /// <param name="optionBvin">Option unique identifier</param>
        public void DeleteForOption(string optionBvin)
        {
            var existing = FindForOption(optionBvin);
            foreach (var sub in existing)
            {
                Delete(sub.Bvin);
            }
        }

        /// <summary>
        ///     Resort the options items for given option.
        /// </summary>
        /// <param name="optionBvin">Option unique identifier</param>
        /// <param name="sortedIds">List of sort ids</param>
        /// <returns>Returns true if the resort done successfully</returns>
        public bool Resort(string optionBvin, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForOptionItem(optionBvin, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        /// <summary>
        ///     Update the sort order for given option item in the option.
        /// </summary>
        /// <param name="optionBvin">Option unique identifier</param>
        /// <param name="optionItemBvin">Option item unique identifier</param>
        /// <param name="newSortOrder">New sorting order</param>
        /// <returns></returns>
        private bool UpdateSortOrderForOptionItem(string optionBvin, string optionItemBvin, int newSortOrder)
        {
            var item = Find(optionItemBvin);
            if (item == null) return false;
            if (item.OptionBvin != optionBvin) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        /// <summary>
        ///     Add given list of option items to the given option
        /// </summary>
        /// <param name="optionBvin">Option unique identifier</param>
        /// <param name="subitems">List of <see cref="OptionItem" /> to be added to the option</param>
        public void MergeList(string optionBvin, List<OptionItem> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OptionBvin = optionBvin;
                item.StoreId = Context.CurrentStore.Id;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var optionGuid = DataTypeHelper.BvinToGuid(optionBvin);
            MergeList(subitems, poi => poi.OptionBvin == optionGuid);
        }

        /// <summary>
        ///     Clone option items from one option to another option
        /// </summary>
        /// <param name="productOptionId">Source product option unique identifier</param>
        /// <param name="newProductOptionId">Destination product option unique identifier</param>
        public void Clone(string productOptionId, string newProductOptionId)
        {
            var productOptionGuid = DataTypeHelper.BvinToGuid(productOptionId);
            var newProductOptionGuid = DataTypeHelper.BvinToGuid(newProductOptionId);
            using (var s = CreateStrategy())
            {
                var productOptionItems = s.GetQuery()
                    .Where(ppv => ppv.OptionBvin == productOptionGuid)
                    .ToList();

                foreach (var productOptionItem in productOptionItems)
                {
                    s.Detach(productOptionItem);

                    var oldProductOptionKey = productOptionItem.bvin;
                    productOptionItem.bvin = Guid.NewGuid();
                    productOptionItem.OptionBvin = newProductOptionGuid;

                    s.Add(productOptionItem);

                    var productOptionTranslations = s.GetQuery<hcc_ProductOptionItemTranslation>()
                        .Where(poit => poit.ProductOptionItemId == oldProductOptionKey)
                        .ToList();

                    foreach (var productOptionTranslation in productOptionTranslations)
                    {
                        s.Detach(productOptionTranslation);

                        productOptionTranslation.ProductOptionItemId = productOptionItem.bvin;
                        s.AddEntity(productOptionTranslation);
                    }
                }
                s.SubmitChanges();
            }
        }
    }
}