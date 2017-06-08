#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Hotcakes.Commerce.Dnn.Resources;

namespace Hotcakes.Commerce.Dnn.Mvc
{
    [Serializable]
    public class HccRazorViewEngine : RazorViewEngine
    {
        // format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:{rootFolder}:"
        private const string CacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";
        private const string CacheKeyPrefixMaster = "Master";
        private const string CacheKeyPrefixPartial = "Partial";
        private const string CacheKeyPrefixView = "View";
        private static readonly string[] _emptyLocations = new string[0];
        internal Func<string, string> GetExtensionThunk;

        public HccRazorViewEngine()
        {
            GetExtensionThunk = VirtualPathUtility.GetExtension;

            AreaViewLocationFormats = new[]
            {
                "{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "{3}/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            AreaMasterLocationFormats = new[]
            {
                "{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "{3}/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            AreaPartialViewLocationFormats = new[]
            {
                "{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "{3}/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[]
            {
                "{2}/Views/{1}/{0}.cshtml",
                "{2}/Views/{1}/{0}.vbhtml",
                "{2}/Views/Shared/{0}.cshtml",
                "{2}/Views/Shared/{0}.vbhtml"
            };

            MasterLocationFormats = new[]
            {
                "{2}/Views/{1}/{0}.cshtml",
                "{2}/Views/{1}/{0}.vbhtml",
                "{2}/Views/Shared/{0}.cshtml",
                "{2}/Views/Shared/{0}.vbhtml"
            };

            PartialViewLocationFormats = new[]
            {
                "{2}/Views/{1}/{0}.cshtml",
                "{2}/Views/{1}/{0}.vbhtml",
                "{2}/Views/Shared/{0}.cshtml",
                "{2}/Views/Shared/{0}.vbhtml"
            };
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(controllerContext, virtualPath);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
            bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException(HotcakesResources.Common_NullOrEmpty, "partialViewName");
            }

            string[] searched;
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var partialPath = GetPath(controllerContext, PartialViewLocationFormats, AreaPartialViewLocationFormats,
                "PartialViewLocationFormats", partialViewName, controllerName, CacheKeyPrefixPartial, useCache,
                out searched);

            if (string.IsNullOrEmpty(partialPath))
            {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName,
            string masterName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(HotcakesResources.Common_NullOrEmpty, "viewName");
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var viewPath = GetPath(controllerContext, ViewLocationFormats, AreaViewLocationFormats,
                "ViewLocationFormats", viewName, controllerName, CacheKeyPrefixView, useCache, out viewLocationsSearched);
            var masterPath = GetPath(controllerContext, MasterLocationFormats, AreaMasterLocationFormats,
                "MasterLocationFormats", masterName, controllerName, CacheKeyPrefixMaster, useCache,
                out masterLocationsSearched);

            if (string.IsNullOrEmpty(viewPath) ||
                (string.IsNullOrEmpty(masterPath) && !string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            }

            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations,
            string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache,
            out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;

            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            var areaName = AreaHelpers.GetAreaName(controllerContext.RouteData);
            var usingAreas = !string.IsNullOrEmpty(areaName);
            var viewLocations = GetViewLocations(locations, usingAreas ? areaLocations : null);

            if (viewLocations.Count == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    HotcakesResources.Common_PropertyCannotBeNullOrEmpty, locationsPropertyName));
            }

            var nameRepresentsPath = IsSpecificPath(name);
            var rootFolder = HotcakesApplication.Current.ViewsVirtualPath;
            var cacheKey = CreateCacheKey(cacheKeyPrefix, name, nameRepresentsPath ? string.Empty : controllerName,
                areaName, rootFolder);

            if (useCache)
            {
                // Only look at cached display modes that can handle the context.
                var possibleDisplayModes =
                    DisplayModeProvider.GetAvailableDisplayModesForContext(controllerContext.HttpContext,
                        controllerContext.DisplayMode);
                foreach (var displayMode in possibleDisplayModes)
                {
                    var cachedLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext,
                        AppendDisplayModeToCacheKey(cacheKey, displayMode.DisplayModeId));

                    if (cachedLocation != null)
                    {
                        if (controllerContext.DisplayMode == null)
                        {
                            controllerContext.DisplayMode = displayMode;
                        }

                        return cachedLocation;
                    }
                }

                // GetPath is called again without using the cache.
                return null;
            }
            return nameRepresentsPath
                ? GetPathFromSpecificName(controllerContext, name, rootFolder, cacheKey, ref searchedLocations)
                : GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, rootFolder,
                    cacheKey, ref searchedLocations);
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats,
            string[] areaViewLocationFormats)
        {
            var list = new List<ViewLocation>();
            if (areaViewLocationFormats != null)
            {
                foreach (var str in areaViewLocationFormats)
                {
                    list.Add(new AreaAwareViewLocation(str));
                }
            }
            if (viewLocationFormats != null)
            {
                foreach (var str2 in viewLocationFormats)
                {
                    list.Add(new ViewLocation(str2));
                }
            }
            return list;
        }

        private static bool IsSpecificPath(string name)
        {
            var c = name[0];
            return c == '~' || c == '/';
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName,
            string rootFolder)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, GetType().AssemblyQualifiedName, prefix,
                name, controllerName, areaName, rootFolder);
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations,
            string name, string controllerName, string areaName, string rootFolder, string cacheKey,
            ref string[] searchedLocations)
        {
            var result = string.Empty;
            searchedLocations = new string[locations.Count];

            for (var i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                var virtualPath = location.Format(name, controllerName, areaName, rootFolder);
                var virtualPathDisplayInfo = DisplayModeProvider.GetDisplayInfoForVirtualPath(virtualPath,
                    controllerContext.HttpContext, path => FileExists(controllerContext, path),
                    controllerContext.DisplayMode);

                if (virtualPathDisplayInfo != null)
                {
                    var resolvedVirtualPath = virtualPathDisplayInfo.FilePath;

                    searchedLocations = _emptyLocations;
                    result = resolvedVirtualPath;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext,
                        AppendDisplayModeToCacheKey(cacheKey, virtualPathDisplayInfo.DisplayMode.DisplayModeId), result);

                    if (controllerContext.DisplayMode == null)
                    {
                        controllerContext.DisplayMode = virtualPathDisplayInfo.DisplayMode;
                    }

                    // Populate the cache with the existing paths returned by all display modes.
                    // Since we currently don't keep track of cache misses, if we cache view.aspx on a request from a standard browser
                    // we don't want a cache hit for view.aspx from a mobile browser so we populate the cache with view.Mobile.aspx.
                    IEnumerable<IDisplayMode> allDisplayModes = DisplayModeProvider.Modes;
                    foreach (var displayMode in allDisplayModes)
                    {
                        if (displayMode.DisplayModeId != virtualPathDisplayInfo.DisplayMode.DisplayModeId)
                        {
                            var displayInfoToCache = displayMode.GetDisplayInfo(controllerContext.HttpContext,
                                virtualPath, path => FileExists(controllerContext, path));

                            if (displayInfoToCache != null && displayInfoToCache.FilePath != null)
                            {
                                ViewLocationCache.InsertViewLocation(controllerContext.HttpContext,
                                    AppendDisplayModeToCacheKey(cacheKey, displayInfoToCache.DisplayMode.DisplayModeId),
                                    displayInfoToCache.FilePath);
                            }
                        }
                    }
                    break;
                }

                searchedLocations[i] = virtualPath;
            }

            return result;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string rootFolder,
            string cacheKey, ref string[] searchedLocations)
        {
            var correctedName = name;
            if (correctedName.StartsWith("~"))
                correctedName = correctedName.TrimStart('~');
            if (!correctedName.StartsWith("/"))
                correctedName = "/" + correctedName;

            var virtualPath = string.Concat(rootFolder, correctedName);

            var result = string.Empty;
            searchedLocations = new string[1];

            if (!(FilePathIsSupported(virtualPath) && FileExists(controllerContext, virtualPath)))
            {
                result = string.Empty;
                searchedLocations[0] = virtualPath;
            }
            else
            {
                result = virtualPath;
                searchedLocations = _emptyLocations;
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private bool FilePathIsSupported(string virtualPath)
        {
            if (FileExtensions == null)
            {
                return true;
            }
            var str = GetExtensionThunk(virtualPath).TrimStart('.');
            return FileExtensions.Contains(str, StringComparer.OrdinalIgnoreCase);
        }

        internal static string AppendDisplayModeToCacheKey(string cacheKey, string displayMode)
        {
            // key format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:"
            // so append "{displayMode}:" to the key
            return cacheKey + displayMode + ":";
        }

        internal static class AreaHelpers
        {
            // Methods
            public static string GetAreaName(RouteBase route)
            {
                var area = route as IRouteWithArea;
                if (area != null)
                {
                    return area.Area;
                }
                var route2 = route as Route;
                if ((route2 != null) && (route2.DataTokens != null))
                {
                    return route2.DataTokens["area"] as string;
                }
                return null;
            }

            public static string GetAreaName(RouteData routeData)
            {
                object obj2;
                if (routeData.DataTokens.TryGetValue("area", out obj2))
                {
                    return obj2 as string;
                }
                return GetAreaName(routeData.Route);
            }
        }

        private class ViewLocation
        {
            protected readonly string _virtualPathFormatString;

            public ViewLocation(string virtualPathFormatString)
            {
                _virtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName, string rootFolder)
            {
                return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName,
                    rootFolder);
            }
        }

        private class AreaAwareViewLocation : ViewLocation
        {
            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string rootFolder)
            {
                return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName,
                    areaName, rootFolder);
            }
        }
    }
}