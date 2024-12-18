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
using System.Web;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Web.OpenXml;

namespace Hotcakes.Commerce.Catalog
{
    public class CatalogExport
    {
        #region Fields

        private readonly HotcakesApplication _hccApp;

        #endregion

        #region Constructor

        public CatalogExport(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        #endregion

        public void ExportToExcel(List<Product> products, string fileName, HttpResponse response = null)
        {
            var writer = new ExcelWriter("Main");

            var mainWriter = new MainSheetWriter(writer, _hccApp);
            mainWriter.Write(products);

            var catsWriter = new CategoriesSheetWriter(writer, _hccApp);
            catsWriter.Write(products);

            var optWriter = new ChoicesSheetWriter(writer, _hccApp);
            optWriter.Write(products);

            var tabsWriter = new InfoTabsSheetWriter(writer, _hccApp);
            tabsWriter.Write(products);

            var propsWriter = new TypePropertiesSheetWriter(writer, _hccApp);
            propsWriter.Write(products);

            var catTree = new CategoryTreeSheetWriter(writer, _hccApp);
            catTree.Write(catsWriter.CategoryBvins);

            writer.Save();

            if (response != null)
                writer.WriteToResponse(response, fileName);
            else
                writer.StreamToFile(fileName);
        }

        #region Internal declarations

        internal class BaseSheetWriter
        {
            protected int _firstRow;
            protected HotcakesApplication _hccApp;
            protected SpreadsheetStyle _headerStyle;
            protected SpreadsheetStyle _rowStyle;
            protected ExcelWriter _writer;

            internal BaseSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
            {
                _writer = writer;
                _hccApp = hccApp;
                _rowStyle = writer.GetStyle();
                _headerStyle = writer.GetStyle();
                _headerStyle.IsBold = true;
            }

            protected string GetYesNo(bool? val)
            {
                if (val.HasValue)
                    return val.Value ? "YES" : "NO";
                return string.Empty;
            }

            protected string GetNextColumn(string colname)
            {
                return SpreadsheetReader.GetColumnName(colname, 1);
            }
        }

        internal class ProductBaseSheetWriter : BaseSheetWriter
        {
            internal ProductBaseSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            public void Write(List<Product> products)
            {
                Initialize(products);
                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var p in products)
                {
                    rowIndex = WriteProductRow(p, rowIndex);
                }
            }

            protected virtual void Initialize(List<Product> products)
            {
            }

            protected virtual int WriteProductRow(Product p, int rowIndex)
            {
                return ++rowIndex;
            }

            protected virtual void WriteHeader()
            {
            }
        }

        internal class MainSheetWriter : ProductBaseSheetWriter
        {
            private List<CatalogRole> _catalogRoles;
            private List<VendorManufacturer> _manufacturerList;
            private List<ProductInventory> _productInventoryList;
            private List<ProductType> _productTypeList;
            private List<TaxSchedule> _taxScheduleList;
            private List<VendorManufacturer> _vendors;

            internal MainSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            protected override void Initialize(List<Product> products)
            {
                _catalogRoles = _hccApp.CatalogServices.CatalogRoles.FindAllPaged(1, int.MaxValue);
                _productInventoryList = _hccApp.CatalogServices.ProductInventories.FindAllPaged(1, int.MaxValue);
                _productTypeList = _hccApp.CatalogServices.ProductTypes.FindAll();
                _taxScheduleList = _hccApp.OrderServices.TaxSchedules.FindAll(_hccApp.CurrentStore.Id);
                _vendors = _hccApp.ContactServices.Vendors.FindAll();
                _manufacturerList = _hccApp.ContactServices.Manufacturers.FindAll();
            }

            protected override void WriteHeader()
            {
                var centerStyle = _writer.GetStyle();
                centerStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

                _writer.SetColumnsWidths(new double[] {50, 8, 8, 12, 50, 12, 12, 12, 12, 12, 12, 12, 50});

                // Write header rows
                _writer.WriteCellFormatted("B", 1, "Main", centerStyle, 5);
                _writer.WriteCellFormatted("G", 1, "Pricing", centerStyle, 3);
                _writer.WriteCellFormatted("J", 1, "Properties", centerStyle, 4);
                _writer.WriteCellFormatted("N", 1, "SEO", centerStyle, 4);
                _writer.WriteCellFormatted("R", 1, "Tax", centerStyle, 2);
                _writer.WriteCellFormatted("T", 1, "Shipping", centerStyle, 8);
                _writer.WriteCellFormatted("AB", 1, "Advanced", centerStyle, 7);

                _writer.WriteRow("A", 2, new List<string>
                {
                    "SLUG",
                    "Active",
                    "Featured",
                    "SKU",
                    "Name",
                    "Product Type",
                    // pricing
                    "MSRP",
                    "Cost",
                    "Price",
                    // properties
                    "Manufacturer",
                    "Vendor",
                    "Image",
                    "Description",
                    // seo
                    "Search Keywords",
                    "Meta Title",
                    "Meta Description",
                    "Meta Keywords",
                    // tax
                    "Tax Schedule",
                    "Tax Exempt",
                    // shipping
                    "Weight",
                    "Length",
                    "Width",
                    "Height",
                    "Extra Ship Fee",
                    "Ship Mode",
                    "Non-Shipping Product",
                    "Ships in a Separate Box",
                    // advanced
                    "Allow Reviews",
                    "Minimum Qty",
                    "Inventory Mode",
                    "Inventory",
                    "StockOut",
                    "Low Stock at",
                    "Roles",
                    "Searchable",
                    "AllowUpcharge",
                    "UpchargeAmount",
                    "UpchargeUnit"
                }, _headerStyle);

                _firstRow = 3;
            }

            protected override int WriteProductRow(Product p, int rowIndex)
            {
                var invs = GetInventories(p.Bvin);

                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    p.UrlSlug,
                    GetYesNo(p.Status == ProductStatus.Active),
                    GetYesNo(p.Featured),
                    p.Sku,
                    p.ProductName,
                    GetProductType(p.ProductTypeId),
                    p.ListPrice.ToString("C"),
                    p.SiteCost.ToString("C"),
                    p.SitePrice.ToString("C"),
                    GetManufacturer(p.ManufacturerId),
                    GetVendor(p.VendorId),
                    p.ImageFileMedium,
                    p.LongDescription,
                    p.Keywords,
                    p.MetaTitle,
                    p.MetaDescription,
                    p.MetaKeywords,
                    GetTaxSchedule(p.TaxSchedule),
                    GetYesNo(p.TaxExempt),
                    p.ShippingDetails.Weight.ToString(),
                    p.ShippingDetails.Length.ToString(),
                    p.ShippingDetails.Width.ToString(),
                    p.ShippingDetails.Height.ToString(),
                    p.ShippingDetails.ExtraShipFee.ToString("C"),
                    p.ShippingMode.ToString(),
                    GetYesNo(p.ShippingDetails.IsNonShipping),
                    GetYesNo(p.ShippingDetails.ShipSeparately),
                    GetYesNo(p.AllowReviews),
                    p.MinimumQty.ToString(),
                    p.InventoryMode.ToString(),
                    invs.Sum(i => i.QuantityAvailableForSale).ToString(),
                    invs.Sum(i => i.OutOfStockPoint).ToString(),
                    invs.Sum(i => i.LowStockPoint).ToString(),
                    GetRoles(p.Bvin),
                    GetYesNo(p.IsSearchable),
                    GetYesNo(p.AllowUpcharge),
                    p.UpchargeAmount.ToString(),
                    p.UpchargeUnit
                }, _rowStyle);

                return base.WriteProductRow(p, rowIndex);
            }

            #region Private Methods

            private string GetRoles(string bvin)
            {
                var roles = _catalogRoles.Where(r => r.ReferenceId == new Guid(bvin));
                return string.Join(", ", roles.Select(r => r.RoleName));
            }

            private List<ProductInventory> GetInventories(string bvin)
            {
                return _productInventoryList != null
                    ? _productInventoryList.Where(i => i.ProductBvin == bvin)
                        .Where(s => s.StoreId == _hccApp.CurrentStore.Id).ToList()
                    : null;
            }

            private string GetTaxSchedule(long id)
            {
                var taxSchedule = _taxScheduleList.Where(t => t.Id == id).FirstOrDefault();
                return taxSchedule != null ? taxSchedule.Name : "";
            }

            private string GetVendor(string vId)
            {
                var vendor = _vendors.Where(v => v.Bvin == vId)
                    .Where(s => s.StoreId == _hccApp.CurrentStore.Id).FirstOrDefault();
                return vendor != null ? vendor.DisplayName : "";
            }

            private string GetManufacturer(string mId)
            {
                var manufacturer = _manufacturerList.Where(m => m.Bvin == mId)
                    .Where(s => s.StoreId == _hccApp.CurrentStore.Id).FirstOrDefault();
                return manufacturer != null ? manufacturer.DisplayName : "";
            }

            private string GetProductType(string typeId)
            {
                var productType =
                    _productTypeList.Where(p => p.Bvin == typeId)
                        .Where(s => s.StoreId == _hccApp.CurrentStore.Id)
                        .FirstOrDefault();
                return productType != null ? productType.ProductTypeName : "Generic";
            }

            #endregion
        }

        internal class CategoriesSheetWriter : ProductBaseSheetWriter
        {
            private List<CategorySnapshot> _categories;
            private List<CategoryProductAssociation> _crosses;

            internal CategoriesSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
                CategoryBvins = new List<string>();
            }

            internal List<string> CategoryBvins { get; set; }

            protected override void Initialize(List<Product> products)
            {
                _crosses = _hccApp.CatalogServices.CategoriesXProducts.FindAll();
                _categories = _hccApp.CatalogServices.Categories.FindAll();
            }

            protected override void WriteHeader()
            {
                _writer.AddWorksheet("Categories");
                _writer.SetColumnsWidths(new double[] {50});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "PRODUCT SLUG",
                    "CATEGORIES SLUGS"
                }, _headerStyle);


                _firstRow = 2;
            }

            protected override int WriteProductRow(Product p, int rowIndex)
            {
                var bvins = _crosses.Where(y => y.ProductId == p.Bvin)
                    .OrderBy(y => y.SortOrder).Select(y => y.CategoryId).ToList();

                var cats = _categories.Where(y => bvins.Contains(y.Bvin))
                    .OrderBy(y => y.SortOrder)
                    .ToList();

                if (cats.Count > 0)
                {
                    var column = "B";

                    foreach (var cat in cats)
                    {
                        _writer.WriteCell(column, rowIndex, cat.RewriteUrl);
                        column = GetNextColumn(column);

                        if (!CategoryBvins.Contains(cat.Bvin))
                        {
                            CategoryBvins.Add(cat.Bvin);
                        }
                    }

                    _writer.WriteRow("A", rowIndex, new List<string> {p.UrlSlug});

                    return base.WriteProductRow(p, rowIndex);
                }

                return rowIndex;
            }
        }

        internal class ChoicesSheetWriter : ProductBaseSheetWriter
        {
            internal ChoicesSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            protected override void WriteHeader()
            {
                _writer.AddWorksheet("Choices");
                _writer.SetColumnsWidths(new double[] {50, 12});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "PRODUCT SLUG",
                    "CHOICE",
                    "CHOICE TYPE",
                    "SHARED",
                    "CHOICE ITEMS"
                }, _headerStyle);

                _firstRow = 2;
            }

            protected override int WriteProductRow(Product p, int rowIndex)
            {
                var slugHasWritten = false;
                var options = p.Options;

                foreach (var option in options)
                {
                    var column = "E";

                    if (option.OptionType == OptionTypes.Html)
                    {
                        _writer.WriteCell(column, rowIndex, option.TextSettings.GetSettingOrEmpty("html"));
                    }

                    foreach (var optionItem in option.Items)
                    {
                        _writer.WriteCell(column, rowIndex, optionItem.Name);
                        column = GetNextColumn(column);
                    }

                    _writer.WriteRow("B", rowIndex, new List<string>
                    {
                        option.Name,
                        option.OptionType.ToString(),
                        GetYesNo(option.IsShared)
                    });

                    if (!slugHasWritten)
                    {
                        _writer.WriteCell("A", rowIndex, p.UrlSlug);
                        slugHasWritten = true;
                    }


                    rowIndex++;
                }

                return rowIndex;
            }
        }

        internal class InfoTabsSheetWriter : ProductBaseSheetWriter
        {
            internal InfoTabsSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            protected override void WriteHeader()
            {
                _writer.AddWorksheet("Info Tabs");
                _writer.SetColumnsWidths(new double[] {50});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "PRODUCT SLUG",
                    "Tab Name",
                    "Tab Description"
                }, _headerStyle);


                _firstRow = 2;
            }

            protected override int WriteProductRow(Product p, int rowIndex)
            {
                var slugHasWritten = false;

                foreach (var tab in p.Tabs)
                {
                    _writer.WriteRow("B", rowIndex, new List<string>
                    {
                        tab.TabTitle,
                        tab.HtmlData
                    });

                    if (!slugHasWritten)
                    {
                        _writer.WriteCell("A", rowIndex, p.UrlSlug);
                        slugHasWritten = true;
                    }

                    rowIndex++;
                }

                return rowIndex;
            }
        }

        internal class TypePropertiesSheetWriter : ProductBaseSheetWriter
        {
            private List<ProductTypePropertyAssociation> _crosses;
            private List<ProductPropertyValue> _productPropertiesValue;
            private List<ProductProperty> _productsProperties;

            internal TypePropertiesSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            protected override void Initialize(List<Product> products)
            {
                _crosses = _hccApp.CatalogServices.ProductTypesXProperties.FindAllPaged(1, int.MaxValue);
                _productsProperties = _hccApp.CatalogServices.ProductProperties.FindAll();
                _productPropertiesValue = _hccApp.CatalogServices.ProductPropertyValues.FindAllPaged(1, int.MaxValue);
            }

            protected override void WriteHeader()
            {
                _writer.AddWorksheet("Type Properties");
                _writer.SetColumnsWidths(new double[] {50});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "PRODUCT SLUG",
                    "Property Name",
                    "Value"
                }, _headerStyle);


                _firstRow = 2;
            }

            protected override int WriteProductRow(Product p, int rowIndex)
            {
                var slugHasWritten = false;

                var ids = _crosses.Where(y => y.ProductTypeBvin == p.ProductTypeId)
                    .OrderBy(y => y.SortOrder).Select(y => y.PropertyId);

                var props = _productsProperties.Where(y => ids.Contains(y.Id))
                    .OrderBy(y => y.PropertyName).ToList();

                foreach (var prop in props)
                {
                    var propertyValue = _productPropertiesValue.Where(y => y.ProductID == p.Bvin)
                        .Where(y => y.PropertyID == prop.Id).FirstOrDefault();

                    var propVal = string.Empty;

                    if (propertyValue != null)
                    {
                        propVal = prop.IsLocalizable
                            ? propertyValue.PropertyLocalizableValue
                            : propertyValue.PropertyValue;
                    }

                    if (prop.TypeCode == ProductPropertyType.MultipleChoiceField)
                    {
                        long choiceId = -1;
                        if (long.TryParse(propVal, out choiceId))
                        {
                            propVal = prop.GetChoiceName(choiceId);
                        }
                    }

                    _writer.WriteRow("B", rowIndex, new List<string>
                    {
                        prop.DisplayName,
                        propVal
                    });

                    if (!slugHasWritten)
                    {
                        _writer.WriteCell("A", rowIndex, p.UrlSlug);
                        slugHasWritten = true;
                    }

                    rowIndex++;
                }

                return rowIndex;
            }
        }

        internal class CategoryTreeSheetWriter : BaseSheetWriter
        {
            private List<Category> _allCategories;

            internal CategoryTreeSheetWriter(ExcelWriter writer, HotcakesApplication hccApp)
                : base(writer, hccApp)
            {
            }

            public void Write(List<string> catIds)
            {
                var categories = FindUsedCategories(catIds);

                WriteHeader();

                var rowIndex = _firstRow;
                foreach (var c in categories)
                {
                    rowIndex = WriteCategoryRow(c, rowIndex);
                }
            }


            private List<Category> FindUsedCategories(List<string> catIds)
            {
                _allCategories = _hccApp.CatalogServices.Categories.FindAllPaged(1, int.MaxValue);
                var cats = _allCategories.Where(c => catIds.Contains(c.Bvin)).ToList();
                var catsCopy = cats.ToList();

                foreach (var cat in catsCopy)
                {
                    AppendParentAxe(cat, cats);
                }

                return cats;
            }

            private void AppendParentAxe(Category cat, List<Category> parents)
            {
                if (!string.IsNullOrEmpty(cat.ParentId))
                {
                    var parent = cat;

                    while (parent != null)
                    {
                        parent = _allCategories.FirstOrDefault(c => c.Bvin == parent.ParentId);

                        if (parent != null && !parents.Any(p => p.Bvin == parent.Bvin))
                        {
                            parents.Add(parent);
                        }
                    }
                }
            }

            protected void WriteHeader()
            {
                _writer.AddWorksheet("Category Tree");
                _writer.SetColumnsWidths(new double[] {50});

                _writer.WriteRow("A", 1, new List<string>
                {
                    "SLUG",
                    "PARENT SLUG",
                    "Category Name"
                }, _headerStyle);


                _firstRow = 2;
            }

            protected int WriteCategoryRow(Category cat, int rowIndex)
            {
                var parentCat = _allCategories.FirstOrDefault(c => c.Bvin == cat.ParentId);
                var parentSlug = parentCat != null ? parentCat.RewriteUrl : string.Empty;

                _writer.WriteRow("A", rowIndex, new List<string>
                {
                    cat.RewriteUrl,
                    parentSlug,
                    cat.Name
                });

                rowIndex++;

                return rowIndex;
            }
        }

        #endregion
    }
}