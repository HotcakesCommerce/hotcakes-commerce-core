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

#region Usings

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Hotcakes.Commerce.Dnn.Providers
{
    [Serializable]
    public class RewriterUtils
    {
        private static readonly Regex ServicesFrameworkRegex = new Regex("DesktopModules/[^/]+/API/");

       internal static void RewriteUrl(HttpContext context, string sendToUrl)
        {
            var x = string.Empty;
            var y = string.Empty;
            RewriteUrl(context, sendToUrl, ref x, ref y);
        }

        internal static void RewriteUrl(HttpContext context, string sendToUrl, ref string sendToUrlLessQString,
            ref string filePath)
        {
            //first strip the querystring, if any
            var queryString = string.Empty;
            sendToUrlLessQString = sendToUrl;
            if (sendToUrl.IndexOf("?") > 0)
            {
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf("?"));
                queryString = sendToUrl.Substring(sendToUrl.IndexOf("?") + 1);
            }

            //grab the file's physical path
            filePath = string.Empty;
            filePath = context.Server.MapPath(sendToUrlLessQString);

            //rewrite the path..
            context.RewritePath(sendToUrlLessQString, string.Empty, queryString);
            //NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
            //If you are using .NET Framework 1.0, use the below form instead:
            //context.RewritePath(sendToUrl);
        }

        internal static string ResolveUrl(string appPath, string url)
        {
            //String is Empty, just return Url
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            //String does not contain a ~, so just return Url
            if (url.StartsWith("~") == false)
            {
                return url;
            }

            //There is just the ~ in the Url, return the appPath
            if (url.Length == 1)
            {
                return appPath;
            }
            var seperatorChar = url.ToCharArray()[1];
            if (seperatorChar == '/' || seperatorChar == '\\')
            {
                //Url looks like ~/ or ~\
                if (appPath.Length > 1)
                {
                    return appPath + "/" + url.Substring(2);
                }
                return "/" + url.Substring(2);
            }
            //Url look like ~something
            if (appPath.Length > 1)
            {
                return appPath + "/" + url.Substring(1);
            }
            return appPath + url.Substring(1);
        }

        internal static bool OmitFromRewriteProcessing(string localPath, IHostSettingsService hostSettingsService, IApplicationStatusInfo applicationStatusInfo)
        {
            var omitSettings = string.Empty;
            if (applicationStatusInfo.Status == UpgradeStatus.None)
            {
                omitSettings = hostSettingsService.GetString("OmitFromRewriteProcessing");
            }

            if (string.IsNullOrEmpty(omitSettings))
            {
                omitSettings = "scriptresource.axd|webresource.axd|gif|ico|jpg|jpeg|png|css|js";
            }
            omitSettings = omitSettings.ToLower();
            localPath = localPath.ToLower();

            var omissions = omitSettings.Split('|');

            var shouldOmit = omissions.Any(x => localPath.EndsWith(x));

            if (!shouldOmit)
            {
                shouldOmit = ServicesFrameworkRegex.IsMatch(localPath);
            }

            return shouldOmit;
        }
    }
}