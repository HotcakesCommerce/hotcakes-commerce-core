#region License

// Distributed under the MIT License
// ============================================================
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
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core
{
    public class ClearUploadTempFiles : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ClearUploadTempFiles));

        public ClearUploadTempFiles(ScheduleHistoryItem item)
        {
            ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            try
            {
                Progressing();

                Logger.Debug("ClearUploadTempFiles Running");

                var portals = PortalController.Instance.GetPortals();

                if (portals == null || portals.Count == 0)
                {
                    Logger.Debug("No portals were found.");
                }
                else
                {
                    Logger.DebugFormat("Found {0} portals to attempt to process.", portals.Count);
                }

                foreach (PortalInfo portal in portals)
                {
                    Logger.DebugFormat("Processing Portal: {0}", ((IPortalInfo)portal).PortalId);

                    var folderPath = string.Format("\\Portals\\{0}\\Hotcakes\\Data\\temp\\", ((IPortalInfo)portal).PortalId);
                    var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(folderPath);

                    Logger.DebugFormat("folderPath: {0}", folderPath);
                    Logger.DebugFormat("fullPath: {0}", fullPath);

                    var tempFolder = new System.IO.DirectoryInfo(fullPath);

                    if (tempFolder != null && tempFolder.Exists)
                    {
                        Logger.Debug("Recursively deleting the contents of the folder.");
                        // delete everything inside of the folder, but leave the folder in place
                        tempFolder.Empty();
                    }
                }

                Logger.Debug("ClearUploadTempFiles Completed");

                //Show success
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                LogError(ex.Message, ex);
                Errored(ref ex);
                Exceptions.LogException(ex);
            }
        }

        #region Logging
        private void LogError(string message, Exception exc)
        {
            try
            {
                if (exc != null)
                {
                    Logger.Error(message, exc);
                    if (exc.InnerException != null)
                    {
                        LogError(exc.InnerException.Message, exc.InnerException);
                    }
                }
                else
                {
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }
        #endregion
    }
}