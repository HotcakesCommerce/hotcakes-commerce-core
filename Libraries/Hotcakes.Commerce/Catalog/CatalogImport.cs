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
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;
using DocumentFormat.OpenXml.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.OpenXml;

// TODO: Localize this class

namespace Hotcakes.Commerce.Catalog
{
    [Serializable]
	public class CatalogImport
	{
		#region Fields

        private readonly HotcakesApplication _hccApp;

		#endregion

		#region Constructor

		public CatalogImport(HotcakesApplication hccApp)
		{
			_hccApp = hccApp;
		}

		#endregion

		#region Public methods

		public void ImportFromExcel(string fileName, Action<double, string> log)
		{
			try
			{
				var reader = new ExcelReader(fileName);
				var mainImport = new MainSheetImport(reader, _hccApp, log);
				mainImport.ImagesImportPath = ImagesImportPath;
				mainImport.UpdateExistingProducts = UpdateExistingProducts;

				if (mainImport.Process(0, 0.20))
				{
					var products = mainImport.Products;

					var catTreeImport = new CategoryTreeSheetImport(reader, _hccApp, log);
					catTreeImport.Products = products;
					catTreeImport.Process(0.20, 0.05);

					var catsImport = new CategoriesSheetImport(reader, _hccApp, log);
					catsImport.Products = products;
					catsImport.Process(0.25, 0.15);

					var choicesImport = new ChoicesSheetImport(reader, _hccApp, log);
					choicesImport.Products = products;
					choicesImport.Process(0.40, 0.20);

					var tabsImport = new InfoTabsSheetImport(reader, _hccApp, log);
					tabsImport.Products = products;
					tabsImport.Process(0.60, 0.20);

					var propsImport = new TypePropertiesSheetImport(reader, _hccApp, log);
					propsImport.Products = products;
					propsImport.Process(0.80, 0.19);

					mainImport.Log("Import completed.", 1);
				}
				else
				{
					mainImport.Log("Import failed.", 1);
				}
			}
			catch (Exception ex)
			{
				EventLog.LogEvent(ex);
                log(1, "Import failed. Source file protected or corrupted.");
			}
		}

			#endregion

			#region Properties

        public bool UpdateExistingProducts { get; set; }
        public string ImagesImportPath { get; set; }

			#endregion

        #region Internal declarations

        internal abstract class WorksheetImport
        {
			internal WorksheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
			{
				_reader = reader;
				_hccApp = hccApp;
				_log = log;
			}

			internal bool Process(double startProgress, double progressRange)
			{
				var systemLog = Factory.CreateEventLogger();
				Log(string.Format("Reading worksheet '{0}'...", SheetName), startProgress);

				if (!_reader.SetWorksheet(SheetName))
				{
					Log(string.Format("Worksheet '{0}' does not exist.", SheetName), startProgress);
					return false;
				}

                var rowCount = _reader.GetRowCount(SheetName);
                var rowIndex = (uint) HeaderRowsCount + 1;

				while (rowIndex <= rowCount)
				{
					try
					{
						var row = _reader.GetRow(rowIndex++);
						if (row == null)
						{
							Log(string.Format("Row {0} is missing or empty.", rowIndex));
						}
						else if (ProcessRow(row))
						{
							_importedRows++;
						}
					}
					catch (DbEntityValidationException ex)
					{
						// Retrieve the error messages as a list of strings.
						var errorMessages = ex.EntityValidationErrors
								.SelectMany(x => x.ValidationErrors)
								.Select(x => x.PropertyName + " : " + x.ErrorMessage);

						// Join the list to a single string.
						var fullErrorMessage = string.Join(",", errorMessages);

						Log(string.Format("- Row {0} thrown validation: {1}", rowIndex, fullErrorMessage));
					}
					catch (Exception ex)
					{
						Log(string.Format("- Row {0} thrown exception: {1}", rowIndex, ex.Message));
						systemLog.LogException(ex);
					}

                    LogProgress(startProgress + progressRange*rowIndex/rowCount);
				}
				ProcessAfter();
				Log(string.Format("{0} rows successfully imported.", _importedRows));
				if (rowCount - HeaderRowsCount > _importedRows)
				{
					Log(string.Format("{0} rows failed import.", rowCount - HeaderRowsCount - _importedRows));
				}
				Log("----------------------------------------");

                return _importedRows > 0;
			}

			protected virtual void ProcessAfter()
            {
            }

			protected abstract bool ProcessRow(Row row);

            #region Fields

            protected HotcakesApplication _hccApp;
            protected Action<double, string> _log;
            protected ExcelReader _reader;
            protected List<string> _cleanedProducts = new List<string>();
            private double _progress;
            private int _importedRows;

            #endregion

            #region Properties

            protected abstract int HeaderRowsCount { get; }
            protected abstract string SheetName { get; }
            internal Dictionary<string, Product> Products { get; set; }

            #endregion

			#region Implementation

			protected void LogProgress(double progress)
			{
				_progress = progress;
				_log(progress, null);
			}

			internal void Log(string message, double? progress = null)
			{
				if (progress.HasValue) _progress = progress.Value;
				_log(_progress, message);
			}

			protected string GetCell(Row row, string column)
			{
				return _reader.GetCellValue(row, column) ?? "";
			}

			protected bool GetCellBool(Row row, string column)
			{
				var val = GetCell(row, column).ToUpper();
				return val == "TRUE" || val == "YES" || val == "1";
			}

			protected bool? GetCellNullableBool(Row row, string column)
			{
				var val = GetCell(row, column).ToUpper();
				if (string.IsNullOrWhiteSpace(val))
					return null;
				return val == "TRUE" || val == "YES" || val == "1";
			}

			protected decimal GetCellCurrency(Row row, string column)
			{
				var val = GetCell(row, column);
				decimal price = 0;

                if (!string.IsNullOrEmpty(val))
				{
					val = Regex.Replace(val, @"[^\d\.,]", "");
				}

				if (!decimal.TryParse(val, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture, out price))
				{
					return 0;
				}
				return Money.RoundCurrency(price);
			}

			protected decimal GetCellDecimal(Row row, string column)
			{
				var val = GetCell(row, column);
				decimal dVal = 0;
				if (!decimal.TryParse(val, NumberStyles.Float, Thread.CurrentThread.CurrentUICulture, out dVal))
				{
					return 0;
				}
				return dVal;
			}

			protected string GetNextColumn(string colname)
			{
				return SpreadsheetReader.GetColumnName(colname, 1);
			}

			#endregion
		}

        [Serializable]
        internal class MainSheetImport : WorksheetImport
		{
			internal MainSheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
				: base(reader, hccApp, log)
			{
				Products = new Dictionary<string, Product>();
				_vendors = _hccApp.ContactServices.Vendors.FindAll();
				_manufacturers = _hccApp.ContactServices.Manufacturers.FindAll();
				_productTypes = _hccApp.CatalogServices.ProductTypes.FindAllForStore(hccApp.CurrentStore.Id);
				_allRoles = _hccApp.MembershipServices.GetAllRoles();
				_catalogRoles = _hccApp.CatalogServices.CatalogRoles.FindAllPaged(1, int.MaxValue);
				_productInventoryList = _hccApp.CatalogServices.ProductInventories.FindAllPaged(1, int.MaxValue);
				_taxScheduleList = _hccApp.OrderServices.TaxSchedules.FindAllPaged(1, int.MaxValue);

				_storeProductsCount = _hccApp.CatalogServices.Products.CountOfAll();
			}

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();
				var sku = GetCell(row, "D").Trim();
                slug = Hotcakes.Web.Text.Slugify(slug, true);
                var p = _hccApp.CatalogServices.Products.FindBySlug(slug);

				if (p == null)
				{
					if (_hccApp.CatalogServices.Products.IsSkuExist(sku))
					{
                        Log(string.Format("- Product SKU - {0} already exists for different slug (row: {1}).", sku,
                            row.RowIndex));
						return false;
					}

                    // create a new product since it doesn't already exist
                    // make new products searchable by default
                    p = new Product {IsSearchable = true};
				}
				else if (!UpdateExistingProducts)
				{
					Log(string.Format("- Product slug - {0} already exists (row: {1}).", slug, row.RowIndex));
					return false;
				}

				p.UrlSlug = slug;
				p.Status = GetCellBool(row, "B") ? ProductStatus.Active : ProductStatus.Disabled;
				p.Featured = GetCellBool(row, "C");
				p.Sku = sku;
				p.ProductName = GetCell(row, "E").Trim();
				p.ProductTypeId = GetProductTypeId(GetCell(row, "F"));
				p.ListPrice = GetCellCurrency(row, "G");
				p.SiteCost = GetCellCurrency(row, "H");
				p.SitePrice = GetCellCurrency(row, "I");
				p.ManufacturerId = GetManufacturerId(GetCell(row, "J"));
				p.VendorId = GetVendorId(GetCell(row, "K"));

                var imageFilename = GetCell(row, "L");
				if (!string.IsNullOrWhiteSpace(imageFilename))
				{
					p.ImageFileSmall = imageFilename;
					p.ImageFileSmallAlternateText = p.ProductName + " " + p.Sku;
					p.ImageFileMedium = imageFilename;
					p.ImageFileMediumAlternateText = p.ProductName + " " + p.Sku;
				}
				p.LongDescription = GetCell(row, "M");
	
				p.Keywords = GetCell(row, "N");
				p.MetaTitle = GetCell(row, "O");
				p.MetaDescription = GetCell(row, "P");
				p.MetaKeywords = GetCell(row, "Q");
				
				p.TaxSchedule = GetTaxScheduleId(GetCell(row, "R"));
				p.TaxExempt = GetCellBool(row, "S");
				
				p.ShippingDetails.Weight = GetCellDecimal(row, "T");
				p.ShippingDetails.Length = GetCellDecimal(row, "U");
				p.ShippingDetails.Width = GetCellDecimal(row, "V");
				p.ShippingDetails.Height = GetCellDecimal(row, "W");
				p.ShippingDetails.ExtraShipFee = GetCellCurrency(row, "X");
				p.ShippingMode = GetShippingMode(GetCell(row, "Y"));
				p.ShippingDetails.IsNonShipping = GetCellBool(row, "Z");
				p.ShippingDetails.ShipSeparately = GetCellBool(row, "AA");
				
				p.AllowReviews = GetCellNullableBool(row, "AB");
                p.MinimumQty = (int) GetCellDecimal(row, "AC");
				
				p.InventoryMode = GetInventoryMode(GetCell(row, "AD"));
                var roles = GetCell(row, "AH");

			    p.IsSearchable = GetCellBool(row, "AI");

				if (string.IsNullOrWhiteSpace(p.ProductName))
				{
					Log(string.Format("- Product name is empty for row {0}", row.RowIndex));
					return false;
				}

                var result = false;

				if (string.IsNullOrEmpty(p.Bvin))
				{
					result = _hccApp.CatalogServices.ProductsCreateWithInventory(p, true);
					if (result)
						_storeProductsCount++;
				}
				else
				{
					result = _hccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);
				}

				if (result)
				{
                    var quantity = (int) GetCellDecimal(row, "AE");
                    var stockOut = (int) GetCellDecimal(row, "AF");
                    var lowStock = (int) GetCellDecimal(row, "AG");
					
                    var inv =
                        _productInventoryList.Where(y => y.ProductBvin == p.Bvin && y.StoreId == _hccApp.CurrentStore.Id)
                            .FirstOrDefault();
					if (inv != null)
					{
						inv.QuantityOnHand = quantity;
						inv.OutOfStockPoint = stockOut;
						inv.LowStockPoint = lowStock;

						result = _hccApp.CatalogServices.ProductInventories.Update(inv);
						_hccApp.CatalogServices.UpdateProductVisibleStatusAndSave(p);

						if (!result)
							Log(string.Format("- Product inventory update failed for product '{0}'.", p.ProductName));
					}
					
					var imageImportResult = ImportImageFile(p.Bvin, imageFilename);
                    if (imageImportResult)
                    {
                        var altText = string.Format("{0} {1}", p.ProductName, p.Sku);
                        var fileName = Hotcakes.Web.Text.CleanFileName(imageFilename);
                        p.ImageFileMedium = fileName;
                        p.ImageFileMediumAlternateText = altText;
                        p.ImageFileSmall = fileName;
                        p.ImageFileSmallAlternateText = altText;
                        _hccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);
                    }

					ImportRoles(new Guid(p.Bvin), roles);
				}
				else
				{
					Log(string.Format("- Product '{0}' creation failed.", p.ProductName));
				}

				if (result)
				{
					Products.Add(p.UrlSlug, p);
				}
				
				return result;
			}

			private void ImportRoles(Guid prodId, string roles)
			{
				if (!string.IsNullOrEmpty(roles))
				{
					var currRoles = _catalogRoles.Where(r => r.ReferenceId == prodId);
					var arr = roles.Split(',');

					foreach (var role in arr)
					{
						var roleTrimmed = role.Trim();

						if (_allRoles.Contains(roleTrimmed))
						{
							if (!currRoles.Any(i => i.RoleName == roleTrimmed))
							{
								var cr = new CatalogRole
								{
									ReferenceId = prodId,
									RoleName = roleTrimmed,
									RoleType = CatalogRoleType.ProductRole
								};

								var result = _hccApp.CatalogServices.CatalogRoles.Create(cr);
								if (result)
								{
									_catalogRoles.Add(cr);
								}
							}
						}
						else
						{
							Log(string.Format("- Role '{0}' does not exist in the CMS and was ignored.", roleTrimmed));
						}
					}
				}
			}

			private bool ImportImageFile(string productId, string filename)
			{
                var importPath = ImagesImportPath;
			    if (string.IsNullOrEmpty(importPath))
			    {
			        importPath = DiskStorage.GetStoreDataVirtualPath(_hccApp.CurrentStore.Id, "import/");
			    }
                
				var filePath = importPath + filename;
			    var mappedPath = HostingEnvironment.MapPath(filePath);

                if (File.Exists(mappedPath))
                {
                    return DiskStorage.CopyProductImage(_hccApp.CurrentStore.Id, productId, filePath, filename);
				}

                return false;
			}

            #region Properties

            public bool UpdateExistingProducts { get; set; }
            public string ImagesImportPath { get; set; }

            protected override int HeaderRowsCount
            {
                get { return 2; }
            }

            protected override string SheetName
            {
                get { return "Main"; }
            }

            private readonly List<VendorManufacturer> _vendors;
            private readonly List<VendorManufacturer> _manufacturers;
            private readonly List<ProductType> _productTypes;
            private readonly List<string> _allRoles;
            private readonly List<CatalogRole> _catalogRoles;
            private readonly List<ProductInventory> _productInventoryList;
            private readonly List<TaxSchedule> _taxScheduleList;
            private int _storeProductsCount;

            #endregion

			#region Implementation

			private ProductInventoryMode GetInventoryMode(string val)
			{
				if (!string.IsNullOrWhiteSpace(val))
				{
					val = val.ToLower().Trim().Replace(" ", "");

					if (val.Contains("hide") || val.Contains("remove"))
					{
						return ProductInventoryMode.WhenOutOfStockHide;
					}
					if (val.Contains("show"))
					{
						return ProductInventoryMode.WhenOutOfStockShow;
					}
					if (val.Contains("allow"))
					{
						return ProductInventoryMode.WhenOutOfStockAllowBackorders;
					}
				}

				return ProductInventoryMode.AlwayInStock;
			}

            private ShippingMode GetShippingMode(string val)
			{
				if (!string.IsNullOrWhiteSpace(val))
				{
					val = val.ToLower().Trim().Replace(" ", "");

					if (val.Contains("manufacturer"))
					{
                        return ShippingMode.ShipFromManufacturer;
					}
					if (val.Contains("vendor"))
					{
                        return ShippingMode.ShipFromVendor;
					}
				}

                return ShippingMode.ShipFromSite;
			}

			private long GetTaxScheduleId(string val)
			{
                var taxSchedule =
                    _taxScheduleList.Where(y => y.StoreId == _hccApp.CurrentStore.Id && y.Name == val).FirstOrDefault();
				return taxSchedule != null ? taxSchedule.Id : -1;
			}

			private string GetVendorId(string val)
			{
				if (string.IsNullOrEmpty(val))
					return string.Empty;

                var vendor =
                    _vendors.FirstOrDefault(m => m.DisplayName.Equals(val, StringComparison.CurrentCultureIgnoreCase));

				if (vendor == null)
				{
                    vendor = new VendorManufacturer();
					vendor.DisplayName = val;
					var result = _hccApp.ContactServices.Vendors.Create(vendor);
					if (result)
					{
						_vendors.Add(vendor);
					}
				}

				return vendor.Bvin;
			}

			private string GetManufacturerId(string val)
			{
				if (string.IsNullOrEmpty(val))
					return string.Empty;

                var manuf =
                    _manufacturers.FirstOrDefault(
                        m => m.DisplayName.Equals(val, StringComparison.CurrentCultureIgnoreCase));

				if (manuf == null)
				{
                    manuf = new VendorManufacturer();
					manuf.DisplayName = val;
					var result = _hccApp.ContactServices.Manufacturers.Create(manuf);
					if (result)
					{
						_manufacturers.Add(manuf);
					}
				}

				return manuf.Bvin;
			}

			private string GetProductTypeId(string val)
			{
				var types = _productTypes.Where(y => y.ProductTypeName == val);
				return types.Any() ? types.First().Bvin : string.Empty;
			}

			#endregion
		}

        [Serializable]
        internal class CategoryTreeSheetImport : WorksheetImport
		{
			internal CategoryTreeSheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
				: base(reader, hccApp, log)
			{
				CachedCategories = hccApp.CatalogServices.Categories.FindAllPaged(1, int.MaxValue);
				PendingCategories = new List<CategoryInfo>();
			}

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();
                slug = Hotcakes.Web.Text.Slugify(slug, true);

				if (!PendingCategories.Any(c => c.Slug == slug))
				{
					var ci = new CategoryInfo();
					ci.Slug = slug;
                    ci.ParentSlug = Hotcakes.Web.Text.Slugify(GetCell(row, "B"), true);
					ci.CategoryName = GetCell(row, "C");

					PendingCategories.Add(ci);
				}

				return true;
			}

			protected override void ProcessAfter()
			{
				var tree = OrganizeCategoryTree();
				AddMissingCategories(tree);
			}

			private List<CategoryInfo> OrganizeCategoryTree()
			{
				var rootCategories = new List<CategoryInfo>();
                var i = PendingCategories.Count - 1;

				while (i >= 0)
				{
					var ci = PendingCategories[i];
					var parent = PendingCategories.FirstOrDefault(pc => pc.Slug == ci.ParentSlug);

					if (parent != null)
					{
						parent.Children.Add(ci);
					}
					else
					{
						rootCategories.Add(ci);
					}

					i--;
				}

				return rootCategories;
			}

			private void AddMissingCategories(List<CategoryInfo> catsTree)
			{
				foreach (var ci in catsTree)
				{
					var cat = new Category
					{
						RewriteUrl = ci.Slug,
						Name = ci.CategoryName
					};

					var parent = CachedCategories.FirstOrDefault(c => c.RewriteUrl == ci.ParentSlug);
					if (parent != null)
					{
						cat.ParentId = parent.Bvin;
					}

					var existCat = CachedCategories.FirstOrDefault(c => c.RewriteUrl == ci.Slug);
					if (existCat == null)
					{
						_hccApp.CatalogServices.Categories.Create(cat);
						CachedCategories.Add(cat);
					}
					else
					{
						existCat.Name = cat.Name;
						existCat.ParentId = cat.ParentId;
						_hccApp.CatalogServices.CategoryUpdate(existCat);
					}

						AddMissingCategories(ci.Children);
				}
            }

            internal class CategoryInfo
            {
                public CategoryInfo()
                {
                    Children = new List<CategoryInfo>();
                }

                public string Slug { get; set; }
                public string ParentSlug { get; set; }
                public string CategoryName { get; set; }
                public List<CategoryInfo> Children { get; set; }
            }

            #region Properties

            protected override int HeaderRowsCount
            {
                get { return 1; }
            }

            protected override string SheetName
            {
                get { return "Category Tree"; }
			}

            internal List<Category> CachedCategories { get; set; }
            internal List<CategoryInfo> PendingCategories { get; set; }

            #endregion
		}

        [Serializable]
        internal class CategoriesSheetImport : WorksheetImport
		{
			internal CategoriesSheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
				: base(reader, hccApp, log)
			{
				_categoryList = _hccApp.CatalogServices.Categories.FindAllPaged(1, int.MaxValue);
			}

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();
                slug = Hotcakes.Web.Text.Slugify(slug, true);

				if (!Products.ContainsKey(slug))
				{
					Log(string.Format("- Row {0} has unknown slug {1}", row.RowIndex, slug));
					return false;
				}
				var p = Products[slug];

				// clean up original categories
				if (!_cleanedProducts.Contains(slug))
				{
					_hccApp.CatalogServices.CategoriesXProducts.DeleteAllForProduct(p.Bvin);
					_cleanedProducts.Add(slug);
				}

                var column = "B";

				while (column.Length < 10)
				{
                    var catSlug = GetCell(row, column);
                    var catSlugFixed = Hotcakes.Web.Text.Slugify(catSlug, true);

					if (string.IsNullOrWhiteSpace(catSlugFixed))
					{
						break;
					}

                    var cat =
                        _categoryList.Where(y => y.RewriteUrl == catSlugFixed && y.StoreId == _hccApp.CurrentStore.Id)
                            .OrderBy(y => y.SortOrder)
                            .FirstOrDefault();

					if (cat == null)
					{
						cat = new Category();
						cat.RewriteUrl = catSlugFixed;
						cat.Name = catSlug.Replace("-", " ");

						_hccApp.CatalogServices.Categories.Create(cat);
					}
					if (cat != null)
					{
						_hccApp.CatalogServices.AddProductToCategory(p.Bvin, cat.Bvin);
					}

					column = GetNextColumn(column);
				}

				return true;
			}

            #region Properties

            protected override int HeaderRowsCount
            {
                get { return 1; }
            }

            protected override string SheetName
            {
                get { return "Categories"; }
            }

            private readonly List<Category> _categoryList;

            #endregion
		}

        [Serializable]
        internal class ChoicesSheetImport : WorksheetImport
		{
            private List<Option> options;
            private string _previousSlug;

			internal ChoicesSheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
				: base(reader, hccApp, log)
			{
				options = _hccApp.CatalogServices.ProductOptions.FindAll(0, int.MaxValue);
			}

            protected override int HeaderRowsCount
            {
                get { return 1; }
            }

            protected override string SheetName
            {
                get { return "Choices"; }
            }

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();

                slug = string.IsNullOrWhiteSpace(slug) ? _previousSlug : Hotcakes.Web.Text.Slugify(slug, true);

				if (!string.IsNullOrEmpty(slug))
				{
					_previousSlug = slug;
				}

				if (!Products.ContainsKey(slug))
				{
					Log(string.Format("- Row {0} has unknown slug {1}", row.RowIndex, slug));
					return false;
				}
				var p = Products[slug];

				// refresh shared options
				if (!_cleanedProducts.Contains(slug))
				{
					RefreshSharedOptions(p);
					_cleanedProducts.Add(slug);
				}

				var choiceName = GetCell(row, "B");
				if (string.IsNullOrWhiteSpace(choiceName))
				{
					Log(string.Format("- Row {0} has empty choice name", row.RowIndex));
					return false;
				}
				var optionType = GetOptionType(GetCell(row, "C"));
                var opt =
                    p.Options.FirstOrDefault(e => e.Name == choiceName && (!e.IsShared || e.OptionType == optionType));

				if (opt != null)
				{
					p.Options.RemoveAll(e => e.Bvin == opt.Bvin);
					opt = options.Where(y => y.Bvin == opt.Bvin).FirstOrDefault();
                }

				var shared = GetCellBool(row, "D");
				if (shared && opt == null)
				{
					opt = options.FirstOrDefault(y => y.StoreId == _hccApp.CurrentStore.Id
									&& y.IsShared
                                                      &&
                                                      y.Name.Equals(choiceName,
                                                          StringComparison.CurrentCultureIgnoreCase)
									&& y.OptionType == optionType);
				}

				var isNew = opt == null;
                var processedOptionItems = new List<string>();

                if (isNew)
				{
                    opt = new Option {Name = choiceName, IsShared = shared};
                
				    opt.SetProcessor(optionType);
                    var column = "E";
				    while (column.Length < 10)
				    {
                        var choiceItem = GetCell(row, column);

					    if (string.IsNullOrWhiteSpace(choiceItem))
					    {
						    break;
					    }

					    if (opt.OptionType == OptionTypes.Html)
					    {
						    opt.TextSettings.AddOrUpdate("html", choiceItem);
					    }
					    else
					    {
						    if (opt.Items.All(e => e.Name != choiceItem))
						    {
							    opt.AddItem(choiceItem);
						    }
						    processedOptionItems.Add(choiceItem);
					    }

					    column = GetNextColumn(column);
				    }

                    _hccApp.CatalogServices.ProductOptions.Create(opt);
                    options = _hccApp.CatalogServices.ProductOptions.FindAll(0, int.MaxValue);
                }

				if (!isNew && !opt.IsShared)
				{
					opt.Items.RemoveAll(e => !processedOptionItems.Contains(e.Name));
				}


				p.Options.Add(opt);
				//_hccApp.CatalogServices.Products.Update(p);

				if (opt.IsShared)
				{
					_hccApp.CatalogServices.ProductsXOptions.AddOptionToProduct(p.Bvin, opt.Bvin);
				}
				

				return true;
			}

			#region Implementation

			private void RefreshSharedOptions(Product product)
			{
				var sharedOptions = product.Options.Where(e => e.IsShared).Select(e => e.Bvin).ToList();
				product.Options.RemoveAll(e => e.IsShared);
				foreach (var sharedOption in sharedOptions)
				{
					var currentOption = options.Where(y => y.Bvin == sharedOption).FirstOrDefault();
					product.Options.Add(currentOption);
				}
			}

			private OptionTypes GetOptionType(string val)
			{
				if (!string.IsNullOrWhiteSpace(val))
				{
					val = val.ToLower().Trim().Replace(" ", "");

					if (val.Contains("radiobutton"))
					{
						return OptionTypes.RadioButtonList;
					}
					if (val.Contains("checkbox"))
					{
						return OptionTypes.CheckBoxes;
					}
					if (val.Contains("html"))
					{
						return OptionTypes.Html;
					}
					if (val.Contains("text"))
					{
						return OptionTypes.TextInput;
					}
					if (val.Contains("file"))
					{
						return OptionTypes.FileUpload;
					}
				}

				return OptionTypes.DropDownList;
			}

			#endregion
		}

        [Serializable]
        internal class InfoTabsSheetImport : WorksheetImport
		{
            private string _previousSlug;

			internal InfoTabsSheetImport(ExcelReader reader, HotcakesApplication hccApp, Action<double, string> log)
				: base(reader, hccApp, log)
			{
            }

            protected override int HeaderRowsCount
            {
                get { return 1; }
            }

            protected override string SheetName
            {
                get { return "Info Tabs"; }
			}

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();

                slug = string.IsNullOrWhiteSpace(slug)
                    ? _previousSlug
                    : Hotcakes.Web.Text.Slugify(slug, true);

				if (!Products.ContainsKey(slug))
				{
					Log(string.Format("- Row {0} has unknown slug {1}", row.RowIndex, slug));
					return false;
				}
				var p = Products[slug];

				// clean up original tabs
				if (!_cleanedProducts.Contains(slug))
				{
					p.Tabs.Clear();
					_cleanedProducts.Add(slug);
				}

				var tabName = GetCell(row, "B");
				if (string.IsNullOrWhiteSpace(tabName))
				{
					Log(string.Format("- Row {0} has empty tab name", row.RowIndex));
					return false;
				}

				var tab = new ProductDescriptionTab();
				tab.Bvin = Guid.NewGuid().ToString("D");
				tab.TabTitle = tabName;
				tab.HtmlData = GetCell(row, "C");

				if (p.Tabs.Count > 0)
				{
					var m = (from sort in p.Tabs select sort.SortOrder).Max();
					tab.SortOrder = m + 1;
				}
				else
				{
					tab.SortOrder = 1;
				}

				p.Tabs.Add(tab);
				_hccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);

				_previousSlug = slug;

				return true;
			}

			#region Implementation

			#endregion
		}
        
        [Serializable]
        internal class TypePropertiesSheetImport : WorksheetImport
		{
            private readonly List<ProductProperty> _allProductProperties;
            private readonly List<ProductTypePropertyAssociation> _crosses;
            private string _previousSlug;
            private List<ProductProperty> _properties;

            internal TypePropertiesSheetImport(ExcelReader reader, HotcakesApplication hccApp,
                Action<double, string> log)
				: base(reader, hccApp, log)
			{
				_crosses = _hccApp.CatalogServices.ProductTypesXProperties.FindAllPaged(1, int.MaxValue);
				_allProductProperties = _hccApp.CatalogServices.ProductProperties.FindAll();
            }

            protected override int HeaderRowsCount
            {
                get { return 1; }
            }

            protected override string SheetName
            {
                get { return "Type Properties"; }
			}

			protected override bool ProcessRow(Row row)
			{
				var slug = GetCell(row, "A").Trim();

                slug = string.IsNullOrWhiteSpace(slug)
                    ? _previousSlug
                    : Hotcakes.Web.Text.Slugify(slug, true);

                var initProductProperties = slug != _previousSlug;
				if (!string.IsNullOrEmpty(slug))
				{
					_previousSlug = slug;
				}

				if (!Products.ContainsKey(slug))
				{
					Log(string.Format("- Row {0} has unknown slug {1}", row.RowIndex, slug));
					return false;
				}
				var p = Products[slug];

				if (initProductProperties)
				{
                    var ids = _crosses
							.Where(y => y.ProductTypeBvin == p.ProductTypeId)
							.OrderBy(y => y.SortOrder)
							.Select(y => y.PropertyId)
							.ToList();

					_properties = _allProductProperties
									.Where(y => ids.Contains(y.Id))
									.OrderBy(y => y.PropertyName).ToList();
				}

				// clean up original tabs
				if (!_cleanedProducts.Contains(slug))
				{
					_cleanedProducts.Add(slug);
				}

				var propName = GetCell(row, "B");
				if (string.IsNullOrWhiteSpace(propName))
				{
					Log(string.Format("- Row {0} has empty property name", row.RowIndex));
					return false;
				}

				var prop = _properties.FirstOrDefault(pr => pr.DisplayName.Trim().ToLower() == propName.Trim().ToLower());
				if (prop == null)
				{
					Log(string.Format("- Row {0} has unknown property name: {1}", row.RowIndex, propName));
					return false;
				}

				var propVal = GetCell(row, "C");

				if (prop.TypeCode == ProductPropertyType.MultipleChoiceField)
				{
					propVal = prop.GetChoiceValue(propVal);
				}
				_hccApp.CatalogServices.ProductPropertyValues.SetPropertyValue(p.Bvin, prop, propVal);

				
				return true;
			}

			#region Implementation

			#endregion
		}

		#endregion
	}
}
