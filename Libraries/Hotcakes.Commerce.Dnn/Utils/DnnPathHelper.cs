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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Dnn.Utils
{
    /// <summary>
    /// Resolves Hotcakes view paths and enumerates available Razor views in the DNN installation.
    /// </summary>
    [Serializable]
    public static class DnnPathHelper
    {
        /// <summary>
        /// Gets the view names available for a controller in the shared Hotcakes views directory.
        /// </summary>
        /// <param name="controller">The controller directory whose views should be enumerated.</param>
        /// <returns>A sorted list of view names without the <c>.cshtml</c> extension.</returns>
        public static List<string> GetViewNames(string controller)
        {
            var path =
                HostingEnvironment.MapPath(string.Concat(HotcakesApplication.Current.ViewsVirtualPath, "/Views/",
                    controller));

            var fileNames = PathHelper.ListFiles(path, new[] {"*.cshtml"}, new[] {"_*", "*.social.cshtml"});

            fileNames = fileNames.Select(fn => Path.GetFileNameWithoutExtension(fn)).ToList();
            fileNames.Sort();
            return fileNames;
        }

        /// <summary>
        /// Gets the view names available for a controller in a specified MVC area.
        /// </summary>
        /// <param name="controller">The controller directory whose views should be enumerated.</param>
        /// <param name="area">The MVC area containing the controller.</param>
        /// <returns>A sorted list of view names without the <c>.cshtml</c> extension.</returns>
        public static List<string> GetViewNames(string controller, string area)
        {
            var corePath = string.Format("/Areas/{0}/Views/{1}", area, controller);
            var path = HostingEnvironment.MapPath(string.Concat(HotcakesApplication.Current.ViewsVirtualPath, corePath));

            var fileNames = PathHelper.ListFiles(path, new[] {"*.cshtml"}, new[] {"_*", "*.social.cshtml"});

            fileNames = fileNames.Select(fn => Path.GetFileNameWithoutExtension(fn)).ToList();
            fileNames.Sort();
            return fileNames;
        }

        /// <summary>
        /// Gets the virtual path to the current portal's Hotcakes views directory.
        /// </summary>
        /// <returns>The portal-relative virtual path used for Hotcakes views.</returns>
        public static string GetViewsVirtualPath()
        {
            var homeDirectory = PortalSettings.Current.HomeDirectory;
            return VirtualPathUtility.Combine(homeDirectory, "HotcakesViews/");
        }
    }
}
