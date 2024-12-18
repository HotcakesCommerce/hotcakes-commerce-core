using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
	public abstract class BaseProductTest : BaseTest
	{
		/// <summary>
		/// The _irepocategory
		/// </summary>
		protected IXmlProductRepository _irepoproduct;

		/// <summary>
		/// Creates the product.
		/// </summary>
		//[TestMethod]
		public void CreateProduct()
		{
			#region Arrange
			//Arrange
			var product = _irepoproduct.GetAddProduct();

			var skuCheckProduct = _application.CatalogServices.Products.FindBySku(product.Sku);
			if (skuCheckProduct != null) Assert.Fail();

			var column = _applicationDB.ContentServices.Columns.FindAll();
			var col1 = column.FirstOrDefault(x => x.DisplayName.Equals(product.PreContentColumnId));
			var col2 = column.FirstOrDefault(x => x.DisplayName.Equals(product.PostContentColumnId));

			var type = _applicationDB.CatalogServices.ProductTypes.FindAll().FirstOrDefault(x => x.ProductTypeName.Equals(product.ProductTypeId));
			var menufacturer = _applicationDB.ContactServices.Manufacturers.FindAll().FirstOrDefault(x => x.DisplayName.Equals(product.ManufacturerId));
			var vendor = _applicationDB.ContactServices.Vendors.FindAll().FirstOrDefault(x => x.DisplayName.Equals(product.VendorId));

			var newproduc = product;
			{
				if (product.IsUserSuppliedPrice)
				{
					newproduc.ListPrice = 0;
					newproduc.SiteCost = 0;
					newproduc.SitePrice = 0;
					newproduc.SitePriceOverrideText = "";
				}
				if (string.IsNullOrEmpty(product.ImageFileSmallAlternateText))
					newproduc.ImageFileSmallAlternateText = product.ProductName + " " + product.Sku;
				if (string.IsNullOrEmpty(product.ImageFileMediumAlternateText))
					newproduc.ImageFileMediumAlternateText = product.ProductName + " " + product.Sku;
				newproduc.GiftWrapAllowed = false;

				newproduc.ManufacturerId = menufacturer == null ? string.Empty : menufacturer.Bvin;
				newproduc.VendorId = vendor == null ? string.Empty : vendor.Bvin;
				newproduc.ProductTypeId = type == null ? string.Empty : type.Bvin;
				newproduc.PreContentColumnId = col1 == null ? string.Empty : col1.Bvin;
				newproduc.PostContentColumnId = col2 == null ? string.Empty : col2.Bvin;
			};

			if (UrlRewriter.IsProductSlugInUse(newproduc.UrlSlug, newproduc.Bvin, _application))
				Assert.Fail();
			#endregion


			//var result = _application.CatalogServices.ProductsCreateWithInventory(newproduc, true);
			//return;


			#region Act
			//Act
			var result = _application.CatalogServices.ProductsCreateWithInventory(newproduc, true);
			//TODO: Need to change EnsureJournalType function for CI

			var resultprj = _application.CatalogServices.Products.FindBySku(product.Sku);
			if (resultprj == null) Assert.Fail();
			var productproptype = _irepoproduct.GetAddProductPropertyValue();
			var resultprops = new List<ProductProperty>();
			if (type != null && result && productproptype.Count > 0)
			{
				var props = _applicationDB.CatalogServices.ProductPropertiesFindForType(type.Bvin);

				foreach (var productProperty in props)
				{
					switch (productProperty.TypeCode)
					{
						case ProductPropertyType.DateField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(resultprj.Bvin, productProperty,
																								productproptype[ProductPropertyType.DateField]);
							break;
						case ProductPropertyType.CurrencyField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(resultprj.Bvin, productProperty,
																								productproptype[ProductPropertyType.CurrencyField]);
							break;
						case ProductPropertyType.TextField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(resultprj.Bvin, productProperty,
																								productproptype[ProductPropertyType.TextField]);
							break;
						case ProductPropertyType.MultipleChoiceField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(resultprj.Bvin, productProperty,
																								productproptype[ProductPropertyType.MultipleChoiceField]);
							break;
					}
				}
				resultprops = _applicationDB.CatalogServices.ProductPropertiesFindForType(type.Bvin);
			}


			var taxonomyTags = _irepoproduct.GetProductTaxonomyTags().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			_application.SocialService.UpdateProductTaxonomy(resultprj, taxonomyTags);
			var resulttex = _application.SocialService.GetTaxonomyTerms(resultprj);
			//TODO:Need to confirm taxonomy update functionality not implemented 
			#endregion

			#region Assert
			//Assert
			Assert.IsTrue(result);
			foreach (var productProperty in resultprops)
			{
				switch (productProperty.TypeCode)
				{
					case ProductPropertyType.DateField:
						Assert.AreEqual(productproptype[ProductPropertyType.DateField],
										_application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.CurrencyField:
						Assert.AreEqual(productproptype[ProductPropertyType.CurrencyField],
									  _application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.TextField:
						Assert.AreEqual(productproptype[ProductPropertyType.TextField],
									_application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.MultipleChoiceField:
						Assert.AreEqual(productproptype[ProductPropertyType.MultipleChoiceField],
								   _application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
				}
			}
			// Assert.AreEqual(taxonomyTags.Count(), resulttex.Count());
			#endregion

		}

		/// <summary>
		/// Clones the product.
		/// </summary>
		//[TestMethod]
		public void CloneProduct()
		{
			//TODO:Need to update test case as per new development
			//Arrange
			var prjinfo = _irepoproduct.GetCloneProductInfo();
			var dic = prjinfo.FirstOrDefault().Value;
			var prj = _application.CatalogServices.Products.FindBySku(prjinfo.FirstOrDefault().Key);

			if (prj == null) Assert.Fail();

			var existing = _application.CatalogServices.Products.FindBySku(Convert.ToString(dic["Sku"]));
			if (existing != null) Assert.Fail();

			#region Act
			//Act
			var newProduct = prj.Clone(Convert.ToBoolean(dic["CloneProductChoices"]), Convert.ToBoolean(dic["CloneProductImages"]));
			newProduct.Status = (ProductStatus)Convert.ToInt32(dic["CreateAsInactive"]);
			newProduct.ProductName = Convert.ToString(dic["ProductName"]);
			newProduct.Sku = Convert.ToString(dic["Sku"]);
			if (!_application.CatalogServices.ProductsCreateWithInventory(newProduct, true)) Assert.Fail();

			if (prj.ProductTypeId != string.Empty)
			{
				var productTypes = _application.CatalogServices.ProductPropertiesFindForType(prj.ProductTypeId);
				foreach (var item in productTypes)
				{
					var value = _application.CatalogServices.ProductPropertyValues.GetPropertyValue(prj.Bvin, item);
					_application.CatalogServices.ProductPropertyValues.SetPropertyValue(newProduct.Bvin, item, value);
				}
			}

			if (Convert.ToBoolean(dic["CloneCategoryPlacement"]))
			{
				var cats = _application.CatalogServices.CategoriesXProducts.FindForProduct(prj.Bvin, 1, 100);
				foreach (var a in cats)
				{
					_application.CatalogServices.CategoriesXProducts.AddProductToCategory(newProduct.Bvin, a.CategoryId);
				}
			}

			var prj1 = _application.CatalogServices.Products.FindBySku(newProduct.Sku);
			#endregion

			//Assert
			Assert.AreNotEqual(null, prj1);

		}

		/// <summary>
		/// Edits the product.
		/// </summary>
		//[TestMethod]
		public void EditProduct()
		{
			#region Arrange
			var oldproductsku = _irepoproduct.GetEditProductSku();
			var product = _irepoproduct.GetEditProduct();

			var oldProduct = _application.CatalogServices.Products.FindBySku(oldproductsku);

			var column = _applicationDB.ContentServices.Columns.FindAll();
			var col1 = column.FirstOrDefault(x => x.DisplayName.Equals(product.PreContentColumnId));
			var col2 = column.FirstOrDefault(x => x.DisplayName.Equals(product.PostContentColumnId));

			var type =
				_applicationDB.CatalogServices.ProductTypes.FindAll()
							  .FirstOrDefault(x => x.ProductTypeName.Equals(product.ProductTypeId));
			var menufacturer =
				_applicationDB.ContactServices.Manufacturers.FindAll()
							  .FirstOrDefault(x => x.DisplayName.Equals(product.ManufacturerId));
			var vendor =
				_applicationDB.ContactServices.Vendors.FindAll()
							  .FirstOrDefault(x => x.DisplayName.Equals(product.VendorId));

			var edit = oldProduct;
			{
				edit.IsUserSuppliedPrice = product.IsUserSuppliedPrice;
				if (product.IsUserSuppliedPrice)
				{
					edit.ListPrice = 0;
					edit.SiteCost = 0;
					edit.SitePrice = 0;
					edit.SitePriceOverrideText = "";
				}
				else
				{
					edit.ListPrice = product.ListPrice;
					edit.SiteCost = product.SiteCost;
					edit.SitePrice = product.SitePrice;
					edit.SitePriceOverrideText = product.SitePriceOverrideText;
				}
				if (string.IsNullOrEmpty(product.ImageFileSmallAlternateText))
					edit.ImageFileSmallAlternateText = product.ProductName + " " + product.Sku;
				if (string.IsNullOrEmpty(product.ImageFileMediumAlternateText))
					edit.ImageFileMediumAlternateText = product.ProductName + " " + product.Sku;
				edit.GiftWrapAllowed = false;

				edit.ManufacturerId = menufacturer == null ? string.Empty : menufacturer.Bvin;
				edit.VendorId = vendor == null ? string.Empty : vendor.Bvin;
				edit.ProductTypeId = type == null ? string.Empty : type.Bvin;
				edit.PreContentColumnId = col1 == null ? string.Empty : col1.Bvin;
				edit.PostContentColumnId = col2 == null ? string.Empty : col2.Bvin;

				edit.TemplateName = product.TemplateName;
				edit.Featured = product.Featured;
				edit.IsBundle = product.IsBundle;
				edit.IsSearchable = product.IsSearchable;
				edit.AllowUpcharge = product.AllowUpcharge;
				edit.UpchargeAmount = product.UpchargeAmount;
				edit.UpchargeUnit = product.UpchargeUnit;
				edit.Status = product.Status;
				edit.Sku = product.Sku;
				edit.ProductName = product.ProductName;
				edit.UserSuppliedPriceLabel = product.UserSuppliedPriceLabel;
				edit.HideQty = product.HideQty;
				edit.UrlSlug = product.UrlSlug;
				edit.TaxSchedule = product.TaxSchedule;
				edit.Keywords = product.Keywords;
				edit.MetaKeywords = product.MetaKeywords;
				edit.MetaDescription = product.MetaDescription;
				edit.MetaTitle = product.MetaTitle;
				edit.SwatchPath = product.SwatchPath;
				edit.AllowReviews = product.AllowReviews;
				edit.TaxExempt = product.TaxExempt;
				edit.MinimumQty = product.MinimumQty;
				edit.ShippingDetails.ShipSeparately = product.ShippingDetails.ShipSeparately;
				edit.ShippingDetails.IsNonShipping = product.ShippingDetails.IsNonShipping;
				edit.ShippingDetails.ExtraShipFee = product.ShippingDetails.ExtraShipFee;
				edit.ShippingDetails.Height = product.ShippingDetails.Height;
				edit.ShippingDetails.Width = product.ShippingDetails.Width;
				edit.ShippingDetails.Length = product.ShippingDetails.Length;
				edit.ShippingDetails.Weight = product.ShippingDetails.Weight;
				edit.ShippingMode = product.ShippingMode;
				//edit.ImageFileSmall = product.ImageFileSmall;
				//edit.ImageFileMedium = product.ImageFileMedium;
			}
			if (UrlRewriter.IsProductSlugInUse(edit.UrlSlug, edit.Bvin, _application))
				Assert.Fail();
			#endregion

			#region Act
			//Act

			if (!edit.IsBundle)
				_application.CatalogServices.BundledProducts.DeleteAllForProduct(edit.Bvin);

			var result = _application.CatalogServices.ProductsUpdateWithSearchRebuild(edit);
			if (!result) Assert.Fail();

			var productproptype = _irepoproduct.GetAddProductPropertyValue();
			var resultprops = new List<ProductProperty>();

			if (type != null && productproptype.Count > 0)
			{
				var props = _applicationDB.CatalogServices.ProductPropertiesFindForType(type.Bvin);

				foreach (var productProperty in props)
				{
					switch (productProperty.TypeCode)
					{
						case ProductPropertyType.DateField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(edit.Bvin, productProperty,
																								productproptype[ProductPropertyType.DateField]);
							break;
						case ProductPropertyType.CurrencyField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(edit.Bvin, productProperty,
																								productproptype[ProductPropertyType.CurrencyField]);
							break;
						case ProductPropertyType.TextField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(edit.Bvin, productProperty,
																								productproptype[ProductPropertyType.TextField]);
							break;
						case ProductPropertyType.MultipleChoiceField:
							_application.CatalogServices.ProductPropertyValues.SetPropertyValue(edit.Bvin, productProperty,
																								productproptype[ProductPropertyType.MultipleChoiceField]);
							break;
					}
				}
				resultprops = _applicationDB.CatalogServices.ProductPropertiesFindForType(type.Bvin);
			}

			if (!string.IsNullOrEmpty(product.UrlSlug))
			{
				_application.ContentServices.CustomUrls.Register301(product.UrlSlug,
										edit.UrlSlug,
										edit.Bvin, CustomUrlType.Product, _application.CurrentRequestContext, _application);
			}

			var resultprj = _application.CatalogServices.Products.Find(edit.Bvin);
			#endregion

			#region Assert
			//Assert
			Assert.IsTrue(result);
			Assert.AreEqual(edit.UrlSlug, resultprj.UrlSlug);
			foreach (var productProperty in resultprops)
			{
				switch (productProperty.TypeCode)
				{
					case ProductPropertyType.DateField:
						Assert.AreEqual(productproptype[ProductPropertyType.DateField],
										_application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.CurrencyField:
						Assert.AreEqual(productproptype[ProductPropertyType.CurrencyField],
									  _application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.TextField:
						Assert.AreEqual(productproptype[ProductPropertyType.TextField],
									_application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
					case ProductPropertyType.MultipleChoiceField:
						Assert.AreEqual(productproptype[ProductPropertyType.MultipleChoiceField],
								   _application.CatalogServices.ProductPropertyValues.GetPropertyValue(resultprj.Bvin, productProperty));
						break;
				}
			}
			#endregion
		}

		/// <summary>
		/// Deletes the product.
		/// </summary>
		//[TestMethod]
		public void DeleteProduct()
		{
			//Arrange
			var prjname = _irepoproduct.GetDeleteProductSku();
			var prj = _application.CatalogServices.Products.FindBySku(prjname);

			//Act/Assert
			Assert.IsTrue(_application.DestroyProduct(prj.Bvin, _application.CurrentStore.Id));
		}


		/// <summary>
		/// Deletes all producty.
		/// </summary>
		//[TestMethod]
		public void DeleteAllProduct()
		{
			//Arrange
			var prjname = _irepoproduct.GetDeleteProductSku();
			var prj = _application.CatalogServices.Products.FindBySku(prjname);

			//Act
			_application.DestroyAllProductsForStore(_application.CurrentStore.Id);
			//TODO:Need to change SecureFind function null ref error

			var result = _application.CatalogServices.Categories.Find(prj.Bvin);

			//Assert
			Assert.AreEqual(null, result);
		}

		/// <summary>
		/// Gets the root product.
		/// </summary>
		/// <returns></returns>
		public Product GetRootProduct()
		{
			var prjsku = _irepoproduct.GetProductSku();
			var prj = _application.CatalogServices.Products.FindBySku(prjsku);
			if (prj == null) Assert.Fail();
			return prj;
		}
	}
}
