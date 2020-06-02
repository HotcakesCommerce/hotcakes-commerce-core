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

        #endregion

        #region Constructor

        static WebAppSettings()
        {
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath(DiskStorage.GetHccVirtualPath("hotcakes.config"))
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
            get { return GetAppSetting("MaxVariants", 150); }
        }

        public static string DefaultTextEditor
        {
            get { return GetAppSetting("DefaultTextEditor", "RadEditor"); }
        }

        public static string AffiliateQueryStringName
        {
            get
            {
                return GetAppSetting("AffiliateQueryStringName", "affid");
            }
        }

        public static int InventoryLowHours
        {
            get
            {
                return GetAppSetting("InventoryLowHours", 24);
            }
        }

        public static string InventoryLowReportLinePrefix
        {
            get
            {
                return GetAppSetting("InventoryLowReportLinePrefix", string.Empty);
            }
        }

        public static int NewProductBadgeDays
        {
            get
            {
                return GetAppSetting("NewProductBadgeDays", 30);
            }
        }

        public static int ProductLongDescriptionEditorHeight
        {
            get
            {
                return GetAppSetting("ProductLongDescriptionEditorHeight", 300);
            }
        }

        public static decimal ProductMaxPrice
        {
            get
            {
                return GetAppSetting<decimal>("ProductMaxPrice", 99999999);
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

        public static string HccConnectionString
        {
            get
            {
                var result = string.Empty;
                try
                {
                    result = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
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
                var connBuilder = new EntityConnectionStringBuilder();
                connBuilder.Provider = "System.Data.SqlClient";
                connBuilder.ProviderConnectionString = HccConnectionString;
                connBuilder.Metadata =
                    @"res://*/Data.EF.HccDbContext.csdl|res://*/Data.EF.HccDbContext.ssdl|res://*/Data.EF.HccDbContext.msl";
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