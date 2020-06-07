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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Search
{
    public class ProductQuery
    {
        public hcc_Product p;
        public IEnumerable<ProductPropertyJoin> ppvj;
        public hcc_ProductTranslation pt;
    }

    /// <summary>
    ///     Contains "Property Value" + "Property" + "Propery Types"
    /// </summary>
    public class ProductPropertyJoin
    {
        /// <summary>
        ///     Reference to Property
        /// </summary>
        public ProductProperty pp;

        /// <summary>
        ///     Property Value
        /// </summary>
        public ProductPropertyValue ppv;

        /// <summary>
        ///     Reference to Product Types
        /// </summary>
        public IEnumerable<hcc_ProductTypeXProductProperty> ptypes;
    }

    /// <summary>
    ///     Contains Product Property with translation
    /// </summary>
    public class ProductProperty
    {
        public hcc_ProductProperty Item;
        public hcc_ProductPropertyTranslation ItemTranslation;
    }

    /// <summary>
    ///     Contains Property Value with translation
    /// </summary>
    public class ProductPropertyValue
    {
        public hcc_ProductPropertyValue Item;
        public hcc_ProductPropertyValueTranslation ItemTranslation;
    }

    public class ProductSearchHelper
    {
        #region Constructor

        public ProductSearchHelper(HccRequestContext reqContext, HccDbContext context, long siteId)
        {
            _reqContext = reqContext;
            _context = context;
            _siteId = siteId;

            _categoryTranslations = context.hcc_CategoryTranslations.
                Where(
                    it =>
                        it.Culture == _reqContext.MainContentCulture || it.Culture == _reqContext.FallbackContentCulture);
            _productTypeTranslations = context.hcc_ProductTypeTranslations.
                Where(
                    it =>
                        it.Culture == _reqContext.MainContentCulture || it.Culture == _reqContext.FallbackContentCulture);
            _propertyChoiceTranslations = context.hcc_ProductPropertyChoiceTranslations.
                Where(
                    it =>
                        it.Culture == _reqContext.MainContentCulture || it.Culture == _reqContext.FallbackContentCulture);

            var productPropertyTranslations = context.hcc_ProductPropertyTranslations.
                Where(
                    it =>
                        it.Culture == _reqContext.MainContentCulture || it.Culture == _reqContext.FallbackContentCulture);
            _productProperties = context.hcc_ProductProperty.Where(pp => pp.DisplayOnSearch).
                GroupJoin(productPropertyTranslations, i => i.Id, it => it.ProductPropertyId,
                    (i, it) => new ProductProperty
                    {
                        Item = i,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2).FirstOrDefault()
                    });

            var productPropertyValueTranslations = context.hcc_ProductPropertyValueTranslations.
                Where(
                    it =>
                        it.Culture == _reqContext.MainContentCulture || it.Culture == _reqContext.FallbackContentCulture);
            var productPropertyValues = context.hcc_ProductPropertyValue.
                GroupJoin(productPropertyValueTranslations, i => i.Id, it => it.ProductPropertyValueId,
                    (i, it) => new ProductPropertyValue
                    {
                        Item = i,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2).FirstOrDefault()
                    });

            _propertiesJoin = _productProperties.
                Join(productPropertyValues, pp => pp.Item.Id, ppv => ppv.Item.PropertyId, (pp, ppv) => new {pp, ppv}).
                GroupJoin(_context.hcc_ProductTypeXProductProperty, pv => pv.pp.Item.Id, ptp => ptp.PropertyId,
                    (pv, ptp) => new ProductPropertyJoin {pp = pv.pp, ppv = pv.ppv, ptypes = ptp});
        }

        #endregion

        #region Fields

        private readonly HccRequestContext _reqContext;
        private readonly HccDbContext _context;
        private long _siteId;
        private ProductSearchQueryAdv _query;
        private List<Guid> _queryCategories;
        private List<Guid?> _queryTypes;
        private List<Guid?> _queryManufacturers;
        private List<Guid?> _queryVendors;
        private List<hcc_ProductProperty> _querySelectedProperties;
        private readonly IQueryable<hcc_CategoryTranslation> _categoryTranslations;
        private readonly IQueryable<hcc_ProductTypeTranslation> _productTypeTranslations;
        private readonly IQueryable<hcc_ProductPropertyChoiceTranslation> _propertyChoiceTranslations;
        private readonly IQueryable<ProductPropertyJoin> _propertiesJoin;
        private readonly IQueryable<ProductProperty> _productProperties;

        #endregion

        #region Implementation

        public ProductSearchResultAdv DoSearch(ProductSearchQueryAdv query, int pageNumber, int pageSize)
        {
            _query = query;
            var result = new ProductSearchResultAdv();

            IQueryable<ProductQuery> dbQuery = null;
            IQueryable<ProductQuery> dbQueryAll = null;

            _queryCategories = query.Categories.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            _queryTypes = query.Types.Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).ToList();
            _queryManufacturers = query.Manufacturers.Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).ToList();
            _queryVendors = query.Vendors.Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).ToList();
            var selPropIds = query.Properties.Keys.ToList();
            _querySelectedProperties = _context.hcc_ProductProperty.Where(pp => selPropIds.Contains(pp.Id)).ToList();

            var productSecurityFilterQuery = _context.hcc_Product.Where(p => p.Status == (int) ProductStatus.Active);
            if (query.IsConsiderSearchable)
            {
                productSecurityFilterQuery = productSecurityFilterQuery.Where(p => p.IsSearchable == query.IsSearchable);
            }
            var prodQuery = SecurityFilter(productSecurityFilterQuery);

            if (query.SortOrder != CategorySortOrder.ProductName &&
                query.SortOrder != CategorySortOrder.ProductNameDescending)
            {
                dbQuery = prodQuery.Where(p => p.IsAvailableForSale)
                    .GroupJoin(_propertiesJoin, p => p.bvin, ppvj => ppvj.ppv.Item.ProductBvin,
                        (p, ppvj) =>
                            new ProductQuery
                            {
                                p = p,
                                pt = null,
                                ppvj = ppvj.Where(pj => pj.ptypes.Any(pt => pt.ProductTypeBvin == p.ProductTypeId))
                            });
            }
            else
            {
                var productTranslation = _context.hcc_ProductTranslations
                    .Where(
                        it =>
                            it.Culture == _reqContext.MainContentCulture ||
                            it.Culture == _reqContext.FallbackContentCulture);
                dbQuery = prodQuery.Where(p => p.IsAvailableForSale)
                    .GroupJoin(productTranslation, p => p.bvin, pt => pt.ProductId, (p, pt)
                        =>
                        new
                        {
                            p,
                            pt =
                                pt.OrderBy(ptr => ptr.Culture == _reqContext.MainContentCulture ? 1 : 2)
                                    .FirstOrDefault()
                        })
                    .GroupJoin(_propertiesJoin, p => p.p.bvin, ppvj => ppvj.ppv.Item.ProductBvin,
                        (p, ppvj) =>
                            new ProductQuery
                            {
                                p = p.p,
                                pt = p.pt,
                                ppvj = ppvj.Where(pj => pj.ptypes.Any(pt => pt.ProductTypeBvin == p.p.ProductTypeId))
                            });
            }
            // Fill price info before filtering by price is done
            result.MinPrice = dbQuery.Min(q => (decimal?) q.p.SitePrice) ?? 0;
            result.MaxPrice = dbQuery.Max(q => (decimal?) q.p.SitePrice) ?? 0;

            using (MiniProfiler.Current.Step("Filtering"))
            {
                // Filtering results by price
                dbQuery = FilterByPrice(dbQuery);
                dbQueryAll = FilterByFacets(dbQuery);

                switch (query.SortOrder)
                {
                    case CategorySortOrder.ProductName:
                        dbQueryAll = dbQueryAll.OrderBy(q => q.pt.ProductName);
                        break;
                    case CategorySortOrder.ProductNameDescending:
                        dbQueryAll = dbQueryAll.OrderByDescending(q => q.pt.ProductName);
                        break;
                    case CategorySortOrder.ProductPriceAscending:
                        dbQueryAll = dbQueryAll.OrderBy(q => q.p.SitePrice);
                        break;
                    case CategorySortOrder.ProductPriceDescending:
                        dbQueryAll = dbQueryAll.OrderByDescending(q => q.p.SitePrice);
                        break;
                    case CategorySortOrder.ProductSKUAscending:
                        dbQueryAll = dbQueryAll.OrderBy(q => q.p.SKU);
                        break;
                    case CategorySortOrder.ProductSKUDescending:
                        dbQueryAll = dbQueryAll.OrderByDescending(q => q.p.SKU);
                        break;
                    default:
                        dbQueryAll =
                            dbQueryAll.OrderBy(
                                q =>
                                    q.p.hcc_ProductXCategory.Where(z => _queryCategories.Contains(z.CategoryId))
                                        .Select(z => z.SortOrder)
                                        .FirstOrDefault());
                        break;
                }
            }

            // Fill facets info
            using (MiniProfiler.Current.Step("Build Manufactory Facets"))
            {
                result.Manufacturers = BuildManufactoryFacets(FilterByFacets(dbQuery, "M"));
            }
            using (MiniProfiler.Current.Step("Build Vendors Facets"))
            {
                result.Vendors = BuildVendorsFacets(FilterByFacets(dbQuery, "V"));
            }
            using (MiniProfiler.Current.Step("Build Types Factes"))
            {
                result.Types = BuildTypesFactes(FilterByFacets(dbQuery, "T"));
            }
            using (MiniProfiler.Current.Step("Build Property Facets"))
            {
                result.Properties = BuildPropertyFacets(FilterByFacets(dbQuery, "P"));
            }
            using (MiniProfiler.Current.Step("Build Selections"))
            {
                // Fill selected data
                result.SelectedCategories = BuildSelectedCategories();
                result.SelectedVendors = BuildSelectedVendors();
                result.SelectedManufacturers = BuildSelectedManufacturers();
                result.SelectedTypes = BuildSelectedTypes();
                result.SelectedProperties = BuildSelectedProperties();

                result.SelectedMinPrice = query.MinPrice ?? result.MinPrice;
                result.SelectedMaxPrice = query.MaxPrice ?? result.MaxPrice;
            }

            using (MiniProfiler.Current.Step("Get Products List"))
            {
                var skip = (pageNumber - 1)*pageSize;
                if (skip < 0) skip = 0;

                var productIds = dbQueryAll.Select(q => q.p.bvin);
                result.TotalCount = productIds.Count();
                result.Products = productIds.
                    Skip(skip).
                    Take(pageSize).
                    ToList().
                    Select(guid => DataTypeHelper.GuidToBvin(guid)).
                    ToList();
            }
            return result;
        }

        protected virtual IQueryable<hcc_Product> SecurityFilter(IQueryable<hcc_Product> items)
        {
            return items;
        }

        private IQueryable<ProductQuery> FilterByPrice(IQueryable<ProductQuery> dbQuery)
        {
            if (_query.MinPrice.HasValue)
                dbQuery = dbQuery.Where(q => q.p.SitePrice >= _query.MinPrice.Value);
            if (_query.MaxPrice.HasValue)
                dbQuery = dbQuery.Where(q => q.p.SitePrice <= _query.MaxPrice.Value);
            return dbQuery;
        }

        private IQueryable<ProductQuery> FilterByFacets(IQueryable<ProductQuery> dbQuery, string excludeFacet = null)
        {
            foreach (var category in _queryCategories)
            {
                dbQuery = dbQuery.Where(q => q.p.hcc_ProductXCategory.Select(c => c.CategoryId).Contains(category));
            }
            if (_queryTypes.Count > 0 && excludeFacet != "T")
                dbQuery = dbQuery.Where(q => _queryTypes.Contains(q.p.ProductTypeId));
            if (_queryManufacturers.Count > 0 && excludeFacet != "M")
                dbQuery = dbQuery.Where(q => _queryManufacturers.Contains(q.p.ManufacturerID));
            if (_queryVendors.Count > 0 && excludeFacet != "V")
                dbQuery = dbQuery.Where(q => _queryVendors.Contains(q.p.VendorID));

            if (_query.Properties.Count > 0 && excludeFacet != "P")
            {
                dbQuery = FilterByPropertyFacets(dbQuery);
            }
            return dbQuery;
        }

        private IQueryable<ProductQuery> FilterByPropertyFacets(IQueryable<ProductQuery> dbQuery,
            long? excludePropertyId = null)
        {
            foreach (var pair in _query.Properties)
            {
                var propertyId = pair.Key;

                if (!excludePropertyId.HasValue || propertyId != excludePropertyId.Value)
                {
                    var propertyValues = pair.Value;

                    var property = _querySelectedProperties.FirstOrDefault(p => p.Id == propertyId);
                    if (property.IsLocalizable)
                        dbQuery =
                            dbQuery.Where(
                                q =>
                                    q.ppvj.Where(
                                        pj =>
                                            pj.ppv.Item.PropertyId == propertyId &&
                                            propertyValues.Contains(pj.ppv.ItemTranslation.PropertyLocalizableValue))
                                        .Count() > 0);
                    else
                        dbQuery =
                            dbQuery.Where(
                                q =>
                                    q.ppvj.Where(
                                        pj =>
                                            pj.ppv.Item.PropertyId == propertyId &&
                                            propertyValues.Contains(pj.ppv.Item.PropertyValue)).Count() > 0);
                }
            }
            return dbQuery;
        }

        private List<FacetItem> BuildManufactoryFacets(IQueryable<ProductQuery> dbQuery)
        {
            return dbQuery.
                Join(_context.hcc_Manufacturer, q => q.p.ManufacturerID, m => m.bvin, (p, m) => m).
                GroupBy(m => new {ManufacturerId = m.bvin, ManufacturerName = m.DisplayName}).
                Select(m => new InternalFacetItem
                {
                    Id = m.Key.ManufacturerId,
                    Name = m.Key.ManufacturerName,
                    Count = m.Sum(t => 1)
                }).ToList().
                OrderBy(f => f.Name).
                Select(f => f.Convert()).
                ToList();
        }

        private List<FacetItem> BuildVendorsFacets(IQueryable<ProductQuery> dbQuery)
        {
            return dbQuery.
                Join(_context.hcc_Vendor, q => q.p.VendorID, v => v.bvin, (p, v) => v).
                GroupBy(v => new {VendorId = v.bvin, VendorName = v.DisplayName}).
                Select(v => new InternalFacetItem
                {
                    Id = v.Key.VendorId,
                    Name = v.Key.VendorName,
                    Count = v.Sum(t => 1)
                }).OrderBy(f => f.Name).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        private List<FacetItem> BuildTypesFactes(IQueryable<ProductQuery> dbQuery)
        {
            return dbQuery.
                Join(_context.hcc_ProductType, q => q.p.ProductTypeId, pt => pt.bvin, (p, pt) => pt).
                GroupBy(pt => pt.bvin).
                Select(g => new
                {
                    ProductTypeId = g.Key,
                    Count = g.Sum(t => 1)
                }).
                GroupJoin(_productTypeTranslations, pt => pt.ProductTypeId, ptt => ptt.ProductTypeId, (i, it) => new
                {
                    i.ProductTypeId,
                    i.Count,
                    ItemTranslation =
                        it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2).FirstOrDefault()
                }).
                Select(pt => new InternalFacetItem
                {
                    Id = pt.ProductTypeId,
                    Name = pt.ItemTranslation.ProductTypeName,
                    Count = pt.Count
                }).
                OrderBy(f => f.Name).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        private List<PropertyFacetItem> BuildPropertyFacets(IQueryable<ProductQuery> dbQuery)
        {
            var result = new List<PropertyFacetItem>();

            // Group property values by property id
            var propertyInfoValuesGrouped = dbQuery.SelectMany(q => q.ppvj).GroupBy(q => q.pp.Item.Id).ToList();

            foreach (var propertyInfoValues in propertyInfoValuesGrouped)
            {
                // Get current property
                var property = propertyInfoValues.FirstOrDefault().pp;
                // Filter products by all properties except current property
                var dbQueryFiltered = FilterByPropertyFacets(dbQuery, property.Item.Id);
                // Get filtered property values by current property 
                var propertyInfoValuesFiltered =
                    dbQueryFiltered.SelectMany(q => q.ppvj).Where(ppvj => ppvj.pp.Item.Id == property.Item.Id);

                List<string> propertyValues = null;
                if (property.Item.IsLocalizable)
                    propertyValues = propertyInfoValuesFiltered.
                        Where(r => r.ppv.ItemTranslation != null).
                        Select(r => r.ppv.ItemTranslation.PropertyLocalizableValue).
                        Distinct().ToList();
                else
                    propertyValues = propertyInfoValuesFiltered.
                        Where(r => r.ppv.Item != null).
                        Select(r => r.ppv.Item.PropertyValue).
                        Distinct().ToList();

                var propertyFacet = new PropertyFacetItem();
                propertyFacet.Id = property.Item.Id;
                propertyFacet.PropertyName = property.Item.PropertyName;
                if (property.ItemTranslation != null)
                    propertyFacet.DisplayName = property.ItemTranslation.DisplayName;
                else
                    propertyFacet.DisplayName = property.Item.PropertyName;

                foreach (var propertyValue in propertyValues)
                {
                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        var facetItem = new FacetItem();

                        facetItem.Id = propertyValue;
                        if (property.Item.TypeCode == (int) ProductPropertyType.MultipleChoiceField)
                        {
                            var choiceId = int.Parse(propertyValue);
                            var propChoice = _context.hcc_ProductPropertyChoice.
                                GroupJoin(_propertyChoiceTranslations, i => i.Id, it => it.ProductPropertyChoiceId,
                                    (i, it) => new
                                    {
                                        Item = i,
                                        ItemTranslation =
                                            it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2)
                                                .FirstOrDefault()
                                    }).
                                FirstOrDefault(ppc => ppc.Item.Id == choiceId);

                            if (propChoice.ItemTranslation != null)
                                facetItem.Name = propChoice.ItemTranslation.DisplayName;
                        }
                        else
                        {
                            facetItem.Name = propertyValue;
                        }

                        if (property.Item.IsLocalizable)
                            facetItem.Count = propertyInfoValuesFiltered.
                                Where(r => r.ppv.ItemTranslation != null).
                                Count(r => r.ppv.ItemTranslation.PropertyLocalizableValue == propertyValue);
                        else
                            facetItem.Count = propertyInfoValuesFiltered.
                                Where(r => r.ppv.Item != null).
                                Count(r => r.ppv.Item.PropertyValue == propertyValue);

                        propertyFacet.FacetItems.Add(facetItem);
                    }
                }
                if (propertyFacet.FacetItems.Count > 0)
                {
                    result.Add(propertyFacet);
                }
            }
            // End - Build product type properties facets

            return result;
        }

        public static List<FacetItem> GroupCategories(List<FacetItem> list)
        {
            var result = new List<FacetItem>();
            var groups = new List<GroupFacetItem>();

            // extract all items that has parent
            foreach (var i in list)
            {
                if (string.IsNullOrEmpty(i.ParentId))
                {
                    result.Add(i);
                }
                else
                {
                    var group = groups.FirstOrDefault(g => g.Id == i.ParentId);
                    if (group == null)
                    {
                        group = new GroupFacetItem
                        {
                            Id = i.ParentId,
                            Name = i.ParentName,
                            ChildItems = new List<FacetItem>()
                        };
                        groups.Add(group);
                    }
                    group.ChildItems.Add(i);
                }
            }

            // append group items to result
            foreach (var g in groups)
            {
                if (g.ChildItems.Count == 1)
                {
                    result.Add(g.ChildItems.First());
                }
                else
                {
                    g.ChildItems = g.ChildItems.OrderByDescending(i => i.Count).ToList();
                    g.Count = g.ChildItems.Sum(i => i.Count);
                    result.Add(g);
                }
            }

            return result.OrderByDescending(i => i.Count).ToList();
        }

        private List<SelectedPropertyFacetItem> BuildSelectedProperties()
        {
            var result = new List<SelectedPropertyFacetItem>();

            foreach (var property in _query.Properties)
            {
                var propertyId = property.Key;
                var propertyValues = property.Value;

                var productProperty = _productProperties.
                    FirstOrDefault(pp => pp.Item.Id == propertyId);

                if (productProperty != null)
                {
                    var propFacet = new SelectedPropertyFacetItem();
                    propFacet.Id = productProperty.Item.Id;
                    propFacet.PropertyName = productProperty.Item.PropertyName;
                    propFacet.DisplayName = productProperty.ItemTranslation.DisplayName;
                    propFacet.PropertyValues = new List<SelectedFacetItem>();

                    foreach (var propValue in propertyValues)
                    {
                        if (productProperty.Item.TypeCode == (int) ProductPropertyType.MultipleChoiceField)
                        {
                            var choiceId = int.Parse(propValue);
                            var propChoice = _context.hcc_ProductPropertyChoice.
                                GroupJoin(_propertyChoiceTranslations, i => i.Id, it => it.ProductPropertyChoiceId,
                                    (i, it) => new
                                    {
                                        Item = i,
                                        ItemTranslation =
                                            it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2)
                                                .FirstOrDefault()
                                    }).
                                FirstOrDefault(ppc => ppc.Item.Id == choiceId);

                            if (propChoice.ItemTranslation != null)
                                propFacet.PropertyValues.Add(new SelectedFacetItem
                                {
                                    Id = propValue,
                                    Name = propChoice.ItemTranslation.DisplayName
                                });
                        }
                        else
                        {
                            propFacet.PropertyValues.Add(new SelectedFacetItem {Id = propValue, Name = propValue});
                        }
                    }

                    result.Add(propFacet);
                }
            }
            return result;
        }

        private List<SelectedFacetItem> BuildSelectedTypes()
        {
            return _context.hcc_ProductType.
                Where(pt => _queryTypes.Contains(pt.bvin)).
                GroupJoin(_productTypeTranslations, pt => pt.bvin, ptt => ptt.ProductTypeId, (i, it) => new
                {
                    Item = i,
                    ItemTranslation =
                        it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2).FirstOrDefault()
                }).
                Select(pt => new InternalSelectedFacetItem
                {
                    Id = pt.Item.bvin,
                    Name = pt.ItemTranslation.ProductTypeName
                }).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        private List<SelectedFacetItem> BuildSelectedManufacturers()
        {
            return _context.hcc_Manufacturer.
                Where(m => _queryManufacturers.Contains(m.bvin)).
                Select(m => new InternalSelectedFacetItem
                {
                    Id = m.bvin,
                    Name = m.DisplayName
                }).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        private List<SelectedFacetItem> BuildSelectedVendors()
        {
            return _context.hcc_Vendor.
                Where(v => _queryVendors.Contains(v.bvin)).
                Select(v => new InternalSelectedFacetItem
                {
                    Id = v.bvin,
                    Name = v.DisplayName
                }).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        private List<SelectedFacetItem> BuildSelectedCategories()
        {
            return _context.hcc_Category.
                Where(c => _queryCategories.Contains(c.bvin)).
                GroupJoin(_categoryTranslations, i => i.bvin, it => it.CategoryId, (i, it) => new
                {
                    Item = i,
                    ItemTranslation =
                        it.OrderBy(iit => iit.Culture == _reqContext.MainContentCulture ? 1 : 2).FirstOrDefault()
                }).
                Select(c => new InternalSelectedFacetItem
                {
                    Id = c.Item.bvin,
                    Name = c.ItemTranslation.Name
                }).
                ToList().
                Select(f => f.Convert()).
                ToList();
        }

        #endregion
    }
}