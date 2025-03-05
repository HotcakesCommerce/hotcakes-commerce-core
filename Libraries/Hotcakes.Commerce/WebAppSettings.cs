#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    ///     These are application wide settings and apply to all stores in the system
    /// </summary>
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

        public static int MaxVariants
        {
            get { return GetAppSetting(SETTING_MAXVARIANTS, SETTING_MAXVARIANTS_DEFAULT); }
        }

        public static string DefaultTextEditor
        {
            get { return GetAppSetting(SETTING_TEXTEDITOR, SETTING_TEXTEDITOR_DEFAULT); }
        }

        public static string AffiliateQueryStringName
        {
            get
            {
                return GetAppSetting(SETTING_AFFILIATEQS, SETTING_AFFILIATEQS_DEFAULT);
            }
        }

        public static int InventoryLowHours
        {
            get
            {
                return GetAppSetting(SETTING_INVENTORYLOWHRS, SETTING_INVENTORYLOWHRS_DEFAULT);
            }
        }

        public static string InventoryLowReportLinePrefix
        {
            get
            {
                return GetAppSetting(SETTING_INVENTORYLOWPREFIX, string.Empty);
            }
        }

        public static int NewProductBadgeDays
        {
            get
            {
                return GetAppSetting(SETTING_NEWPRODUCTBADGEDAYS, SETTING_NEWPRODUCTBADGEDAYS_DEFAULT);
            }
        }

        public static int ProductLongDescriptionEditorHeight
        {
            get
            {
                return GetAppSetting(SETTING_PRODUCTLONGDESCHGT, SETTING_PRODUCTLONGDESCHGT_DEFAULT);
            }
        }

        public static decimal ProductMaxPrice
        {
            get
            {
                return GetAppSetting<decimal>(SETTING_PRODUCTMAXPRICE, SETTING_PRODUCTMAXPRICE_DEFAULT);
            }
        }

        public static string ApplicationCountryBvin
        {
            get { return Country.UnitedStatesCountryBvin; }
        }

        public static WeightType ApplicationWeightUnits
        {
            get { return WeightType.Pounds; }
        }

        public static LengthType ApplicationLengthUnits
        {
            get { return LengthType.Inches; }
        }

        public static int PasswordMinimumLength
        {
            get { return System.Web.Security.Membership.MinRequiredPasswordLength; }
        }

        #endregion

        #region Properties / Web.Config

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