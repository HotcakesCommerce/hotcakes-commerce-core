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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.Web.Client;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Commerce.Taxes.Providers.Avalara;
using Hotcakes.Shipping.FedEx;
using Util = DotNetNuke.Entities.Content.Common.Util;

namespace Hotcakes.Modules.Core
{
    [Serializable]
    public class HotcakesController : IUpgradeable
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(HotcakesController));

        private bool IsGenericCodeExecuted { get; set; }
        
        public string UpgradeModule(string Version)
        {
            try
            {
                switch (Version)
                {
                    case "01.00.00":
                        DnnEventLog.InstallLogTypes();
                        break;

                    case "01.00.07":
                        AddAdminPageToAllPortals();
                        UninstallObsoletePackages();
                        break;

                    case "01.05.00":
                        EnsureDefaultZones();
                        break;

                    case "01.07.00":
                        MigrateOldPromotions();
                        break;

                    case "01.09.00":
                        MigrateFedExRateSetting();
                        break;
                    case "01.10.00":
                        MigrateAvalaraTaxProviderSetting();
                        break;

                    case "02.00.00":
                        CreateAbandonedCartScheduler();
                        UpdateRecurringOrdersScheduler();
                        UpdateConfigFile();
                        break;

                    case "03.00.01":
                        RevertHotcakesCloudConfig();
                        break;

                    case "03.03.00":
                        DeleteHostPage();
                        break;

                    default:
                        break;
                }

                // This code have to be executed only once and not depending on version
                if (!IsGenericCodeExecuted)
                {
                    // Copy System.Web.Mvc if it is not already present in bin folder
                    CopyMvcLibrary();

                    // Increment CRM version that is used to render resources
                    IncrementCrmVersion();

                    // Have to be run once to fix categorization for installs after 1.00.08
                    CategorizeModules();

                    InstallApplication();

                    IsGenericCodeExecuted = true;
                }

                return "Success";
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);

                return "Failed";
            }
        }

        //this is not used now. Kept for reference if in future needs to 
        // set config changes by string directly
        private void MergeFileUpdated()
        {
            var AssemblyBinding = @"<configuration>
              <nodes configfile=""Web.config"">
                <node path=""/configuration/runtime/ab:assemblyBinding"" 
                      action=""update"" 
                      collision=""save"" 
                      targetpath=""/configuration/runtime/ab:assemblyBinding/ab:dependentAssembly[ab:assemblyIdentity/@name='System.Web.Mvc'][ab:assemblyIdentity/@publicKeyToken='31bf3856ad364e35']"" 
                      nameSpace=""urn:schemas-microsoft-com:asm.v1"" 
                      nameSpacePrefix=""ab"">
                  <dependentAssembly xmlns=""urn:schemas-microsoft-com:asm.v1"">
                    <assemblyIdentity name=""System.Web.Mvc"" publicKeyToken=""31bf3856ad364e35"" />
                    <bindingRedirect oldVersion=""1.0.0.0-4.0.0.0"" newVersion=""4.0.0.1""/>
                  </dependentAssembly>
                </node>
              </nodes>
            </configuration>";

            using (TextReader sr = new StringReader(AssemblyBinding))
            {
                var app = DotNetNukeContext.Current.Application;
                var merge = new XmlMerge(sr, Globals.FormatVersion(app.Version), app.Description);
                merge.UpdateConfigs();
            }
        }

        /// <summary>
        ///     Update config file. This settings needs to be changed for Windows Azure Pack environment
        ///     where trial sites created. Trial sites not working without MVC dll's binding redirect
        ///     setting.
        /// </summary>
        private void UpdateConfigFile()
        {
            if (Environment.MachineName.ToUpper().Contains("CSITES-"))
            {
                var intallFolderPath = "~/DesktopModules/Hotcakes/Core/Install/";
                var configPath = HttpContext.Current.Server.MapPath(intallFolderPath + "02.00.00.config");
                ExecuteXmlMerge(configPath);
            }
        }

        private void RevertHotcakesCloudConfig()
        {
            if (Environment.MachineName.ToUpper().Contains("CSITES-"))
            {
                var intallFolderPath = "~/DesktopModules/Hotcakes/Core/Install/";
                var configPath = HttpContext.Current.Server.MapPath(intallFolderPath + "03.00.01.config");
                ExecuteXmlMerge(configPath);
            }
        }

        private void MigrateAvalaraTaxProviderSetting()
        {
            // 1.10.0: Avalara tax provider implementation has been changed for tax provider approach where end user can create their own tax provider any time.
            var context = new HccRequestContext();
            var accountServices = Factory.CreateService<AccountService>(context);
            var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

            foreach (var store in stores)
            {
                context.CurrentStore = store;

                var result = TaxProviders.AvailableProviders();
                var avalaraProvider = result.FirstOrDefault(p => p.ProviderName == "Avalara");

                var currentProvider = TaxProviders.CurrentTaxProvider(store);

                if (avalaraProvider != null)
                {
                    var storesettings = context.CurrentStore.Settings;

                    if (storesettings.Avalara.Enabled)
                    {
                        var settings = new AvalaraSettings();
                        settings.Merge(context.CurrentStore.Settings.TaxProviderSettingsGet(avalaraProvider.ProviderId));

                        if (string.IsNullOrEmpty(settings.LicenseKey))
                        {
                            settings.Account = storesettings.Avalara.Account;
                            settings.LicenseKey = storesettings.Avalara.LicenseKey;
                            settings.CompanyCode = storesettings.Avalara.CompanyCode;
                            settings.ServiceUrl = storesettings.Avalara.ServiceUrl;
                            settings.DebugMode = storesettings.Avalara.DebugMode;
                            settings.ShippingTaxCode = storesettings.Avalara.ShippingTaxCode;
                            settings.TaxExemptCode = storesettings.Avalara.TaxExemptCode;

                            // Save Settings 
                            context.CurrentStore.Settings.TaxProviderSettingsSet(avalaraProvider.ProviderId, settings);
                            accountServices.Stores.Update(context.CurrentStore);

                            if (currentProvider == null)
                            {
                                var taxProviderSetting =
                                    store.Settings.AllSettings.FirstOrDefault(
                                        s => s.SettingName == "TaxProvidersEnabled");

                                taxProviderSetting.SettingValue = avalaraProvider.ProviderId;

                                var settingsRepo = Factory.CreateRepo<StoreSettingsRepository>(context);
                                settingsRepo.Update(taxProviderSetting);
                            }
                        }
                    }

                    var avalraSettings =
                        store.Settings.AllSettings.FindAll(s => s.SettingName.ToLower().StartsWith("avalara"));

                    foreach (var item in avalraSettings)
                    {
                        var settingsRepo = Factory.CreateRepo<StoreSettingsRepository>(context);
                        settingsRepo.Delete(item.Id);
                    }
                }

                CacheManager.ClearForStore(context.CurrentStore.Id);
            }
        }

        private void IncrementCrmVersion()
        {
            var newVersion = Host.CrmVersion + 1;
            HostController.Instance.Update(ClientResourceSettings.VersionKey,
                newVersion.ToString(CultureInfo.InvariantCulture), true);
        }

        private void EnsureDefaultZones()
        {
            var context = new HccRequestContext();
            var accountServices = Factory.CreateService<AccountService>(context);
            var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

            foreach (var store in stores)
            {
                context.CurrentStore = store;

                var orderService = Factory.CreateService<OrderService>(context);
                orderService.EnsureDefaultZones(store.Id);
            }
        }

        private void UninstallObsoletePackages()
        {
            string[] obsoletePackages =
            {
                "Hotcakes.ProductRotator",
                "Hotcakes.CategoryRotator"
            };
            foreach (var obsoltePackage in obsoletePackages)
            {
                UninstallPackage(obsoltePackage);
            }
        }

        /// <summary>
        ///     Creates and configures hotcakes store from config file.
        /// </summary>
        /// <exception cref="CreateStoreException">Could not create store</exception>
        private void InstallApplication()
        {
            var intallFolderPath = "~/DesktopModules/Hotcakes/Core/Install/";
            var configPath = HttpContext.Current.Server.MapPath(intallFolderPath + "Hotcakes.install.config");
            if (File.Exists(configPath))
            {
                // hardcoded for now since we create only one store now
                var portalId = 0;
                var portalSettings = new PortalSettings(portalId);
                DnnGlobal.SetPortalSettings(portalSettings);

                var productsFile = HttpContext.Current.Server.MapPath(intallFolderPath + "Products.xlsx");

                var hccApp = HotcakesApplication.Current;

                var store = hccApp.AccountServices.CreateAndSetupStore();
                if (store == null)
                    throw new CreateStoreException("Could not create store");

                var config = GetStoreConfig(configPath);

                store.StoreName = config.StoreName;
                foreach (var settingConfig in config.Settings)
                {
                    store.Settings.AddOrUpdateLocalSetting(new StoreSetting
                    {
                        SettingName = settingConfig.Name,
                        SettingValue = settingConfig.Value
                    });
                }

                hccApp.CurrentStore = store;
                hccApp.UpdateCurrentStore();

                hccApp.AccountServices.Stores.SetLastOrderNumber(store.Id, config.LastOrderNumber);

                var systemColumnsFilePath =
                    "~/DesktopModules/Hotcakes/Core/Admin/Parts/ContentBlocks/SystemColumnsData.xml";
                hccApp.ContentServices.Columns.CreateFromTemplateFile(systemColumnsFilePath);

                hccApp.OrderServices.EnsureDefaultZones(store.Id);

                var urlSettings = hccApp.CurrentStore.Settings.Urls;
                PageUtils.EnsureTabsExist(urlSettings);
                hccApp.UpdateCurrentStore();

                var catImport = new CatalogImport(hccApp) {ImagesImportPath = intallFolderPath + "Images/"};
                catImport.ImportFromExcel(productsFile, (p, l) => { });

                Directory.Delete(HttpContext.Current.Server.MapPath(intallFolderPath), true);

                DnnGlobal.SetPortalSettings(null);
            }
        }

        private StoreConfig GetStoreConfig(string configPath)
        {
            using (var configFile = new FileStream(configPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof (StoreConfig));
                return (StoreConfig) serializer.Deserialize(configFile);
            }
        }

        private void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (var file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }

        private void AddAdminPageToAllPortals()
        {
            var tabCtl = new TabController();
            var portals = new PortalController().GetPortals().OfType<PortalInfo>();

            foreach (var portal in portals)
            {
                AddAdminPage(tabCtl, portal);
            }
        }

        internal static void AddAdminPage(int portalId)
        {
            var portalInfo = new PortalController().GetPortal(portalId);
            AddAdminPage(new TabController(), portalInfo);
        }

        private static void AddAdminPage(TabController tabCtl, PortalInfo portal)
        {
            var tab = Upgrade.AddAdminPage(
                portal,
                "Hotcakes Administration",
                "Hotcakes Administration",
                "~/Icons/Sigma/Configuration_16X16_Standard.png",
                "~/Icons/Sigma/Configuration_32X32_Standard.png",
                true);
            tab.Url = VirtualPathUtility.ToAbsolute("~/DesktopModules/Hotcakes/Core/Admin/Default.aspx");
            tabCtl.UpdateTab(tab);
        }

        private void DeleteHostPage()
        {
            try
            {
                // this will only be the case if this is an upgrade 
                Upgrade.RemoveHostPage("Hotcakes Administration");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private void CategorizeModules()
        {
            foreach (var moduleName in HotcakesModules)
            {
                CategorizeHccModule(moduleName);
            }
        }

        public void CategorizeHccModule(string moduleName)
        {
            var objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(moduleName, Null.NullInteger);
            var vocabularyId = -1;
            var termController = Util.GetTermController();
            Term moduleCategoryTerm = null;
            foreach (var term in termController.GetTermsByVocabulary("Module_Categories"))
            {
                vocabularyId = term.VocabularyId;
                if (term.Name == MODULE_CATEGORY)
                {
                    moduleCategoryTerm = term;
                    break;
                }
            }
            if (moduleCategoryTerm == null)
            {
                moduleCategoryTerm = new Term(vocabularyId);
                moduleCategoryTerm.Name = MODULE_CATEGORY;
                termController.AddTerm(moduleCategoryTerm);
            }
            var contentItem = Util.GetContentController().GetContentItem(objDesktopModule.ContentItemId);
            var contentItemTermIds = contentItem.Terms.Select(t => t.TermId).ToList();
            if (!contentItemTermIds.Contains(moduleCategoryTerm.TermId))
                termController.AddTermToContent(moduleCategoryTerm, contentItem);
        }

        private void MigrateOldPromotions()
        {
            var context = new HccRequestContext();
            var accountServices = Factory.CreateService<AccountService>(context);
            var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

            foreach (var store in stores)
            {
                context.CurrentStore = store;

                var marketingServices = Factory.CreateService<MarketingService>(context);
                marketingServices.MigrateOldPromotions();
            }
        }

        private void MigrateFedExRateSetting()
        {
            // 1.9.0: negotiated rates are no longer a global setting, but a "local" setting
            var context = new HccRequestContext();
            var accountServices = Factory.CreateService<AccountService>(context);
            var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

            foreach (var store in stores)
            {
                context.CurrentStore = store;

                var fedExService =
                    AvailableServices.FindAll(store).FirstOrDefault(s => s.GetType() == typeof (FedExProvider));

                if (fedExService != null)
                {
                    var oldNegotiationRateValue = true;
                    var oldNegotiationRateSetting =
                        store.Settings.AllSettings.FirstOrDefault(s => s.SettingName == "ShippingFedExUseListRates");

                    if (oldNegotiationRateSetting != null)
                    {
                        // reversing the logic since the previous setting was citing LIST rates and not negotiated rates
                        oldNegotiationRateValue = !oldNegotiationRateSetting.SettingValueAsBool;
                    }

                    // get all fedex methods
                    var orderService = Factory.CreateService<OrderService>(context);
                    var methods =
                        orderService.ShippingMethods.FindAll(store.Id)
                            .Where(m => m.ShippingProviderId == fedExService.Id);

                    foreach (var method in methods)
                    {
                        // update each FedEx shipping method instance with a local setting, using the original global value as a default
                        var settings = new FedExServiceSettings();
                        settings.Merge(method.Settings);

                        settings["UseNegotiatedRates"] = oldNegotiationRateValue.ToString();

                        method.Settings.Merge(settings);

                        orderService.ShippingMethods.Update(method);
                    }

                    // get rid of the old global setting
                    if (oldNegotiationRateSetting != null)
                    {
                        var settingsRepo = Factory.CreateRepo<StoreSettingsRepository>(context);
                        settingsRepo.Delete(oldNegotiationRateSetting.Id);
                    }
                }

                CacheManager.ClearForStore(store.Id);
            }
        }

        public static void Uninstall(bool deleteModuleFiles, bool deleteStoreFiles)
        {
            foreach (var packageName in HotcakesPackages)
            {
                UninstallPackage(packageName);
            }

            Upgrade.RemoveHostPage("Hotcakes Administration");
            Upgrade.RemoveAdminPages("//Admin//HotcakesAdministration");

            var portals = new PortalController().GetPortals().OfType<PortalInfo>();
            if (deleteModuleFiles)
            {
                var hostViewsetsDir = Path.Combine(Globals.HostMapPath, "HotcakesViews");
                if (Directory.Exists(hostViewsetsDir))
                    Directory.Delete(hostViewsetsDir, true);

                foreach (var portal in portals)
                {
                    var portalViewsetsDir = Path.Combine(portal.HomeDirectoryMapPath, "HotcakesViews");
                    if (Directory.Exists(portalViewsetsDir))
                        Directory.Delete(portalViewsetsDir, true);
                }
            }

            if (deleteStoreFiles)
            {
                foreach (var portal in portals)
                {
                    var portalViewsetsDir = Path.Combine(portal.HomeDirectoryMapPath, "Hotcakes");
                    if (Directory.Exists(portalViewsetsDir))
                        Directory.Delete(portalViewsetsDir, true);
                }
            }
        }

        private static void UninstallPackage(string packageName)
        {
            var packageController = ServiceLocator<IPackageController, PackageController>.Instance;
            var package = packageController.GetExtensionPackage(Null.NullInteger,
                (PackageInfo p) => p.Name == packageName);
            if (package != null)
            {
                var installer = new Installer(package, HttpContext.Current.Request.MapPath("~/"));
                installer.UnInstall(true);
            }
        }

        public void CreateAbandonedCartScheduler()
        {
            var friendlyName = "Send Abandoned Cart Emails";
            var typeFullName = "Hotcakes.Modules.Core.AbandonedCartEmailJob, Hotcakes.Modules";

            var schedules = SchedulingProvider.Instance().GetSchedule().OfType<ScheduleItem>();
            if (!schedules.Any(s => s.FriendlyName == friendlyName))
            {
                var oItem = new ScheduleItem
                {
                    FriendlyName = friendlyName,
                    TypeFullName = typeFullName,
                    Enabled = true,
                    CatchUpEnabled = false,
                    RetainHistoryNum = 0,
                    TimeLapse = 1,
                    TimeLapseMeasurement = "h",
                    RetryTimeLapse = 1,
                    RetryTimeLapseMeasurement = "h",
                    ScheduleSource = ScheduleSource.NOT_SET
                };
                SchedulingProvider.Instance().AddSchedule(oItem);
            }
        }

        public void UpdateRecurringOrdersScheduler()
        {
            var friendlyName = "Update Recurring Orders";
            var typeFullName = "Hotcakes.Modules.Core.UpdateRecurringOrdersJob, Hotcakes.Modules";

            var schedules = SchedulingProvider.Instance().GetSchedule().OfType<ScheduleItem>();
            if (!schedules.Any(s => s.FriendlyName == friendlyName))
            {
                var oItem = new ScheduleItem
                {
                    FriendlyName = friendlyName,
                    TypeFullName = typeFullName,
                    Enabled = true,
                    CatchUpEnabled = false,
                    RetainHistoryNum = 0,
                    TimeLapse = 8,
                    TimeLapseMeasurement = "h",
                    RetryTimeLapse = 8,
                    RetryTimeLapseMeasurement = "h",
                    ScheduleSource = ScheduleSource.NOT_SET
                };
                SchedulingProvider.Instance().AddSchedule(oItem);
            }
        }

        private void CopyMvcLibrary()
        {
            var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version;

            if (versionofDNN < new Version("8.0"))
            {
                var sourceFile = HostingEnvironment.MapPath("~/DesktopModules/Hotcakes/System.Web.Mvc.dll");
                var destFile = HostingEnvironment.MapPath("~/bin/System.Web.Mvc.dll");
                if (!File.Exists(destFile))
                    File.Copy(sourceFile, destFile);
            }
        }

        private void ExecuteXmlMerge(string path)
        {
            if (!File.Exists(path)) return;

            var doc = new XmlDocument();
            doc.Load(path);
            
            var app = DotNetNukeContext.Current.Application;
            var merge = new XmlMerge(doc, Globals.FormatVersion(app.Version), app.Description);

            merge.UpdateConfigs();
        }

        #region Constants

        private const string MODULE_CATEGORY = "Hotcakes Commerce";

        private static readonly string[] HotcakesModules =
        {
            "Hotcakes.AddressBook",
            "Hotcakes.Cart",
            "Hotcakes.CategoryMenu",
            "Hotcakes.CategoryViewer",
            "Hotcakes.Checkout",
            "Hotcakes.FeaturedProducts",
            "Hotcakes.LastProductsViewed",
            "Hotcakes.MiniCart",
            "Hotcakes.SearchInput",
            "Hotcakes.OrderHistory",
            "Hotcakes.ProductGrid",
            "Hotcakes.ProductViewer",
            "Hotcakes.ProductReviews",
            "Hotcakes.ContentBlocks",
            "Hotcakes.Search",
            "Hotcakes.TopWeeklySellers",
            "Hotcakes.Top10Products",
            "Hotcakes.WishList",
            "Hotcakes.AffiliateRegistration",
            "Hotcakes.AffiliateDashboard"
        };

        private static readonly string[] HotcakesPackages =
        {
            //Libraries
            "Hotcakes.Core",
            "Hotcakes.ExtensionUrlProvider",
            //Modules
            "Hotcakes.AddressBook",
            "Hotcakes.Cart",
            "Hotcakes.CategoryMenu",
            "Hotcakes.CategoryViewer",
            "Hotcakes.Checkout",
            "Hotcakes.FeaturedProducts",
            "Hotcakes.LastProductsViewed",
            "Hotcakes.MiniCart",
            "Hotcakes.SearchInput",
            "Hotcakes.OrderHistory",
            "Hotcakes.ProductGrid",
            "Hotcakes.ProductViewer",
            "Hotcakes.ProductReviews",
            "Hotcakes.ContentBlocks",
            "Hotcakes.Search",
            "Hotcakes.TopWeeklySellers",
            "Hotcakes.Top10Products",
            "Hotcakes.WishList",
            "Hotcakes.AffiliateRegistration",
            "Hotcakes.AffiliateDashboard",
            //SkinObjects
            "Hotcakes.SkinAffiliate",
            "Hotcakes.SkinSearch",
            //Providers
            "Hotcakes.ControlPanel"
        };

        #endregion
    }
}