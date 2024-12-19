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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Xml;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Framework.Providers;
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
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Commerce.Taxes.Providers.Avalara;
using Hotcakes.Modules.Core.Admin.Controls;
using Hotcakes.Shipping.FedEx;
using Util = DotNetNuke.Entities.Content.Common.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Hotcakes.Modules.Core
{
    [Serializable]
    public class HotcakesController : PortalModuleBase, IUpgradeable
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(HotcakesController));
        private IHostSettingsService _hostSettingsService;
        private HccRequestContext context = null;
        private AccountService accountServices = null;
        private List<Store> stores = null;
        private const string SCHEDULED_JOB_TYPE = "Hotcakes.Modules.Core.ClearUploadTempFiles, Hotcakes.Modules";
        private const string SCHEDULED_JOB_NAME = "Hotcakes Commerce: Clear Temporary Files";

        const string ScriptDelimiterRegex = "(?<=(?:[^\\w]+|^))GO(?=(?: |\\t)*?(?:\\r?\\n|$))";
        private static readonly Regex SqlObjRegex = new Regex(ScriptDelimiterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private const string TEMPLATE_MATCH = "</body>";
        private const string TEMPLATE_REPLACE =
            "<table class=\"hc-noprint\" style=\"width:100%;margin:1.5em auto;\"><tr><td style=\"width:100%;text-align:center;\">We built our store using the <em>FREE</em> and open-source <a href=\"https://hotcakes.org/?utm_source=hcc-install&amp;utm_medium=email-template&amp;utm_campaign={0}\" target=\"_blank\">Hotcakes Commerce e-commerce solution</a>.</td></tr></table></body>";

        private bool IsGenericCodeExecuted { get; set; }
        public HotcakesController()
        {
            _hostSettingsService = DependencyProvider.GetRequiredService<IHostSettingsService>();
        }

        public string UpgradeModule(string Version)
        {
            try
            {
                Logger.Info("Hotcakes - Upgrade Module...");
                context = new HccRequestContext();
                accountServices = Factory.CreateService<AccountService>(context);
                stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

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
                        UninstallControlPanel();
                        CreateTempFileScheduledJob();
                        UpdateEmailTemplateBranding();
                        break;

                    case "03.05.00":
                    case "03.06.00":
                        UpdateStoreSettings();
                        break; 
                    
                    case "03.09.00":
                        CreatePaymentFailureScheduler();
                        break;

                    default:
                        break;
                }

                // This code have to be executed only once and not depending on version
                if (!IsGenericCodeExecuted)
                {
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
                LogError(ex.Message, ex);
                Exceptions.LogException(ex);

                return "Failed";
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
            _hostSettingsService.Update(ClientResourceSettings.VersionKey,newVersion.ToString(CultureInfo.InvariantCulture), true);
        }

        private void EnsureDefaultZones()
        {
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
            foreach (var obsoletePackage in obsoletePackages)
            {
                UninstallPackage(obsoletePackage);
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

                var catImport = new CatalogImport(hccApp) { ImagesImportPath = intallFolderPath + "Images/" };
                catImport.ImportFromExcel(productsFile, (p, l) => { });

                Directory.Delete(HttpContext.Current.Server.MapPath(intallFolderPath), true);

                DnnGlobal.SetPortalSettings(null);
            }
        }

        private StoreConfig GetStoreConfig(string configPath)
        {
            using (var configFile = new FileStream(configPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(StoreConfig));
                return (StoreConfig)serializer.Deserialize(configFile);
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

        private void MigrateFedExRateSetting()
        {
            // 1.9.0: negotiated rates are no longer a global setting, but a "local" setting
            foreach (var store in stores)
            {
                context.CurrentStore = store;

                var fedExService =
                    AvailableServices.FindAll(store).FirstOrDefault(s => s.GetType() == typeof(FedExProvider));

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
            //Gets the UnInstall.SqlDataProvider file
            string script = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/DesktopModules/Hotcakes/Providers/DataProviders/SqlDataProvider/UnInstall.SqlDataProvider"));
            //Get the Connection String from the web.config file
            string connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            //Check if the script has the correct format
            var runAsQuery = SqlObjRegex.IsMatch(script);
            if (runAsQuery)
            {
                //Execute the SQL query using a DataProvider instance that owns DNN.
                //With this method we clean the module tables in the database and then we delete them. 
                DataProvider.Instance().ExecuteScript(connectionstring, script);
            }
            
            //Method to remove all module directories within the DesktopModules directory
            if (deleteModuleFiles && deleteStoreFiles)
            {
                var DesktopModulesDir = System.Web.HttpContext.Current.Server.MapPath("~/DesktopModules/Hotcakes");
                if (Directory.Exists(DesktopModulesDir))
                    Directory.Delete(DesktopModulesDir, true);
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
        
        public void CreatePaymentFailureScheduler()
        {
            var friendlyName = "Send Payment Failure Emails";
            var typeFullName = "Hotcakes.Modules.Core.PaymentFailureEmailJob, Hotcakes.Modules";

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
                    TimeLapseMeasurement = "d",
                    RetryTimeLapse = 30,
                    RetryTimeLapseMeasurement = "m",
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
                    Enabled = false,
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

        private void ExecuteXmlMerge(string path)
        {
            if (!File.Exists(path)) return;

            var doc = new XmlDocument();
            doc.Load(path);

            var app = DotNetNukeContext.Current.Application;
            var merge = new XmlMerge(doc, Globals.FormatVersion(app.Version), app.Description);

            merge.UpdateConfigs();
        }

        private void UninstallControlPanel()
        {
            try
            {
                UninstallPackage("Hotcakes.ControlPanel");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        private void CreateTempFileScheduledJob()
        {
            try
            {
                var scheduledJobs = SchedulingProvider.Instance().GetSchedule();  // retrieves all scheduled jobs 
                var jobList = ConvertArrayList(scheduledJobs);
                var job = jobList.FirstOrDefault(s => s.TypeFullName == SCHEDULED_JOB_TYPE);

                if (job == null || job.ScheduleID == Null.NullInteger)
                {
                    // the scheduled job doesn't exist yet
                    SchedulingProvider.Instance().AddSchedule(new ScheduleItem
                    {
                        TypeFullName = SCHEDULED_JOB_TYPE,
                        TimeLapseMeasurement = "w", // week
                        TimeLapse = 1,
                        RetryTimeLapseMeasurement = "m", // minutes
                        RetryTimeLapse = 30,
                        RetainHistoryNum = 10,
                        FriendlyName = SCHEDULED_JOB_NAME,
                        Enabled = true,
                        CatchUpEnabled = false
                    });

                    Logger.Debug("Created the scheduled job to clear temporary files created by Hotcakes Commerce.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        private void UpdateEmailTemplateBranding()
        {
            var installDate = DateTime.MinValue;
            var packageController = ServiceLocator<IPackageController, PackageController>.Instance;
            var package = packageController.GetExtensionPackage(Null.NullInteger, (PackageInfo p) => p.Name == "Hotcakes.Core");

            if (package != null)
            {
                installDate = package.CreatedOnDate;
            }

            if (installDate != DateTime.MinValue && installDate.Date == DateTime.Now.Date)
            {
                // this is a new install (not an upgrade)
                var context = new HccRequestContext();
                var templateRepo = Factory.CreateRepo<HtmlTemplateRepository>(context);
                var templates = templateRepo.FindAll();
                var hccVersion = package.Version;
                var branding = string.Format(TEMPLATE_REPLACE, hccVersion);

                foreach (var template in templates)
                {
                    if ((template.Subject == "Affiliate Approved Confirmation"
                        || template.Subject == "Affiliate Registration Confirmation"
                        || template.Subject == "Order [[Order.OrderNumber]] Update from [[Store.StoreName]]"
                        || template.Subject == "Receipt for Order [[Order.OrderNumber]] from [[Store.StoreName]]"
                        || template.Subject == "Shipping update for order [[Order.OrderNumber]] from [[Store.StoreName]]"
                        || template.Subject == "You haven't finished your purchase"
                        || template.Subject == "You received Gift Card")
                        && template.Body.StartsWith("<html>"))
                    {
                        template.Body.Replace(TEMPLATE_MATCH, branding);
                        templateRepo.Update(template);
                    }
                }
            }
        }

        private void UpdateStoreSettings()
        {
            try
            {
                // TODO: Implement the RefreshStoreSettings sproc 
                using (var db = Factory.CreateHccDbContext())
                {
                    Logger.Info("Trigger - Refresh Store Settings.");
                    db.hcc_RefreshStoreSettings();
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                throw;
            }
        }

        #region Private Helper Methods
        private List<ScheduleItem> ConvertArrayList(ArrayList jobs)
        {
            var newJobs = new List<ScheduleItem>();

            foreach (var item in jobs)
            {
                var newItem = (ScheduleItem)item;
                newJobs.Add(newItem);
            }

            return newJobs;
        }

        private void LogError(string message, Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(message, ex);
                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
            else
            {
                Logger.Error(message);
            }
        }
        #endregion

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
            "Hotcakes.MiniCartSkinObject",
            "Hotcakes.SkinSearch"
        };

        #endregion
    }
}