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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using System.Xml.XPath;
using DotNetNuke.Collections.Internal;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnLocalizationHelper : ILocalizationHelper
    {
        private readonly string _resourceFile;

        public DnnLocalizationHelper(string resourcefile)
        {
            _resourceFile = resourcefile;
        }

        public string GetString(string key)
        {
            return GetString(key, _resourceFile, null);
        }

        public string GetString(string key, string culture)
        {
            return GetString(key, _resourceFile, culture);
        }

        public string GetFormattedString(string key, params object[] args)
        {
            var value = GetString(key);
            if (string.IsNullOrEmpty(value))
                return value;
            return string.Format(value, args);
        }

        public string GetJsEncodedString(string key)
        {
            return GetJsEncodedString(key, new[] {"\'", "\""});
        }

        public string GetJsEncodedString(string key, string[] characters)
        {
            var value = GetString(key);
            foreach (var character in characters)
            {
                value = value.Replace(character, "\\" + character);
            }
            return value;
        }

        public Dictionary<string, string> GetResourceDictionary()
        {
            return GetResourceDictionary(_resourceFile);
        }

        #region Implementation

        private enum CustomizedLocale
        {
            None = 0,
            Portal = 1,
            Host = 2
        }

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof (DnnLocalizationHelper));

        private string GetString(string key, string resourceFileRoot, string language)
        {
            return GetString(key, resourceFileRoot, language, DnnGlobal.Instance.GetCurrentPortalSettings(), false);
        }

        private Dictionary<string, string> GetResourceDictionary(string resourceFileRoot)
        {
            var userLanguage = Null.NullString;
            var fallbackLanguage = Localization.SystemLocale;
            var defaultLanguage = Null.NullString;
            var portalId = Null.NullInteger;

            //Get the default language
            var portalSettings = DnnGlobal.Instance.GetCurrentPortalSettings();
            if (portalSettings != null)
            {
                defaultLanguage = portalSettings.DefaultLanguage;
                portalId = portalSettings.PortalId;
            }

            //Set the userLanguage if not passed in
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = Thread.CurrentThread.CurrentUICulture.ToString();
            }

            //Default the userLanguage to the defaultLanguage if not set
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = defaultLanguage;
            }
            Locale userLocale = null;
            try
            {
                if (Globals.Status != Globals.UpgradeStatus.Install)
                {
                    //Get Fallback language, but not when we are installing (because we may not have a database yet)
                    userLocale = LocaleController.Instance.GetLocale(userLanguage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
            {
                fallbackLanguage = userLocale.Fallback;
            }
            if (string.IsNullOrEmpty(resourceFileRoot))
            {
                resourceFileRoot = Localization.SharedResourceFile;
            }

            var resourceFile = GetResourceFileName(resourceFileRoot, userLanguage);
            var cacheKey = GetResourceFileCacheKey(resourceFile, portalId, CustomizedLocale.None);
            var dicResources = GetResourceFile(cacheKey);
            Dictionary<string, string> dicFallbackResources = null;
            Dictionary<string, string> dicDefaultResources = null;
            if (fallbackLanguage != userLanguage)
            {
                resourceFile = GetResourceFileName(resourceFileRoot, fallbackLanguage);
                cacheKey = GetResourceFileCacheKey(resourceFile, portalId, CustomizedLocale.None);
                dicFallbackResources = GetResourceFile(cacheKey);
            }
            if (defaultLanguage != userLanguage && defaultLanguage != fallbackLanguage)
            {
                resourceFile = GetResourceFileName(resourceFileRoot, defaultLanguage);
                cacheKey = GetResourceFileCacheKey(resourceFile, portalId, CustomizedLocale.None);
                dicDefaultResources = GetResourceFile(cacheKey);
            }

            return MergeDictionaries(dicResources, dicFallbackResources, dicDefaultResources);
        }

        private string GetString(string key, string resourceFileRoot, string language, PortalSettings portalSettings,
            bool disableShowMissingKeys)
        {
            //make the default translation property ".Text"
            if (key.IndexOf(".", StringComparison.Ordinal) < 1)
            {
                key += ".Text";
            }
            var resourceValue = Null.NullString;
            var keyFound = TryGetStringInternal(key, language, resourceFileRoot, portalSettings, ref resourceValue);

            //If the key can't be found then it doesn't exist in the Localization Resources
            if (Localization.ShowMissingKeys && !disableShowMissingKeys)
            {
                if (keyFound)
                {
                    resourceValue = "[L]" + resourceValue;
                }
                else
                {
                    resourceValue = "RESX:" + key;
                }
            }

            if (!keyFound)
            {
                Logger.WarnFormat("Missing localization key. key:{0} resFileRoot:{1} threadCulture:{2} userlan:{3}", key,
                    resourceFileRoot, Thread.CurrentThread.CurrentUICulture, language);
            }

            return resourceValue;
        }

        private static object GetResourceFileCallBack(CacheItemArgs cacheItemArgs)
        {
            var cacheKey = cacheItemArgs.CacheKey;
            Dictionary<string, string> resources = null;

            string filePath = null;
            try
            {
                //Get resource file lookup to determine if the resource file even exists
                var resourceFileExistsLookup = GetResourceFileLookupDictionary();

                if (ResourceFileMayExist(resourceFileExistsLookup, cacheKey))
                {
                    //check if an absolute reference for the resource file was used
                    if (cacheKey.Contains(":\\") && Path.IsPathRooted(cacheKey))
                    {
                        //if an absolute reference, check that the file exists
                        if (File.Exists(cacheKey))
                        {
                            filePath = cacheKey;
                        }
                    }

                    //no filepath found from an absolute reference, try and map the path to get the file path
                    if (filePath == null)
                    {
                        filePath = HostingEnvironment.MapPath(Globals.ApplicationPath + cacheKey);
                    }

                    //The file is not in the lookup, or we know it exists as we have found it before
                    if (File.Exists(filePath))
                    {
                        if (filePath != null)
                        {
                            var doc = new XPathDocument(filePath);
                            resources = new Dictionary<string, string>();
                            foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data"))
                            {
                                if (nav.NodeType != XPathNodeType.Comment)
                                {
                                    var selectSingleNode = nav.SelectSingleNode("value");
                                    if (selectSingleNode != null)
                                    {
                                        resources[nav.GetAttribute("name", string.Empty)] = selectSingleNode.Value;
                                    }
                                }
                            }
                        }
                        cacheItemArgs.CacheDependency = new DNNCacheDependency(filePath);

                        //File exists so add it to lookup with value true, so we are safe to try again
                        using (resourceFileExistsLookup.GetWriteLock())
                        {
                            resourceFileExistsLookup[cacheKey] = true;
                        }
                    }
                    else
                    {
                        //File does not exist so add it to lookup with value false, so we don't try again
                        using (resourceFileExistsLookup.GetWriteLock())
                        {
                            resourceFileExistsLookup[cacheKey] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("The following resource file caused an error while reading: {0}", filePath), ex);
            }
            return resources;
        }

        private static SharedDictionary<string, bool> GetResourceFileLookupDictionary()
        {
            return CBO.GetCachedObject<SharedDictionary<string, bool>>(
                new CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey,
                    DataCache.ResourceFileLookupDictionaryTimeOut, DataCache.ResourceFileLookupDictionaryCachePriority),
                c => new SharedDictionary<string, bool>(),
                true);
        }

        private static Dictionary<string, string> GetResourceFile(string resourceFile)
        {
            return CBO.GetCachedObject<Dictionary<string, string>>(
                new CacheItemArgs(resourceFile, DataCache.ResourceFilesCacheTimeOut,
                    DataCache.ResourceFilesCachePriority),
                GetResourceFileCallBack,
                true);
        }

        private static string GetResourceFileName(string resourceFileRoot, string language)
        {
            string resourceFile;
            language = language.ToLower();
            if (resourceFileRoot != null)
            {
                if (language == Localization.SystemLocale.ToLower() || string.IsNullOrEmpty(language))
                {
                    switch (resourceFileRoot.Substring(resourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            resourceFile = resourceFileRoot;
                            break;
                        case ".ascx":
                            resourceFile = resourceFileRoot + ".resx";
                            break;
                        case ".aspx":
                            resourceFile = resourceFileRoot + ".resx";
                            break;
                        default:
                            resourceFile = resourceFileRoot + ".ascx.resx"; //a portal module
                            break;
                    }
                }
                else
                {
                    switch (resourceFileRoot.Substring(resourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            resourceFile = resourceFileRoot.Replace(".resx", "." + language + ".resx");
                            break;
                        case ".ascx":
                            resourceFile = resourceFileRoot.Replace(".ascx", ".ascx." + language + ".resx");
                            break;
                        case ".aspx":
                            resourceFile = resourceFileRoot.Replace(".aspx", ".aspx." + language + ".resx");
                            break;
                        default:
                            resourceFile = resourceFileRoot + ".ascx." + language + ".resx";
                            break;
                    }
                }
            }
            else
            {
                if (language == Localization.SystemLocale.ToLower() || string.IsNullOrEmpty(language))
                {
                    resourceFile = Localization.SharedResourceFile;
                }
                else
                {
                    resourceFile = Localization.SharedResourceFile.Replace(".resx", "." + language + ".resx");
                }
            }
            return resourceFile;
        }

        private static bool ResourceFileMayExist(SharedDictionary<string, bool> resourceFileExistsLookup,
            string cacheKey)
        {
            bool mayExist;
            using (resourceFileExistsLookup.GetReadLock())
            {
                mayExist = !resourceFileExistsLookup.ContainsKey(cacheKey) || resourceFileExistsLookup[cacheKey];
            }
            return mayExist;
        }

        private static bool TryGetFromResourceFile(string key, string resourceFile, string userLanguage,
            string fallbackLanguage, string defaultLanguage, int portalID, ref string resourceValue)
        {
            //Try the user's language first
            var bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, userLanguage), portalID,
                ref resourceValue);

            if (!bFound && fallbackLanguage != userLanguage)
            {
                //Try fallback language next
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, fallbackLanguage), portalID,
                    ref resourceValue);
            }
            if (!bFound && !(defaultLanguage == userLanguage || defaultLanguage == fallbackLanguage))
            {
                //Try default Language last
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, defaultLanguage), portalID,
                    ref resourceValue);
            }
            return bFound;
        }

        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID,
            ref string resourceValue)
        {
            //Try Portal Resource File
            var bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Portal, ref resourceValue);
            if (!bFound)
            {
                //Try Host Resource File
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Host, ref resourceValue);
            }
            if (!bFound)
            {
                //Try Portal Resource File
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.None, ref resourceValue);
            }
            return bFound;
        }

        private static bool TryGetStringInternal(string key, string userLanguage, string resourceFile,
            PortalSettings portalSettings, ref string resourceValue)
        {
            var defaultLanguage = Null.NullString;
            var fallbackLanguage = Localization.SystemLocale;
            var portalId = Null.NullInteger;

            //Get the default language
            if (portalSettings != null)
            {
                defaultLanguage = portalSettings.DefaultLanguage;
                portalId = portalSettings.PortalId;
            }

            //Set the userLanguage if not passed in
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = Thread.CurrentThread.CurrentUICulture.ToString();
            }

            //Default the userLanguage to the defaultLanguage if not set
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = defaultLanguage;
            }
            Locale userLocale = null;
            try
            {
                if (Globals.Status != Globals.UpgradeStatus.Install)
                {
                    //Get Fallback language, but not when we are installing (because we may not have a database yet)
                    userLocale = LocaleController.Instance.GetLocale(userLanguage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
            {
                fallbackLanguage = userLocale.Fallback;
            }
            if (string.IsNullOrEmpty(resourceFile))
            {
                resourceFile = Localization.SharedResourceFile;
            }

            //Try the resource file for the key
            var bFound = TryGetFromResourceFile(key, resourceFile, userLanguage, fallbackLanguage, defaultLanguage,
                portalId, ref resourceValue);
            if (!bFound)
            {
                if (Localization.SharedResourceFile.ToLowerInvariant() != resourceFile.ToLowerInvariant())
                {
                    //try to use a module specific shared resource file
                    var localSharedFile =
                        resourceFile.Substring(0, resourceFile.LastIndexOf("/", StringComparison.Ordinal) + 1) +
                        Localization.LocalSharedResourceFile;

                    if (localSharedFile.ToLowerInvariant() != resourceFile.ToLowerInvariant())
                    {
                        bFound = TryGetFromResourceFile(key, localSharedFile, userLanguage, fallbackLanguage,
                            defaultLanguage, portalId, ref resourceValue);
                    }
                }
            }
            if (!bFound)
            {
                if (Localization.SharedResourceFile.ToLowerInvariant() != resourceFile.ToLowerInvariant())
                {
                    bFound = TryGetFromResourceFile(key, Localization.SharedResourceFile, userLanguage, fallbackLanguage,
                        defaultLanguage, portalId, ref resourceValue);
                }
            }
            return bFound;
        }

        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID,
            CustomizedLocale resourceType, ref string resourceValue)
        {
            var bFound = Null.NullBoolean;
            var cacheKey = GetResourceFileCacheKey(resourceFile, portalID, resourceType);

            //Get resource file lookup to determine if the resource file even exists
            var resourceFileExistsLookup = GetResourceFileLookupDictionary();

            if (ResourceFileMayExist(resourceFileExistsLookup, cacheKey))
            {
                //File is not in lookup or its value is true so we know it exists
                var dicResources = GetResourceFile(cacheKey);
                if (dicResources != null)
                {
                    bFound = dicResources.TryGetValue(key, out resourceValue);
                }
            }

            return bFound;
        }

        private static string GetResourceFileCacheKey(string resourceFile, int portalID, CustomizedLocale resourceType)
        {
            var resourceFileName = resourceFile;
            switch (resourceType)
            {
                case CustomizedLocale.Host:
                    resourceFileName = resourceFile.Replace(".resx", ".Host.resx");
                    break;
                case CustomizedLocale.Portal:
                    resourceFileName = resourceFile.Replace(".resx", ".Portal-" + portalID + ".resx");
                    break;
            }

            if (resourceFileName.StartsWith("desktopmodules", StringComparison.InvariantCultureIgnoreCase)
                || resourceFileName.StartsWith("admin", StringComparison.InvariantCultureIgnoreCase)
                || resourceFileName.StartsWith("controls", StringComparison.InvariantCultureIgnoreCase))
            {
                resourceFileName = "~/" + resourceFileName;
            }

            //Local resource files are either named ~/... or <ApplicationPath>/...
            //The following logic creates a cachekey of /....
            var cacheKey = resourceFileName.Replace("~/", "/").ToLowerInvariant();
            if (!string.IsNullOrEmpty(Globals.ApplicationPath))
            {
                if (Globals.ApplicationPath != "/portals")
                {
                    if (cacheKey.StartsWith(Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length);
                    }
                }
                else
                {
                    cacheKey = "~" + cacheKey;
                    if (cacheKey.StartsWith("~" + Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length + 1);
                    }
                }
            }
            return cacheKey;
        }

        private static Dictionary<string, string> MergeDictionaries(params Dictionary<string, string>[] dictionaries)
        {
            var resultDictionary = new Dictionary<string, string>();
            foreach (var dictionary in dictionaries)
            {
                if (dictionary == null)
                    continue;

                foreach (var item in dictionary)
                {
                    if (!resultDictionary.ContainsKey(item.Key))
                        resultDictionary.Add(item.Key, item.Value);
                }
            }
            return resultDictionary;
        }

        #endregion
    }
}