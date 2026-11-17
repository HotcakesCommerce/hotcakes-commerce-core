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
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Web.Hosting;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Storage;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce
{
    /// <summary>
    ///     Provides application-wide Hotcakes Commerce settings that apply to all stores in the system.
    /// </summary>
    /// <remarks>
    ///     This partial declaration loads settings from the mapped <c>hotcakes.config</c> file and exposes
    ///     values from the web application's <c>SiteSqlServer</c> connection string. The companion
    ///     <c>WebAppSettings.System.cs</c> partial declaration contains runtime configuration such as
    ///     cookie names and version information. Properties designer files are not part of this runtime
    ///     configuration surface and are intentionally excluded. Typed settings use their documented
    ///     defaults when the setting is absent or cannot be converted to the property's type. Connection
    ///     string accessors catch configuration lookup failures and return an empty string.
    /// </remarks>
    [Serializable]
    public partial class WebAppSettings
    {
        #region Fields

        private static readonly AppSettingsSection _appSettings;

        private const string HOTCAKESCONFIG = "hotcakes.config";

        private const string SETTING_MAXVARIANTS = "MaxVariants";
        private const int SETTING_MAXVARIANTS_DEFAULT = 150;
        private const string SETTING_TEXTEDITOR = "DefaultTextEditor";
        private const string SETTING_TEXTEDITOR_DEFAULT = "TinyMCE";
        private const string SETTING_AFFILIATEQS = "AffiliateQueryStringName";
        private const string SETTING_AFFILIATEQS_DEFAULT = "affid";
        private const string SETTING_INVENTORYLOWHRS = "InventoryLowHours";
        private const int SETTING_INVENTORYLOWHRS_DEFAULT = 24;
        private const string SETTING_INVENTORYLOWPREFIX = "InventoryLowReportLinePrefix";
        private const string SETTING_NEWPRODUCTBADGEDAYS = "NewProductBadgeDays";
        private const int SETTING_NEWPRODUCTBADGEDAYS_DEFAULT = 30;
        private const string SETTING_PRODUCTLONGDESCHGT = "ProductLongDescriptionEditorHeight";
        private const int SETTING_PRODUCTLONGDESCHGT_DEFAULT = 300;
        private const string SETTING_PRODUCTMAXPRICE = "ProductMaxPrice";
        private const decimal SETTING_PRODUCTMAXPRICE_DEFAULT = 99999999;

        private const string CONNECTIONSTRING = "SiteSqlServer";
        private const string DATAPROVIDER = "System.Data.SqlClient";
        private const string METADATA = @"res://*/Data.EF.HccDbContext.csdl|res://*/Data.EF.HccDbContext.ssdl|res://*/Data.EF.HccDbContext.msl";

        #endregion

        #region Constructor

        static WebAppSettings()
        {
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath(DiskStorage.GetHccVirtualPath(HOTCAKESCONFIG))
            };

            if (!string.IsNullOrEmpty(configFileMap.ExeConfigFilename))
            {
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                _appSettings = config.AppSettings;
            }
        }

        #endregion

        #region Properties / Hotcakes.Config

        /// <summary>
        ///     Gets the maximum number of variants allowed for a product.
        /// </summary>
        /// <value>The configured <c>MaxVariants</c> value, or <c>150</c> when it is unavailable.</value>
        public static int MaxVariants
        {
            get { return GetAppSetting(SETTING_MAXVARIANTS, SETTING_MAXVARIANTS_DEFAULT); }
        }

        /// <summary>
        ///     Gets the text editor identifier used by default by the application.
        /// </summary>
        /// <value>The configured <c>DefaultTextEditor</c> value, or <c>TinyMCE</c>.</value>
        public static string DefaultTextEditor
        {
            get { return GetAppSetting(SETTING_TEXTEDITOR, SETTING_TEXTEDITOR_DEFAULT); }
        }

        /// <summary>
        ///     Gets the query-string parameter name used to identify an affiliate.
        /// </summary>
        /// <value>The configured <c>AffiliateQueryStringName</c> value, or <c>affid</c>.</value>
        public static string AffiliateQueryStringName
        {
            get
            {
                return GetAppSetting(SETTING_AFFILIATEQS, SETTING_AFFILIATEQS_DEFAULT);
            }
        }

        /// <summary>
        ///     Gets the number of hours used when identifying low-inventory products.
        /// </summary>
        /// <value>The configured <c>InventoryLowHours</c> value, or <c>24</c>.</value>
        public static int InventoryLowHours
        {
            get
            {
                return GetAppSetting(SETTING_INVENTORYLOWHRS, SETTING_INVENTORYLOWHRS_DEFAULT);
            }
        }

        /// <summary>
        ///     Gets the prefix placed before each line in the low-inventory report.
        /// </summary>
        /// <value>The configured <c>InventoryLowReportLinePrefix</c> value, or an empty string.</value>
        public static string InventoryLowReportLinePrefix
        {
            get
            {
                return GetAppSetting(SETTING_INVENTORYLOWPREFIX, string.Empty);
            }
        }

        /// <summary>
        ///     Gets the number of days for which a product is marked as new.
        /// </summary>
        /// <value>The configured <c>NewProductBadgeDays</c> value, or <c>30</c>.</value>
        public static int NewProductBadgeDays
        {
            get
            {
                return GetAppSetting(SETTING_NEWPRODUCTBADGEDAYS, SETTING_NEWPRODUCTBADGEDAYS_DEFAULT);
            }
        }

        /// <summary>
        ///     Gets the height, in pixels, of the product long-description editor.
        /// </summary>
        /// <value>The configured <c>ProductLongDescriptionEditorHeight</c> value, or <c>300</c>.</value>
        public static int ProductLongDescriptionEditorHeight
        {
            get
            {
                return GetAppSetting(SETTING_PRODUCTLONGDESCHGT, SETTING_PRODUCTLONGDESCHGT_DEFAULT);
            }
        }

        /// <summary>
        ///     Gets the maximum product price accepted by the application.
        /// </summary>
        /// <value>The configured decimal <c>ProductMaxPrice</c> value, or <c>99999999</c>.</value>
        public static decimal ProductMaxPrice
        {
            get
            {
                return GetAppSetting<decimal>(SETTING_PRODUCTMAXPRICE, SETTING_PRODUCTMAXPRICE_DEFAULT);
            }
        }

        /// <summary>
        ///     Gets the bvin of the country used as the application country.
        /// </summary>
        /// <value>The United States country bvin from <see cref="Country.UnitedStatesCountryBvin"/>.</value>
        public static string ApplicationCountryBvin
        {
            get { return Country.UnitedStatesCountryBvin; }
        }

        /// <summary>
        ///     Gets the application's weight unit.
        /// </summary>
        /// <value><see cref="WeightType.Pounds"/>.</value>
        public static WeightType ApplicationWeightUnits
        {
            get { return WeightType.Pounds; }
        }

        /// <summary>
        ///     Gets the application's length unit.
        /// </summary>
        /// <value><see cref="LengthType.Inches"/>.</value>
        public static LengthType ApplicationLengthUnits
        {
            get { return LengthType.Inches; }
        }

        /// <summary>
        ///     Gets the minimum password length configured for ASP.NET membership.
        /// </summary>
        /// <value>The value of <see cref="System.Web.Security.Membership.MinRequiredPasswordLength"/>.</value>
        public static int PasswordMinimumLength
        {
            get { return System.Web.Security.Membership.MinRequiredPasswordLength; }
        }

        #endregion

        #region Properties / Web.Config

        /// <summary>
        ///     Gets the SQL Server connection string configured for read-only operations.
        /// </summary>
        /// <value>
        ///     The <c>SiteSqlServer</c> connection string prefixed with
        ///     <c>ApplicationIntent=ReadOnly;</c>, or an empty string when configuration cannot be read.
        /// </value>
        public static string HccReadOnlyConnectionString
        {
            get
            {
                var result = string.Empty;
                try
                {
                    result = "ApplicationIntent=ReadOnly;" + ConfigurationManager.ConnectionStrings[CONNECTIONSTRING].ConnectionString;
                }
                catch
                {
                }
                return result;
            }
        }

        /// <summary>
        ///     Gets the SQL Server connection string configured under <c>SiteSqlServer</c>.
        /// </summary>
        /// <value>The configured connection string, or an empty string when configuration cannot be read.</value>
        public static string HccConnectionString
        {
            get
            {
                var result = string.Empty;
                try
                {
                    result = ConfigurationManager.ConnectionStrings[CONNECTIONSTRING].ConnectionString;
                }
                catch
                {
                }
                return result;
            }
        }

        /// <summary>
        ///     Gets an Entity Framework connection string for the primary Hotcakes database.
        /// </summary>
        /// <value>
        ///     An entity connection using provider <c>System.Data.SqlClient</c>, the embedded
        ///     <c>HccDbContext</c> metadata, and <see cref="HccConnectionString"/> as its provider connection string.
        /// </value>
        public static string HccEFConnectionString
        {
            get
            {
                var connBuilder = new EntityConnectionStringBuilder
                {
                    Provider = DATAPROVIDER, ProviderConnectionString = HccConnectionString, Metadata = METADATA
                };
                return connBuilder.ConnectionString;
            }
        }

        /// <summary>
        ///     Gets an Entity Framework connection string for read-only Hotcakes database operations.
        /// </summary>
        /// <value>
        ///     An entity connection using provider <c>System.Data.SqlClient</c>, the embedded
        ///     <c>HccDbContext</c> metadata, and <see cref="HccReadOnlyConnectionString"/> as its provider connection string.
        /// </value>
        public static string HccEFReadOnlyConnectionString
        {
            get
            {
                var connBuilder = new EntityConnectionStringBuilder
                {
                    Provider = DATAPROVIDER,
                    ProviderConnectionString = HccReadOnlyConnectionString,
                    Metadata = METADATA
                };
                return connBuilder.ConnectionString;
            }
        }

        #endregion

        #region Implementation

        private static string GetAppSetting(string key, string defValue)
        {
            var element = _appSettings != null ? _appSettings.Settings[key] : null;
            return element != null ? element.Value : defValue;
        }

        private static T GetAppSetting<T>(string key, T defValue)
        {
            var element = _appSettings != null ? _appSettings.Settings[key] : null;

            if (element != null)
            {
                try
                {
                    return (T) Convert.ChangeType(element.Value, typeof (T));
                }
                catch
                {
                    return defValue;
                }
            }

            return defValue;
        }

        #endregion
    }
}
