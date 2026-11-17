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
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Persists the search lexicon, indexed objects, and object-word scores in the Hotcakes SQL database.
    /// </summary>
    /// <remarks>
    /// The provider uses the search tables and stored procedure exposed by <see cref="HccDbContext"/>.
    /// Basic searches are ranked first by the number of matched words and then by the sum of their
    /// stored scores. Product searches additionally apply product security, filters, culture fallback,
    /// and facet calculation in SQL. The implementation intentionally preserves the database provider's
    /// existing XML word-score representation and sentinel behavior.
    /// </remarks>
    public class SqlSearchProvider : ISearchProvider
    {
        private readonly HccRequestContext _hccRequestContext;

        /// <summary>
        /// Initializes a provider using the request context whose content cultures are used by product searches.
        /// </summary>
        /// <param name="hccRequestContext">The request context supplying the main and fallback content cultures.</param>
        public SqlSearchProvider(HccRequestContext hccRequestContext)
        {
            _hccRequestContext = hccRequestContext;
        }

        /// <summary>
        /// Creates the database context used by this provider.
        /// </summary>
        /// <returns>A new Hotcakes database context.</returns>
        /// <remarks>Overrides can supply a specialized context for an extension of the provider.</remarks>
        protected HccDbContext CreateHccDbContext()
        {
            return Factory.CreateHccDbContext();
        }

        #region Lexicon operations

        /// <summary>
        /// Inserts a stemmed word and culture into the SQL lexicon.
        /// </summary>
        /// <param name="stemmedWord">The already-stemmed word to persist.</param>
        /// <param name="culture">The culture associated with the word.</param>
        /// <returns>The database-generated lexicon identifier.</returns>
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

        /// <summary>
        /// Finds the SQL lexicon identifier for a stemmed word in a culture.
        /// </summary>
        /// <param name="stemmedWord">The stemmed word to find.</param>
        /// <param name="culture">The culture to match.</param>
        /// <returns>The matching identifier, or <c>0</c> when no row is found.</returns>
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

        /// <summary>
        /// Finds all SQL lexicon identifiers whose words occur in the supplied list for a culture.
        /// </summary>
        /// <param name="stemmedWords">The stemmed words to match.</param>
        /// <param name="culture">The culture to match.</param>
        /// <returns>The matching database identifiers.</returns>
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

        /// <summary>
        /// Inserts an indexed search object and assigns its database identifier to the object.
        /// </summary>
        /// <param name="s">The search object to persist.</param>
        /// <returns>The assigned search-object identifier.</returns>
        /// <remarks>The index timestamp is set to the current UTC time by the provider.</remarks>
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

        /// <summary>
        /// Loads one indexed search object by its database identifier.
        /// </summary>
        /// <param name="id">The search-object identifier.</param>
        /// <returns>The matching object, or <c>null</c> when it is not present.</returns>
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

        /// <summary>
        /// Loads all indexed search objects whose database identifiers occur in the supplied list.
        /// </summary>
        /// <param name="ids">The search-object identifiers to load.</param>
        /// <returns>The matching objects as materialized search objects.</returns>
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

        /// <summary>
        /// Finds an indexed object by site, object type, and application object identifier.
        /// </summary>
        /// <param name="siteId">The site identifier owning the indexed object.</param>
        /// <param name="type">The search object type.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <returns>The matching object, or <c>null</c> when no row matches all three values.</returns>
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

        /// <summary>
        /// Determines whether an indexed object exists for a site, type, and application identifier.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="type">The search object type.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <returns><c>true</c> when a matching SQL row exists; otherwise, <c>false</c>.</returns>
        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            using (var context = CreateHccDbContext())
            {
                return context.hcc_SearchObjects.Any(so =>
                    so.ObjectType == type && so.ObjectId == objectId && so.SiteId == siteId);
            }
        }

        /// <summary>
        /// Deletes an indexed search object by database identifier.
        /// </summary>
        /// <param name="id">The search-object identifier.</param>
        /// <returns><c>true</c> when a row was deleted; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Adds an object and its word scores through the database indexing procedure.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <param name="objectType">The search object type.</param>
        /// <param name="title">The object title.</param>
        /// <param name="wordScores">A map from stemmed word to its score.</param>
        /// <param name="culture">The culture of the indexed content.</param>
        /// <remarks>
        /// The score map is serialized as <c>&lt;dict&gt;</c> containing <c>&lt;pair&gt;</c> elements with
        /// <c>&lt;word&gt;</c> and <c>&lt;score&gt;</c> children. Values are appended without XML escaping;
        /// callers therefore retain responsibility for the existing word format. The stored procedure
        /// persists the object, lexicon associations, and scores according to the database schema.
        /// </remarks>
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

        /// <summary>
        /// Deletes the SQL object-word row matching both object and word identifiers.
        /// </summary>
        /// <param name="word">The object-word association to remove.</param>
        /// <returns><c>true</c> when a row was deleted; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Inserts an object-word association and its score into SQL.
        /// </summary>
        /// <param name="w">The association containing the object, word, site, and score values.</param>
        /// <returns><c>true</c> after the insert is submitted successfully.</returns>
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

        /// <summary>
        /// Searches across sites for indexed objects matching the supplied word identifiers.
        /// </summary>
        /// <param name="wordIds">The lexicon identifiers to match.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of objects to return.</param>
        /// <param name="totalResults">Receives the total number of grouped matches before paging.</param>
        /// <returns>The page of objects, ranked by match count and then summed score.</returns>
        /// <remarks>This overload delegates to <see cref="DoSearchBySite(long, List{long}, int, int, ref int)"/> with site filtering disabled.</remarks>
        public List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults)
        {
            return DoSearchBySite(-1, wordIds, pageNumber, pageSize, ref totalResults);
        }

        /// <summary>
        /// Searches indexed objects for matching words, optionally restricted to a site.
        /// </summary>
        /// <param name="siteId">The site identifier; values greater than zero restrict results to that site.</param>
        /// <param name="wordIds">The lexicon identifiers to match.</param>
        /// <param name="pageNumber">The one-based page number; negative paging offsets are clamped to zero.</param>
        /// <param name="pageSize">The maximum number of objects to return.</param>
        /// <param name="totalResults">Receives the total number of grouped matches before paging.</param>
        /// <returns>Objects in SQL ranking order: descending matched-word count, then descending score.</returns>
        public List<SearchObject> DoSearchBySite(long siteId, List<long> wordIds, int pageNumber, int pageSize,
            ref int totalResults)
        {
            using (var context = CreateHccDbContext())
            {
                var results = new List<SearchObject>();

                var skip = (pageNumber - 1) * pageSize;
                if (skip < 0) skip = 0;

                var query = context.hcc_SearchObjectWords.Where(w => wordIds.Contains(w.WordId));

                if (siteId > 0)
                    query = query.Where(w => w.SiteId == siteId);

                var oderredQuery = query.GroupBy(w => w.SearchObjectId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Score = g.Sum(y => y.Score),
                        Count = g.Count()
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

                // Inline fetch of search objects to avoid creating a separate context and extra round trips
                var unsorted = context.hcc_SearchObjects
                    .Where(o => objectIds.Contains(o.Id))
                    .Select(o => new SearchObject
                    {
                        Id = o.Id,
                        ObjectId = o.ObjectId,
                        ObjectType = o.ObjectType,
                        Title = o.Title,
                        SiteId = o.SiteId,
                        LastIndexUtc = o.LastIndexUtc
                    })
                    .ToList();

                var unsortedDict = unsorted.ToDictionary(x => x.Id);

                // Preserve ordering from items
                foreach (var s in items)
                {
                    if (unsortedDict.TryGetValue(s.Id, out var temp))
                    {
                        results.Add(temp);
                    }
                }

                return results;
            }
        }

        /// <summary>
        /// Performs an advanced product search across sites.
        /// </summary>
        /// <param name="wordIds">The lexicon identifiers to match.</param>
        /// <param name="query">The product filters, culture-sensitive facet selections, and price bounds.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products to return.</param>
        /// <returns>Products, counts, price bounds, selected facets, and available facets.</returns>
        /// <remarks>This overload delegates to the site-aware implementation with site filtering disabled.</remarks>
        public ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            return DoSearchBySite(-1, wordIds, query, pageNumber, pageSize);
        }

        /// <summary>
        /// Performs an advanced SQL product search using word scores and product filters.
        /// </summary>
        /// <param name="siteId">The site identifier; values greater than zero restrict indexed words to that site.</param>
        /// <param name="wordIds">The lexicon identifiers to match.</param>
        /// <param name="query">The product filters and requested facet selections.</param>
        /// <param name="pageNumber">The one-based page number; negative offsets are clamped to zero.</param>
        /// <param name="pageSize">The maximum number of products to return.</param>
        /// <returns>A result containing ranked products, total count, price range, selected values, and facets.</returns>
        /// <remarks>
        /// Active products are security-filtered, ranked by matched-word count and then summed score,
        /// and translated using the main content culture before the fallback content culture. Price
        /// bounds are calculated before price filtering; facets are calculated from the filtered query.
        /// </remarks>
        public ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var result = new ProductSearchResultAdv();

                var skip = (pageNumber - 1) * pageSize;
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
                        Count = g.Count()
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
                                        s.ppvj.Any(
                                            pj =>
                                                pj.ppv.Item.PropertyId == propertyId &&
                                                pj.ppv.ItemTranslation.PropertyLocalizableValue == propertyValue));
                        else
                            dbQueryJ =
                                dbQueryJ.Where(
                                    s =>
                                        s.ppvj.Any(
                                            pj =>
                                                pj.ppv.Item.PropertyId == propertyId &&
                                                pj.ppv.Item.PropertyValue == propertyValue));
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
                        Count = g.Count()
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

                result.Categories = ProductSearchHelper.GroupCategories(result.Categories).OrderByDescending(c => c.Count).ThenBy(c => c.Name).ToList();

                result.Manufacturers = dbQueryJ.
                    Join(context.hcc_Manufacturer, s => s.p.ManufacturerID, m => m.bvin, (s, m) => m).
                    GroupBy(m => new {ManufacturerId = m.bvin, ManufacturerName = m.DisplayName}).
                    Select(m => new InternalFacetItem
                    {
                        Id = m.Key.ManufacturerId,
                        Name = m.Key.ManufacturerName,
                        Count = m.Count()
                    })
                    .OrderByDescending(m => m.Count).ThenBy(m => m.Name)
                    .ToList().
                    Select(f => f.Convert()).
                    ToList();

                result.Vendors = dbQueryJ.
                    Join(context.hcc_Vendor, s => s.p.VendorID, v => v.bvin, (s, v) => v).
                    GroupBy(v => new {VendorId = v.bvin, VendorName = v.DisplayName}).
                    Select(v => new InternalFacetItem
                    {
                        Id = v.Key.VendorId,
                        Name = v.Key.VendorName,
                        Count = v.Count()
                    })
                    .OrderByDescending(v => v.Count).ThenBy(v => v.Name)
                    .ToList().
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
                        Count = g.Count()
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
                    OrderByDescending(f => f.Count).ThenBy(f => f.Name).
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

                if (result.Properties != null && result.Properties.Count > 0)
                {
                    result.Properties = result.Properties.OrderByDescending(p => p.FacetItems.Count).ThenBy(p => p.DisplayName).ToList();
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

        /// <summary>
        /// Performs an advanced product search through the reusable SQL search helper.
        /// </summary>
        /// <param name="siteId">The site identifier passed to the search helper.</param>
        /// <param name="query">The product filters and requested facet selections.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products to return.</param>
        /// <returns>The helper-produced advanced product search result.</returns>
        /// <remarks>The operation is profiled as <c>SqlProductSearcher_DoSearch_DrillDown</c>.</remarks>
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

        /// <summary>
        /// Creates the helper used by the advanced product-search path.
        /// </summary>
        /// <param name="reqContext">The commerce request context.</param>
        /// <param name="context">The database context for the search.</param>
        /// <param name="siteId">The site identifier to pass to the helper.</param>
        /// <returns>A product search helper.</returns>
        /// <remarks>Derived providers can override this extension point without changing the public search contract.</remarks>
        protected virtual ProductSearchHelper CreateSearchHelper(HccRequestContext reqContext, HccDbContext context,
            long siteId)
        {
            return new ProductSearchHelper(reqContext, context, siteId);
        }

        /// <summary>
        /// Applies the provider's product visibility or security filter to a queryable product set.
        /// </summary>
        /// <param name="items">The active-product query to filter.</param>
        /// <returns>The query after security filtering.</returns>
        /// <remarks>The base implementation returns the query unchanged; derived providers can add store- or user-specific rules.</remarks>
        protected virtual IQueryable<hcc_Product> SecurityFilter(IQueryable<hcc_Product> items)
        {
            return items;
        }

        #endregion

        #endregion
    }
}
