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
using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Analytics.Config;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnAccountService : AccountService
    {
        #region Constructor

        public DnnAccountService(HccRequestContext context)
            : base(context)
        {
        }

        #endregion

        #region Properties

        public PortalSettings CurrentPortalSettings
        {
            get { return DnnGlobal.Instance.GetCurrentPortalSettings(); }
        }

        #endregion

        #region Public methods

        public override Store GetStoreByUrl(string url)
        {
            var aliasInfo = PortalAliasController.Instance.GetPortalAlias(url);
            if (aliasInfo != null)
            {
                var portalController = new PortalController();
                var portalInfo = portalController.GetPortal(aliasInfo.PortalID);
                if (portalInfo != null)
                {
                    return Stores.FindByStoreGuid(portalInfo.GUID);
                }
            }
            return null;
        }

        public override Store GetCurrentStore()
        {
            if (CurrentPortalSettings != null)
                return Stores.FindByStoreGuid(CurrentPortalSettings.GUID);
            return null;
        }

        public override GoogleAnalyticsSettings GetGoogleAnalyticsSettings()
        {
            var config = AnalyticsConfiguration.GetConfig("GoogleAnalytics");
            var result = new GoogleAnalyticsSettings();
            if (config != null)
            {
                bool? enable = null;
                foreach (AnalyticsSetting setting in config.Settings)
                {
                    switch (setting.SettingName.ToLower())
                    {
                        case "enable":
                            result.UseTracker = bool.Parse(setting.SettingValue);
                            break;
                        case "trackingid":
                            result.TrackerId = setting.SettingValue;
                            break;
                    }
                }
                if (!enable.HasValue)
                    enable = !string.IsNullOrEmpty(result.TrackerId);
                if (enable.HasValue)
                    result.UseTracker = enable.Value;
            }
            return result;
        }

        public override void SetGoogleAnalyticsSettings(GoogleAnalyticsSettings newSettings)
        {
            var config = AnalyticsConfiguration.GetConfig("GoogleAnalytics");
            if (config == null)
            {
                var isCommunityEdition = DotNetNukeContext.Current.Application.Name == "DNNCORP.CE";

                config = new AnalyticsConfiguration();
                if (!isCommunityEdition)
                    config.Rules = new AnalyticsRuleCollection();
                config.Settings = new AnalyticsSettingCollection();

                if (!isCommunityEdition)
                {
                    var enable = new AnalyticsSetting();
                    enable.SettingName = "Enable";
                    config.Settings.Add(enable);
                }

                var trackingId = new AnalyticsSetting();
                trackingId.SettingName = "TrackingId";
                config.Settings.Add(trackingId);
            }

            foreach (AnalyticsSetting setting in config.Settings)
            {
                switch (setting.SettingName.ToLower())
                {
                    case "enable":
                        setting.SettingValue = newSettings.UseTracker.ToString();
                        break;
                    case "trackingid":
                        setting.SettingValue = newSettings.TrackerId;
                        break;
                }
            }
            AnalyticsConfiguration.SaveConfig("GoogleAnalytics", config);
        }

        #endregion

        #region Implementation

        protected override Store CreateStore()
        {
            var portalSettings = DnnGlobal.Instance.GetCurrentPortalSettings();

            var s = new Store
            {
                StoreGuid = portalSettings.GUID,
                StoreName = portalSettings.PortalName,
                DateCreated = DateTime.UtcNow,
                CustomUrl = string.Empty
            };
            return s;
        }

        protected override void SetupDefaultStoreSettings(Store s)
        {
            base.SetupDefaultStoreSettings(s);
            var email = DnnGlobal.Instance.GetCurrentPortalSettings().Email;

            s.Settings.MailServer.FromEmail = email;
            s.Settings.MailServer.EmailForGeneral = email;
            s.Settings.MailServer.EmailForNewOrder = email;
        }

        #endregion
    }
}