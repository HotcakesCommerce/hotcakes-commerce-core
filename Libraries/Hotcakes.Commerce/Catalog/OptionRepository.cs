#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Perform database operation on hcc_ProductOptions and hcc_ProductOptionTranslation tables
    /// </summary>
    public class OptionRepository :
        HccLocalizationRepoBase<hcc_ProductOptions, hcc_ProductOptionTranslation, Option, Guid>
    {
        private readonly OptionItemRepository itemRepository;
        private readonly ProductOptionAssociationRepository optionCrosses;

        public OptionRepository(HccRequestContext c)
            : base(c)
        {
            optionCrosses = new ProductOptionAssociationRepository(c);
            itemRepository = new OptionItemRepository(c);
        }

        protected override Expression<Func<hcc_ProductOptions, Guid>> ItemKeyExp
        {
            get { return po => po.bvin; }
        }

        protected override Expression<Func<hcc_ProductOptionTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pot => pot.ProductOptionId; }
        }

        protected override Func<JoinedItem<hcc_ProductOptions, hcc_ProductOptionTranslation>, bool> MatchItems(
            Option item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return po => po.Item.bvin == guid;
        }

        protected override Func<JoinedItem<hcc_ProductOptions, hcc_ProductOptionTranslation>, bool> NotMatchItems(
            List<Option> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return po => !po.Item.IsShared && !itemGuids.Contains(po.Item.bvin);
        }

        /// <summary>
        ///     Copy from database object model to view model.
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductOptions" /> instance</param>
        /// <param name="model"><see cref="Option" /> instance </param>
        protected override void CopyItemToModel(hcc_ProductOptions data, Option model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.IsShared = data.IsShared;
            model.IsVariant = data.IsVariant;
            model.SetProcessor((OptionTypes) data.OptionType);
            model.NameIsHidden = data.NameIsHidden;
            model.Settings = OptionSettings.FromJson(data.Settings);
            model.StoreId = data.StoreId;
            model.IsColorSwatch = data.IsColorSwatch;
        }

        /// <summary>
        ///     Copy name from database object model to view model
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductOptions" /> instance</param>
        /// <param name="model"><see cref="Option" /> instance </param>
        protected override void CopyTransToModel(hcc_ProductOptionTranslation data, Option model)
        {
            model.Name = data.Name;
            model.TextSettings = OptionSettings.FromJson(data.TextSettings);
        }

        /// <summary>
        ///     Copy viewer model to database object model
        /// </summary>
        /// <param name="data">Joined item for database object model</param>
        /// <param name="model"><see cref="Option" /> instance</param>
        protected override void CopyModelToItem(JoinedItem<hcc_ProductOptions, hcc_ProductOptionTranslation> data,
            Option model)
        {
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.IsShared = model.IsShared;
            data.Item.IsVariant = model.IsVariant;
            data.Item.OptionType = (int) model.OptionType;
            data.Item.NameIsHidden = model.NameIsHidden;
            data.Item.Settings = model.Settings.ToJson();
            data.Item.StoreId = model.StoreId;
            data.Item.IsColorSwatch = model.IsColorSwatch;
        }

        /// <summary>
        ///     Copy view model name to database object model
        /// </summary>
        /// <param name="data">Joined item for database object model</param>
        /// <param name="model"><see cref="Option" /> instance</param>
        protected override void CopyModelToTrans(JoinedItem<hcc_ProductOptions, hcc_ProductOptionTranslation> data,
            Option model)
        {
            data.ItemTranslation.ProductOptionId = data.Item.bvin;

            data.ItemTranslation.Name = model.Name;
            data.ItemTranslation.TextSettings = model.TextSettings.ToJson();
        }

        /// <summary>
        ///     Get list of option items for given options
        /// </summary>
        /// <param name="models">List of <see cref="Option" /> instances</param>
        protected override void GetSubItems(List<Option> models)
        {
            var bvinIds = models.Select(s => s.Bvin).ToList();
            var optionItems = itemRepository.FindForManyOption(bvinIds);

            foreach (var model in models)
            {
                var items = optionItems.Where(oi => oi.OptionBvin == model.Bvin).ToList();
                model.Items = items;
            }
        }

        /// <summary>
        ///     Merge the option and its option items.
        /// </summary>
        /// <param name="model"><see cref="Option" /> instance</param>
        protected override void MergeSubItems(Option model)
        {
            itemRepository.MergeList(model.Bvin, model.Items);
        }

        /// <summary>
        ///     Find option by unique identifier
        /// </summary>
        /// <param name="bvin">Option unique identifier</param>
        /// <returns>Returns <see cref="Option" /> instance</returns>
        public Option Find(string bvin)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        ///     Get option by unique identifier from all store
        /// </summary>
        /// <param name="bvin">Option unique identifier</param>
        /// <returns>Returns <see cref="Option" /> instance</returns>
        public Option FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        /// <summary>
        ///     Create new option
        /// </summary>
        /// <param name="item"><see cref="Option" /> instance</param>
        /// <returns>Returns true if option created successfully.</returns>
        public override bool Create(Option item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        ///     Update Option
        /// </summary>
        /// <param name="c"><see cref="Option" /> instance</param>
        /// <returns>Returns true if the option updated successully.</returns>
        public bool Update(Option c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        /// <summary>
        ///     Delete option
        /// </summary>
        /// <param name="bvin">Option unique identifier</param>
        /// <returns>Returns true if option deleted successfully</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == storeId);
        }

        /// <summary>
        ///     Delete list of options
        /// </summary>
        /// <param name="optionBvins">List of option unique identifier</param>
        /// <returns>Returns true if options deleted successfully</returns>
        public bool Delete(List<string> optionBvins)
        {
            var storeId = Context.CurrentStore.Id;
            var optionGuids = optionBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return Delete(y => y.StoreId == storeId && optionGuids.Contains(y.bvin));
        }

        /// <summary>
        ///     Get list of all options with paging
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns list of <see cref="Option" /> instances</returns>
        public List<Option> FindAll(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.ItemTranslation.Name);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find all shared options with paging
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns list of <see cref="Option" /> instances</returns>
        public List<Option> FindAllShared(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.IsShared)
                    .Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.ItemTranslation.Name);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find shared option by name and option type
        /// </summary>
        /// <param name="name">name of the option</param>
        /// <param name="optionType"><see cref="OptionTypes" /> of the option</param>
        /// <returns></returns>
        public Option FindSharedOptionByNameAndType(string name, OptionTypes optionType)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var option = GetJoinedQuery(s).AsNoTracking().FirstOrDefault(y => y.Item.StoreId == storeId
                                                                   && y.Item.IsShared
                                                                   &&
                                                                   y.ItemTranslation.Name.Equals(name,
                                                                       StringComparison.CurrentCultureIgnoreCase)
                                                                   && y.Item.OptionType == (int) optionType);

                return FirstPoco(option);
            }
        }

        /// <summary>
        ///     Find list of options by id
        /// </summary>
        /// <param name="bvins">List of option unique identifier</param>
        /// <returns>Returns list <see cref="Option" /> instances</returns>
        public List<Option> FindMany(List<string> bvins)
        {
            var result = new List<Option>();
            var storeId = Context.CurrentStore.Id;
            var guids = bvins
                .Select(bvin => DataTypeHelper.BvinToGuid(bvin))
                .ToList();

            using (var s = CreateReadStrategy())
            {
                var items = GetJoinedQuery(s)
                    .AsNoTracking()
                    .Where(y => guids.Contains(y.Item.bvin))
                    .Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.Item.bvin);

                if (items != null)
                {
                    result = ListPoco(items);
                }
            }

            return result;
        }

        /// <summary>
        ///     Return list of options for each given product ids
        /// </summary>
        /// <param name="productBvins">List of product unique identifiers</param>
        /// <returns>Returns dictionary with product and its options</returns>
        public Dictionary<string, List<Option>> FindByProductIds(List<string> productBvins)
        {
            var result = new Dictionary<string, List<Option>>();
            foreach (var productBvin in productBvins)
            {
                result[productBvin] = new List<Option>();
            }
            var crosses = optionCrosses.FindForProducts(productBvins, 1, int.MaxValue);
            if (crosses.Count == 0)
                return result;

            var optionIds = crosses.Select(c => c.OptionBvin).ToList();
            var allOptions = FindMany(optionIds);

            foreach (var productBvin in productBvins)
            {
                var optionList = result[productBvin];

                var productCrosses = crosses.Where(c => c.ProductBvin == productBvin).ToList();
                foreach (var cross in productCrosses)
                {
                    var option = allOptions.Where(y => y.Bvin == cross.OptionBvin).FirstOrDefault();
                    if (option != null)
                        optionList.Add(option);
                }
            }

            return result;
        }

        /// <summary>
        ///     Find list of options for given product id
        /// </summary>
        /// <param name="productBvin">Product unique identifier</param>
        /// <returns>List of  <see cref="Option" /> instances.</returns>
        public List<Option> FindByProductId(string productBvin)
        {
            var crosses = optionCrosses.FindForProduct(productBvin, 1, 1000);
            var optionIds = crosses.Select(c => c.OptionBvin).ToList();

            // FindMany sorts by BVIN so we
            // need to resort based on option order
            // in ProductXOption table
            var unsorted = FindMany(optionIds);
            var result = new List<Option>();
            foreach (var cross in crosses)
            {
                var found = unsorted.Where(y => y.Bvin == cross.OptionBvin).FirstOrDefault();
                if (found != null)
                    result.Add(found);
            }

            return result;
        }

        /// <summary>
        ///     Delete options for given product id
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <returns>Returns true if the options removed successfully.</returns>
        public bool DeleteForProductId(string productId)
        {
            var opts = FindByProductId(productId);
            optionCrosses.DeleteAllForProduct(productId);
            var notSharedOptionBvins = opts.Where(o => !o.IsShared).Select(o => o.Bvin).ToList();
            Delete(notSharedOptionBvins);
            return true;
        }

        /// <summary>
        ///     Merge products options
        /// </summary>
        /// <param name="productBvin">Product unique identifier</param>
        /// <param name="subitems"><see cref="OptionList" /> instance</param>
        public void MergeList(string productBvin, OptionList subitems)
        {
            var storeId = Context.CurrentStore.Id;
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.StoreId = storeId;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            var result = MergeList(subitems,
                po => po.hcc_ProductXOption.Where(pxo => pxo.ProductBvin == productGuid).Count() > 0);

            var created = subitems.Where(i => !result.OldValues.Select(ov => ov.Bvin).Contains(i.Bvin)).ToList();
            var deleted = result.OldValues.Where(ov => !subitems.Select(i => i.Bvin).Contains(ov.Bvin)).ToList();

            foreach (var item in created)
            {
                optionCrosses.AddOptionToProduct(productBvin, item.Bvin);
            }

            foreach (var item in deleted)
            {
                optionCrosses.RemoveOptionFromProduct(productBvin, item.Bvin);
            }
        }

        /// <summary>
        ///     Delete option item
        /// </summary>
        /// <param name="optionItemBvin">Option Item unique identifier</param>
        /// <returns>Returns true if the option removed successfully</returns>
        public bool DeleteOptionItem(string optionItemBvin)
        {
            return itemRepository.Delete(optionItemBvin);
        }

        /// <summary>
        ///     Resort the option items for given option
        /// </summary>
        /// <param name="optionId">Option unique identifier</param>
        /// <param name="sortedItemIds">List of sorted items ids.</param>
        /// <returns></returns>
        public bool ResortOptionItems(string optionId, List<string> sortedItemIds)
        {
            return itemRepository.Resort(optionId, sortedItemIds);
        }

        /// <summary>
        ///     Find option item
        /// </summary>
        /// <param name="bvin">Option item unique identifier</param>
        /// <returns>Returns <see cref="OptionItem" /> instance</returns>
        public OptionItem OptionItemFind(string bvin)
        {
            return itemRepository.Find(bvin);
        }

        /// <summary>
        ///     Update option item
        /// </summary>
        /// <param name="item"><see cref="OptionItem" /> instance</param>
        /// <returns>Returns true if option updated successfully.</returns>
        public bool OptionItemUpdate(OptionItem item)
        {
            return itemRepository.Update(item);
        }

        /// <summary>
        ///     Clone product option
        /// </summary>
        /// <param name="productOptionId">Source product option unique identifier</param>
        /// <param name="newProductOptionId">Destionation product option unique identifier</param>
        public void Clone(string productOptionId, out string newProductOptionId)
        {
            var productOptionGuid = DataTypeHelper.BvinToGuid(productOptionId);
            using (var s = CreateStrategy())
            {
                var productOption = s.GetQuery()
                    .Where(po => po.bvin == productOptionGuid)
                    .FirstOrDefault();

                s.Detach(productOption);

                var newProductOptionGuid = Guid.NewGuid();
                productOption.bvin = newProductOptionGuid;

                s.Add(productOption);

                var productOptionTranslations = s.GetQuery<hcc_ProductOptionTranslation>()
                    .Where(pt => pt.ProductOptionId == productOptionGuid)
                    .ToList();

                foreach (var productOptionTranslation in productOptionTranslations)
                {
                    s.Detach(productOptionTranslation);

                    productOptionTranslation.ProductOptionId = newProductOptionGuid;

                    s.AddEntity(productOptionTranslation);
                }
                s.SubmitChanges();

                newProductOptionId = newProductOptionGuid.ToString();
                itemRepository.Clone(productOptionId, newProductOptionId);
            }
        }
    }
}