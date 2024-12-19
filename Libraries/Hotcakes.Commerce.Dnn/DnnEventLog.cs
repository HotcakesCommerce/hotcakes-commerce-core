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
using DotNetNuke.Abstractions.Logging;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using Hotcakes.Web.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnEventLog : PortalModuleBase, ILogger
    {
        private const string LOGTYPE_MESSAGE = "HOTCAKES_INFO";
        private const string LOGTYPE_EXCEPTION = "HOTCAKES_EXCEPTION";
        private readonly IEventLogger eventLogger;
        private readonly LogInfo LogInfo;
        private ILogInfo logInfo => LogInfo;

        public DnnEventLog()
        {
            eventLogger = DependencyProvider.GetRequiredService<IEventLogger>();
            LogInfo = new LogInfo();          
        }

        public void LogMessage(string message)
        {
            LogMessage("Hotcakes", message, EventLogSeverity.Information);
        }

        public void LogException(Exception ex)
        {
            LogException(ex, EventLogSeverity.Error);
        }

        public void LogMessage(string source, string message, EventLogSeverity severity)
        {
            var user = DnnUserController.Instance.GetCurrentUserInfo();
            logInfo.LogUserId = user != null ? user.UserID : -1;
            logInfo.LogPortalId = PortalSettings.Current != null ? PortalSettings.Current.PortalId : -1;
            logInfo.LogTypeKey = LOGTYPE_MESSAGE;
            logInfo.AddProperty("Severity", severity.ToString());
            logInfo.LogProperties.Add(new LogDetailInfo("Source", source));
            logInfo.LogProperties.Add(new LogDetailInfo("Message", message));

            eventLogger.AddLog(logInfo);
        }

        public void LogException(Exception ex, EventLogSeverity severity)
        {
            var user = DnnUserController.Instance.GetCurrentUserInfo();

            logInfo.LogUserId = user != null ? user.UserID : -1;
            logInfo.LogPortalId = DnnGlobal.Instance.GetPortalId();
            logInfo.LogTypeKey = LOGTYPE_EXCEPTION;
            logInfo.AddProperty("Severity", severity.ToString());

            var exception4 = new BasePortalException(ex.ToString(), ex);
            logInfo.LogProperties.Add(new LogDetailInfo("AssemblyVersion", exception4.AssemblyVersion));
            logInfo.LogProperties.Add(new LogDetailInfo("PortalID", exception4.PortalID.ToString()));
            logInfo.LogProperties.Add(new LogDetailInfo("PortalName", exception4.PortalName));
            logInfo.LogProperties.Add(new LogDetailInfo("UserID", exception4.UserID.ToString()));
            logInfo.LogProperties.Add(new LogDetailInfo("UserName", exception4.UserName));
            logInfo.LogProperties.Add(new LogDetailInfo("ActiveTabID", exception4.ActiveTabID.ToString()));
            logInfo.LogProperties.Add(new LogDetailInfo("ActiveTabName", exception4.ActiveTabName));
            logInfo.LogProperties.Add(new LogDetailInfo("RawURL", exception4.RawURL));
            logInfo.LogProperties.Add(new LogDetailInfo("AbsoluteURL", exception4.AbsoluteURL));
            logInfo.LogProperties.Add(new LogDetailInfo("AbsoluteURLReferrer", exception4.AbsoluteURLReferrer));
            logInfo.LogProperties.Add(new LogDetailInfo("UserAgent", exception4.UserAgent));
            logInfo.LogProperties.Add(new LogDetailInfo("DefaultDataProvider", exception4.DefaultDataProvider));
            logInfo.LogProperties.Add(new LogDetailInfo("ExceptionGUID", exception4.ExceptionGUID));
            logInfo.LogProperties.Add(new LogDetailInfo("InnerException", exception4.InnerException.Message));
            logInfo.LogProperties.Add(new LogDetailInfo("FileName", exception4.FileName));
            logInfo.LogProperties.Add(new LogDetailInfo("FileLineNumber", exception4.FileLineNumber.ToString()));
            logInfo.LogProperties.Add(new LogDetailInfo("FileColumnNumber", exception4.FileColumnNumber.ToString()));
            logInfo.LogProperties.Add(new LogDetailInfo("Method", exception4.Method));
            logInfo.LogProperties.Add(new LogDetailInfo("StackTrace", exception4.StackTrace));
            logInfo.LogProperties.Add(new LogDetailInfo("Message", exception4.Message));
            logInfo.LogProperties.Add(new LogDetailInfo("Source", exception4.Source));

            eventLogger.AddLog(logInfo);
        }

        public static void InstallLogTypes()
        {
            var controller = new LogController();
            RegisterLogType(LOGTYPE_MESSAGE, "Hotcakes Info", "hcLogInfo", controller);
            RegisterLogType(LOGTYPE_EXCEPTION, "Hotcakes Exception", "hcLogError", controller);
        }

        private static void RegisterLogType(string logTypeKey, string logTypeFriendlyName, string cssClass,
            LogController controller)
        {
            if (controller.GetLogTypeInfoDictionary().ContainsKey(logTypeKey))
            {
                //Add LogType
                var logTypeInfo = controller.GetLogTypeInfoDictionary()[logTypeKey];
                logTypeInfo.LogTypeFriendlyName = logTypeFriendlyName;
                logTypeInfo.LogTypeDescription = string.Empty;
                logTypeInfo.LogTypeCSSClass = cssClass;
                logTypeInfo.LogTypeOwner = "Hotcakes.Logging.EventLogType";
                controller.UpdateLogType(logTypeInfo);
            }
            else
            {
                //Add LogType
                var logTypeInfo = new LogTypeInfo
                {
                    LogTypeKey = logTypeKey,
                    LogTypeFriendlyName = logTypeFriendlyName,
                    LogTypeDescription = string.Empty,
                    LogTypeCSSClass = cssClass,
                    LogTypeOwner = "Hotcakes.Logging.EventLogType"
                };
                controller.AddLogType(logTypeInfo);

                //Add LogType
                var logTypeConf = new LogTypeConfigInfo
                {
                    LoggingIsActive = true,
                    LogTypeKey = logTypeKey,
                    KeepMostRecent = "100",
                    NotificationThreshold = 1,
                    NotificationThresholdTime = 1,
                    NotificationThresholdTimeType = LogTypeConfigInfo.NotificationThresholdTimeTypes.Seconds,
                    MailFromAddress = Null.NullString,
                    MailToAddress = Null.NullString,
                    LogTypePortalID = "*"
                };

                controller.AddLogTypeConfigInfo(logTypeConf);
            }
        }
    }
}