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
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Search
{
    public class SqlSearchProvider : ISearchProvider
    {
        private readonly HccRequestContext _hccRequestContext;

        public SqlSearchProvider(HccRequestContext hccRequestContext)
        {
            _hccRequestContext = hccRequestContext;
        }

        protected HccDbContext CreateHccDbContext()
        {
            return Factory.CreateHccDbContext();
        }

        #region Lexicon operations

        public long InsertWord(string stemmedWord, string culture)
        {
            using (var context = CreateHccDbContext())
            {
                var word = new hcc_SearchLexicon();
                word.Word = stemmedWord;
                word.Culture = culture;
                context.hcc_SearchLexicon.Add(word);
                context.SaveChanges();
                return word.Id;
            }
        }

        public long FindWordId(string stemmedWord, string culture)
        {
            using (var context = CreateHccDbContext())
            {
                return context.hcc_SearchLexicon
                    .Where(w => w.Culture == culture && w.Word == stemmedWord)
                    .Select(w => w.Id)
                    .SingleOrDefault();
            }
        }

        public List<long> FindAllWordIds(List<string> stemmedWords, string culture)
        {
            using (var context = CreateHccDbContext())
            {
                return context.hcc_SearchLexicon
                    .Where(w => w.Culture == culture && stemmedWords.Contains(w.Word))
                    .Select(w => w.Id)
                    .ToList();
            }
        }

        #endregion

        #region Search Object operations

        public long ObjectIndexInsert(SearchObject s)
        {
            using (var context = CreateHccDbContext())
            {
                var dataObject = new hcc_SearchObject();
                dataObject.ObjectId = s.ObjectId;
                dataObject.ObjectType = s.ObjectType;
                dataObject.Title = s.Title;
                dataObject.SiteId = s.SiteId;
                dataObject.LastIndexUtc = DateTime.UtcNow;
                context.hcc_SearchObjects.Add(dataObject);
                context.SaveChanges();

                s.Id = dataObject.Id;
                return s.Id;
            }
        }

        public SearchObject ObjectIndexFind(long id)
        {
            using (var context = CreateHccDbContext())
            {
                var obj = context.hcc_SearchObjects
                    .Where(so => so.Id == id)
                    .FirstOrDefault();

                if (obj != null)
                {
                    var result = new SearchObject();
                    result.Id = obj.Id;
                    result.ObjectId = obj.ObjectId;
                    result.ObjectType = obj.ObjectType;
                    result.Title = obj.Title;
                    result.SiteId = obj.SiteId;
                    result.LastIndexUtc = obj.LastIndexUtc;

                    return result;
                }

                return null;
            }
        }

        public List<SearchObject> ObjectIndexFindAllInList(List<long> ids)
        {
            using (var context = CreateHccDbContext())
            {
                var objects = context.hcc_SearchObjects
                    .Where(o => ids.Contains(o.Id))
                    .ToList();

                var result = new List<SearchObject>();
                foreach (var obj in objects)
                {
                    var so = new SearchObject();
                    so.Id = obj.Id;
                    so.ObjectId = obj.ObjectId;
                    so.ObjectType = obj.ObjectType;
                    so.Title = obj.Title;
                    so.SiteId = obj.SiteId;
                    so.LastIndexUtc = obj.LastIndexUtc;
                    result.Add(so);
                }

                return result;
            }
        }

        public SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId)
        {
            using (var context = CreateHccDbContext())
            {
                var o = context.hcc_SearchObjects
                    .Where(so => so.ObjectType == type)
                    .Where(so => so.ObjectId == objectId)
                    .Where(so => so.SiteId == siteId)
                    .FirstOrDefault();
                if (o != null)
                {
                    var result = new SearchObject();
                    result.Id = o.Id;
                    result.ObjectId = o.ObjectId;
                    result.ObjectType = o.ObjectType;
                    result.Title = o.Title;
                    result.SiteId = o.SiteId;
                    result.LastIndexUtc = o.LastIndexUtc;
                    return result;
                }
                return null;
            }
        }

        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            var o = ObjectIndexFindByTypeAndId(siteId, type, objectId);
            if (o != null)
            {
                return true;
            }
            return false;
        }

        public bool ObjectIndexDelete(long id)
        {
            using (var context = CreateHccDbContext())
            {
                var s = context.hcc_SearchObjects
                    .Where(so => so.Id == id)
                    .FirstOrDefault();
                if (s != null)
                {
                    context.hcc_SearchObjects.Remove(s);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public void AddObjectIndex(long siteId, Guid objectId, int objectType, string title,
            Dictionary<string, int> wordScores, string culture)
        {
            using (var context = CreateHccDbContext())
            {
                var xml = new StringBuilder();
                xml.Append("<dict>");
                foreach (var pair in wordScores)
                {
                    xml.AppendFormat("<pair><word>{0}</word><score>{1}</score></pair>", pair.Key, pair.Value);
                }
                xml.Append("</dict>");
                context.AddObjectIndex(
                    siteId,
                    objectId,
                    objectType,
                    title,
                    culture,
                    xml.ToString()
                    );
            }
        }

        #endregion

        #region Search Object Word operations

        public bool ObjectWordIndexDelete(SearchObjectWord word)
        {
            using (var context = CreateHccDbContext())
            {
                var s = context.hcc_SearchObjectWords
                    .Where(w => w.SearchObjectId == word.SearchObjectId && w.WordId == word.WordId)
                    .FirstOrDefault();

                if (s != null)
                {
                    context.hcc_SearchObjectWords.Remove(s);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool ObjectWordIndexInsert(SearchObjectWord w)
        {
            using (var context = CreateHccDbContext())
            {
                var word = new hcc_SearchObjectWord();
                word.Score = w.Score;
                word.SearchObjectId = w.SearchObjectId;
                word.WordId = w.WordId;
                word.SiteId = w.SiteId;

                context.hcc_SearchObjectWords.Add(word);
                context.SaveChanges();
                return true;
            }
        }

        #endregion

        #region Search operations

        public List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults)
        {
            return DoSearchBySite(-1, wordIds, pageNumber, pageSize, ref totalResults);
        }

        public List<SearchObject> DoSearchBySite(long siteId, List<long> wordIds, int pageNumber, int pageSize,
            ref int totalResults)
        {
            using (var context = CreateHccDbContext())
            {
                var results = new List<SearchObject>();

                var skip = (pageNumber - 1)*pageSize;
                if (skip < 0) skip = 0;

                var query = context.hcc_SearchObjectWords.Where(w => wordIds.Contains(w.WordId));

                if (siteId > 0)
                    query = query.Where(w => w.SiteId == siteId);

                var oderredQuery = query.GroupBy(w => w.SearchObjectId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Score = g.Sum(y => y.Score),
                        Count = g.Sum(y => 1)
                    })
                    .OrderByDescending(y => y.Count)
                    .ThenByDescending(y => y.Score);

                totalResults = oderredQuery.Count();
                var items = oderredQuery.Skip(skip).Take(pageSize).ToList();

                var objectIds = new List<long>();
                foreach (var s in items)
                {
                    objectIds.Add(s.Id);
                }

                // Now find all objects but they are unsorted 
                var unsorted = ObjectIndexFindAllInList(objectIds);
                // Make sure results are sorted by keyword values
                foreach (var s in items)
                {
                    var temp = unsorted.Where(y => y.Id == s.Id).FirstOrDefault();
                    if (temp != null)
                    {
                        results.Add(temp);
                    }
                }

                return results;
            }
        }

        public ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            return DoSearchBySite(-1, wordIds, query, pageNumber, pageSize);
        }

        public ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var result = new ProductSearchResultAdv();

                var skip = (pageNumber - 1)*pageSize;
                if (skip < 0) skip = 0;

                var dbQuery = context.hcc_SearchObjectWords.
                    Where(w => wordIds.Contains(w.WordId));
                if (siteId > 0)
                    dbQuery = dbQuery.Where(w => w.SiteId == siteId);

                var productPropertyTranslations = context.hcc_ProductPropertyTranslations.
                    Where(
                        it =>
                            it.Culture == _hccRequestContext.MainContentCulture ||
                            it.Culture == _hccRequestContext.FallbackContentCulture);
                var productProperties = context.hcc_ProductProperty.Where(pp => pp.DisplayOnSearch).
                    GroupJoin(productPropertyTranslations, i => i.Id, it => it.ProductPropertyId, (i, it) => new
                    {
                        Item = i,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault()
                    });

                var productPropertyValueTranslations = context.hcc_ProductPropertyValueTranslations.
                    Where(
                        it =>
                            it.Culture == _hccRequestContext.MainContentCulture ||
                            it.Culture == _hccRequestContext.FallbackContentCulture);
                var productPropertyValues = context.hcc_ProductPropertyValue.
                    GroupJoin(productPropertyValueTranslations, i => i.Id, it => it.ProductPropertyValueId,
                        (i, it) => new
                        {
                            Item = i,
                            ItemTranslation =
                                it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                    .FirstOrDefault()
                        });

                var propertiesJoin = productProperties.
                    Join(productPropertyValues, pp => pp.Item.Id, ppv => ppv.Item.PropertyId, (pp, ppv) => new {pp, ppv})
                    .
                    GroupJoin(context.hcc_ProductTypeXProductProperty, pv => pv.pp.Item.Id, ptp => ptp.PropertyId,
                        (pv, ptp) => new {pv.pp, pv.ppv, ptypes = ptp});


                var productSecurityFilterQuery = context.hcc_Product.Where(p => p.Status == (int) ProductStatus.Active);
                if (query.IsConsiderSearchable)
                {
                    productSecurityFilterQuery =
                        productSecurityFilterQuery.Where(p => p.IsSearchable == query.IsSearchable);
                }
                var products = SecurityFilter(productSecurityFilterQuery);

                var dbQueryJ = dbQuery.
                    Join(context.hcc_SearchObjects, sow => sow.SearchObjectId, so => so.Id, (sow, so) => new {sow, so}).
                    Where(s => s.so.ObjectType == (int) SearchManagerObjectType.Product).
                    GroupBy(s => s.so.ObjectId).
                    Select(g => new
                    {
                        ObjectId = g.Key,
                        Score = g.Sum(y => y.sow.Score),
                        Count = g.Sum(y => 1)
                    }).
                    Join(products, s => s.ObjectId, p => p.bvin, (s, p) => new {so = s, p}).
                    GroupJoin(propertiesJoin, s => s.p.bvin, ppvj => ppvj.ppv.Item.ProductBvin,
                        (s, ppvj) =>
                            new
                            {
                                s.so,
                                s.p,
                                ppvj = ppvj.Where(pj => pj.ptypes.Any(pt => pt.ProductTypeBvin == s.p.ProductTypeId))
                            });

                var queryCategories = query.Categories.
                    Select(bvin => DataTypeHelper.BvinToGuid(bvin)).
                    ToList();
                var queryTypes = query.Types.
                    Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).
                    ToList();
                var queryManufacturers = query.Manufacturers.
                    Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).
                    ToList();
                var queryVendors = query.Vendors.
                    Select(bvin => DataTypeHelper.BvinToNullableGuid(bvin)).
                    ToList();

                if (queryCategories.Count > 0)
                {
                    foreach (var category in queryCategories)
                    {
                        dbQueryJ =
                            dbQueryJ.Where(s => s.p.hcc_ProductXCategory.Select(c => c.CategoryId).Contains(category));
                    }
                }
                if (queryTypes.Count > 0)
                    dbQueryJ = dbQueryJ.Where(s => queryTypes.Contains(s.p.ProductTypeId));
                if (queryManufacturers.Count > 0)
                    dbQueryJ = dbQueryJ.Where(s => queryManufacturers.Contains(s.p.ManufacturerID));
                if (queryVendors.Count > 0)
                    dbQueryJ = dbQueryJ.Where(s => queryVendors.Contains(s.p.VendorID));
                if (query.Properties.Count > 0)
                {
                    foreach (var pair in query.Properties)
                    {
                        var propertyId = pair.Key;
                        var propertyValue = pair.Value[0];
                        var property = context.hcc_ProductProperty.FirstOrDefault(p => p.Id == propertyId);

                        if (property.IsLocalizable)
                            dbQueryJ =
                                dbQueryJ.Where(
                                    s =>
                                        s.ppvj.Where(
                                            pj =>
                                                pj.ppv.Item.PropertyId == propertyId &&
                                                pj.ppv.ItemTranslation.PropertyLocalizableValue == propertyValue)
                                            .Count() > 0);
                        else
                            dbQueryJ =
                                dbQueryJ.Where(
                                    s =>
                                        s.ppvj.Where(
                                            pj =>
                                                pj.ppv.Item.PropertyId == propertyId &&
                                                pj.ppv.Item.PropertyValue == propertyValue).Count() > 0);
                    }
                }

                // Fill price info before filtering by price is done
                var minPrice = dbQueryJ.Min(s => (decimal?) s.p.SitePrice);
                var maxPrice = dbQueryJ.Max(s => (decimal?) s.p.SitePrice);

                result.MinPrice = minPrice ?? 0;
                result.MaxPrice = maxPrice ?? 0;

                // Filtering results by price
                if (query.MinPrice.HasValue)
                    dbQueryJ = dbQueryJ.Where(s => s.p.SitePrice >= query.MinPrice.Value);
                if (query.MaxPrice.HasValue)
                    dbQueryJ = dbQueryJ.Where(s => s.p.SitePrice <= query.MaxPrice.Value);

                // Fill facets info
                var categoryTranslations = context.hcc_CategoryTranslations.
                    Where(
                        it =>
                            it.Culture == _hccRequestContext.MainContentCulture ||
                            it.Culture == _hccRequestContext.FallbackContentCulture);
                result.Categories = dbQueryJ.
                    Join(context.hcc_ProductXCategory, s => s.p.bvin, pc => pc.ProductId, (s, pc) => pc.hcc_Category).
                    GroupJoin(context.hcc_Category, c => c.ParentID, parent => parent.bvin, (c, parentColl) => new
                    {
                        Category = c,
                        Parent = parentColl.FirstOrDefault()
                    }).
                    GroupBy(c => new {CategoryId = c.Category.bvin, ParentId = c.Category.ParentID}).
                    Select(g => new
                    {
                        g.Key.CategoryId,
                        g.Key.ParentId,
                        Count = g.Sum(t => 1)
                    }).
                    GroupJoin(categoryTranslations, i => i.CategoryId, it => it.CategoryId, (i, it) => new
                    {
                        i.CategoryId,
                        i.ParentId,
                        i.Count,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault()
                    }).
                    GroupJoin(categoryTranslations, i => i.ParentId, it => it.CategoryId, (i, it) => new
                    {
                        i.CategoryId,
                        i.ParentId,
                        i.Count,
                        i.ItemTranslation,
                        ParentTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault()
                    }).
                    Select(c => new InternalFacetItem
                    {
                        Id = c.CategoryId,
                        Name = c.ItemTranslation.Name,
                        Count = c.Count,
                        ParentId = c.ParentId,
                        ParentName = c.ParentId != null ? c.ParentTranslation.Name : null
                    }).
                    OrderByDescending(f => f.Count).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.Categories = ProductSearchHelper.GroupCategories(result.Categories);

                result.Manufacturers = dbQueryJ.
                    Join(context.hcc_Manufacturer, s => s.p.ManufacturerID, m => m.bvin, (s, m) => m).
                    GroupBy(m => new {ManufacturerId = m.bvin, ManufacturerName = m.DisplayName}).
                    Select(m => new InternalFacetItem
                    {
                        Id = m.Key.ManufacturerId,
                        Name = m.Key.ManufacturerName,
                        Count = m.Sum(t => 1)
                    }).ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.Vendors = dbQueryJ.
                    Join(context.hcc_Vendor, s => s.p.VendorID, v => v.bvin, (s, v) => v).
                    GroupBy(v => new {VendorId = v.bvin, VendorName = v.DisplayName}).
                    Select(v => new InternalFacetItem
                    {
                        Id = v.Key.VendorId,
                        Name = v.Key.VendorName,
                        Count = v.Sum(t => 1)
                    }).ToList().
                    Select(f => f.Convert()).
                    ToList();

                var productTypeTranslations = context.hcc_ProductTypeTranslations.
                    Where(
                        it =>
                            it.Culture == _hccRequestContext.MainContentCulture ||
                            it.Culture == _hccRequestContext.FallbackContentCulture);
                result.Types = dbQueryJ.
                    Join(context.hcc_ProductType, s => s.p.ProductTypeId, pt => pt.bvin, (s, pt) => pt).
                    GroupBy(pt => pt.bvin).
                    Select(g => new
                    {
                        ProductTypeId = g.Key,
                        Count = g.Sum(t => 1)
                    }).
                    GroupJoin(productTypeTranslations, pt => pt.ProductTypeId, ptt => ptt.ProductTypeId, (i, it) => new
                    {
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault(),
                        i.Count
                    }).
                    Select(pt => new InternalFacetItem
                    {
                        Id = pt.ItemTranslation.ProductTypeId,
                        Name = pt.ItemTranslation.ProductTypeName,
                        Count = pt.Count
                    }).
                    OrderBy(f => f.Name).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                // Build product type properties facets

                var customPropertiesInfo = dbQueryJ.
                    SelectMany(q => q.ppvj).
                    GroupBy(q => q.pp.Item.Id).
                    ToList();

                var propertyChoiceTranslations = context.hcc_ProductPropertyChoiceTranslations.
                    Where(
                        it =>
                            it.Culture == _hccRequestContext.MainContentCulture ||
                            it.Culture == _hccRequestContext.FallbackContentCulture);
                foreach (var customPropertyInfo in customPropertiesInfo)
                {
                    var propertyFacet = new PropertyFacetItem();

                    var property = customPropertyInfo.FirstOrDefault().pp;
                    List<string> propertyValues = null;
                    if (property.Item.IsLocalizable)
                        propertyValues = customPropertyInfo.
                            Where(r => r.ppv.ItemTranslation != null).
                            Select(r => r.ppv.ItemTranslation.PropertyLocalizableValue).
                            Distinct().ToList();
                    else
                        propertyValues = customPropertyInfo.
                            Where(r => r.ppv.Item != null).
                            Select(r => r.ppv.Item.PropertyValue).
                            Distinct().ToList();

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
                                var propChoice = context.hcc_ProductPropertyChoice.
                                    GroupJoin(propertyChoiceTranslations, i => i.Id, it => it.ProductPropertyChoiceId,
                                        (i, it) => new
                                        {
                                            Item = i,
                                            ItemTranslation =
                                                it.OrderBy(
                                                    iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
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
                                facetItem.Count = customPropertyInfo.
                                    Where(r => r.ppv.ItemTranslation != null).
                                    Count(r => r.ppv.ItemTranslation.PropertyLocalizableValue == propertyValue);
                            else
                                facetItem.Count = customPropertyInfo.
                                    Where(r => r.ppv.Item != null).
                                    Count(r => r.ppv.Item.PropertyValue == propertyValue);

                            propertyFacet.FacetItems.Add(facetItem);
                        }
                    }
                    result.Properties.Add(propertyFacet);
                }
                // End - Build product type properties facets

                // Fill selected data
                result.SelectedCategories = context.hcc_Category.
                    Where(c => queryCategories.Contains(c.bvin)).
                    GroupJoin(categoryTranslations, i => i.bvin, it => it.CategoryId, (i, it) => new
                    {
                        Item = i,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault()
                    }).
                    Select(c => new InternalSelectedFacetItem
                    {
                        Id = c.Item.bvin,
                        Name = c.ItemTranslation.Name
                    }).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.SelectedVendors = context.hcc_Vendor.
                    Where(v => queryVendors.Contains(v.bvin)).
                    Select(v => new InternalSelectedFacetItem
                    {
                        Id = v.bvin,
                        Name = v.DisplayName
                    }).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.SelectedManufacturers = context.hcc_Manufacturer.
                    Where(m => queryManufacturers.Contains(m.bvin)).
                    Select(m => new InternalSelectedFacetItem
                    {
                        Id = m.bvin,
                        Name = m.DisplayName
                    }).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.SelectedTypes = context.hcc_ProductType.
                    Where(pt => queryTypes.Contains(pt.bvin)).
                    GroupJoin(productTypeTranslations, pt => pt.bvin, ptt => ptt.ProductTypeId, (i, it) => new
                    {
                        Item = i,
                        ItemTranslation =
                            it.OrderBy(iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                .FirstOrDefault()
                    }).
                    Select(pt => new InternalSelectedFacetItem
                    {
                        Id = pt.Item.bvin,
                        Name = pt.ItemTranslation.ProductTypeName
                    }).
                    ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.SelectedProperties = new List<SelectedPropertyFacetItem>();
                foreach (var property in query.Properties)
                {
                    var propertyId = property.Key;
                    var propertyValue = property.Value[0];

                    var productProperty = productProperties.
                        Where(pp => pp.Item.Id == propertyId).
                        FirstOrDefault();

                    if (productProperty != null)
                    {
                        var propFacet = new SelectedPropertyFacetItem();
                        propFacet.Id = productProperty.Item.Id;
                        propFacet.PropertyName = productProperty.Item.PropertyName;
                        propFacet.DisplayName = productProperty.ItemTranslation.DisplayName;
                        propFacet.PropertyValues = new List<SelectedFacetItem>();

                        if (productProperty.Item.TypeCode == (int) ProductPropertyType.MultipleChoiceField)
                        {
                            var choiceId = int.Parse(propertyValue);
                            var propChoice = context.hcc_ProductPropertyChoice.
                                GroupJoin(propertyChoiceTranslations, i => i.Id, it => it.ProductPropertyChoiceId,
                                    (i, it) => new
                                    {
                                        Item = i,
                                        ItemTranslation =
                                            it.OrderBy(
                                                iit => iit.Culture == _hccRequestContext.MainContentCulture ? 1 : 2)
                                                .FirstOrDefault()
                                    }).
                                FirstOrDefault(ppc => ppc.Item.Id == choiceId);

                            if (propChoice.ItemTranslation != null)
                                propFacet.PropertyValues.Add(new SelectedFacetItem
                                {
                                    Id = propertyValue,
                                    Name = propChoice.ItemTranslation.DisplayName
                                });
                        }
                        else
                        {
                            propFacet.PropertyValues.Add(new SelectedFacetItem
                            {
                                Id = propertyValue,
                                Name = propertyValue
                            });
                        }
                        result.SelectedProperties.Add(propFacet);
                    }
                }

                result.SelectedMinPrice = query.MinPrice ?? result.MinPrice;
                result.SelectedMaxPrice = query.MaxPrice ?? result.MaxPrice;

                var step1a = dbQueryJ.
                    OrderByDescending(y => y.so.Count).
                    ThenByDescending(y => y.so.Score).
                    Select(y => y.p.bvin);

                result.TotalCount = step1a.Count();
                result.Products = step1a.
                    Skip(skip).
                    Take(pageSize).
                    ToList().
                    Select(guid => DataTypeHelper.GuidToBvin(guid)).
                    ToList();

                return result;
            }
        }

        public ProductSearchResultAdv DoSearchBySite(long siteId, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var helper = CreateSearchHelper(_hccRequestContext, context, siteId);

                using (MiniProfiler.Current.Step("SqlProductSearcher_DoSearch_DrillDown"))
                {
                    return helper.DoSearch(query, pageNumber, pageSize);
                }
            }
        }

        #region Implementation

        protected virtual ProductSearchHelper CreateSearchHelper(HccRequestContext reqContext, HccDbContext context,
            long siteId)
        {
            return new ProductSearchHelper(reqContext, context, siteId);
        }

        protected virtual IQueryable<hcc_Product> SecurityFilter(IQueryable<hcc_Product> items)
        {
            return items;
        }

        #endregion

        #endregion
    }
}