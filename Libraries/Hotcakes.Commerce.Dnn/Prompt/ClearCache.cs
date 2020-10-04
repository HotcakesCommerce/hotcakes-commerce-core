#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Linq;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Prompt
{
    [ConsoleCommand("clear-store-cache", Constants.Namespace, "PromptClearCache")]
    public class ClearCache: PromptBase, IConsoleCommand
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ClearCache));

        public override ConsoleResultModel Run()
        {
            try
            {
                var messages = new List<PromptMessage>();
                var portals = PortalController.Instance.GetPortals();

                if (portals == null || portals.Count == 0)
                {
                    messages.Add(new PromptMessage(LocalizeString("NoPortalsFound")));
                }
                else
                {
                    messages.Add(new PromptMessage(string.Format(LocalizeString("PortalsFound"), portals.Count)));
                }

                foreach (PortalInfo portal in portals)
                {
                    messages.Add(new PromptMessage(string.Format(LocalizeString("ClearingImages"), portal.PortalName)));

                    var folderPath = string.Format("\\Portals\\{0}\\Hotcakes\\Data\\temp\\", portal.PortalID);
                    var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(folderPath);

                    var tempFolder = new System.IO.DirectoryInfo(fullPath);

                    if (tempFolder != null && tempFolder.Exists)
                    {
                        // delete everything inside of the folder, but leave the folder in place
                        tempFolder.Empty();
                    }
                }

                // clearing DNN CRM cache
                ClientResourceManager.ClearCache();
                messages.Add(new PromptMessage(LocalizeString("ClearedCrmCache")));

                // clearing cache in each portal
                foreach (PortalInfo portal in portals)
                {
                    DataCache.ClearPortalCache(portal.PortalID, true);
                    messages.Add(new PromptMessage(string.Format(LocalizeString("ClearedPortalCache"), portal.PortalName)));
                }

                // clear DNN cache 
                DataCache.ClearCache();
                DotNetNuke.Web.Client.ClientResourceManagement.ClientResourceManager.ClearCache();
                messages.Add(new PromptMessage(LocalizeString("ClearedDnnCache")));

                // clear HCC cache here
                CacheManager.ClearAll();
                messages.Add(new PromptMessage(LocalizeString("ClearedHccCache")));

                return new ConsoleResultModel
                {
                    Data = messages,
                    Output = string.Concat(Constants.OutputPrefix, string.Format(LocalizeString("ClearedCache"))) 
                };
            }
            catch (Exception e)
            {
                LogError(e);
                return new ConsoleErrorResultModel(string.Concat(Constants.OutputPrefix, LocalizeString("ErrorOccurred")));
            }
        }

        protected override void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);
                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }
    }
}