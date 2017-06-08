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
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.AppCode
{
    /// <summary>
    ///     This class contains helpers that enable a designer to perform more advanced tasks, such as registering scrips,
    ///     styles, and showing icons.
    /// </summary>
    [Serializable]
    public static class HtmlExtensions
    {
        private const string FILE_ICON_BASE_URL = "~/DesktopModules/Hotcakes/Core/Admin/Images/fileicons/";
        private const string DEFAULT_32 = "default_32.png";
        private const string EXTENSION = "_32.png";

        /// <summary>
        ///     Primarily used by the File Browser view, this allows you to return an icon representing the given file type.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fileName">The name of the file, such as MyCompany.pdf</param>
        /// <returns>The virtual path to the matching or default icon file</returns>
        public static string FileIconUrl(this HtmlHelper helper, string fileName)
        {
            var baseUrl = FILE_ICON_BASE_URL;

            var extension = string.Empty;

            try
            {
                extension = Path.GetExtension(fileName);
                extension = extension.TrimStart('.');
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return baseUrl + DEFAULT_32;
            }

            if (extension == string.Empty)
            {
                return baseUrl + DEFAULT_32;
            }

            if (HostingEnvironment.VirtualPathProvider.FileExists(baseUrl + extension + EXTENSION))
            {
                return baseUrl + extension + EXTENSION;
            }

            return baseUrl + DEFAULT_32;
        }

        /// <summary>
        ///     Allows you to register a JavaScript file to be automatically loaded in the page load using the Client Resource
        ///     Management API.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">The name of the JavaScript file, such as MyCompany.js</param>
        /// <param name="priorityOrder">The default is 20, but any value above 100 is safe.</param>
        /// <returns>This will always return string.empty</returns>
        /// <remarks>The script file must be present in the Scripts folder of your custom viewset.</remarks>
        public static string RegisterViewScript(this HtmlHelper helper, string path, int priorityOrder = 20)
        {
            var page = helper.ViewContext.HttpContext.Handler as Page;

            ClientResourceManager.RegisterScript(
                page,
                page.ResolveUrl(HotcakesApplication.Current.ViewsVirtualPath + "/Scripts/" + path),
                FileOrder.Js.DefaultPriority + priorityOrder);

            return string.Empty;
        }

        /// <summary>
        ///     Allows you to register a style sheet to be automatically loaded in the page load using the Client Resource
        ///     Management API.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">The name of the CSS file, such as MyCompany.css</param>
        /// <param name="priorityOrder">The default is 40, but any value above 40 is safe.</param>
        /// <returns>This will always return string.empty</returns>
        /// <remarks>
        ///     The style sheet must be present in the Styles folder of your custom viewset. If the Styles folder does not
        ///     exist, create it.
        /// </remarks>
        public static string RegisterViewStyleSheet(this HtmlHelper helper, string path, int priorityOrder = 40)
        {
            var page = helper.ViewContext.HttpContext.Handler as Page;

            ClientResourceManager.RegisterStyleSheet(
                page,
                page.ResolveUrl(HotcakesApplication.Current.ViewsVirtualPath + "/Styles/" + path),
                FileOrder.Css.DefaultPriority + priorityOrder);

            return string.Empty;
        }
    }
}