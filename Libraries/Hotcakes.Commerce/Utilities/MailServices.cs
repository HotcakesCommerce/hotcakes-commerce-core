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
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Utilities
{
    public class MailServices
    {
        private readonly Store _store = new Store();

        public MailServices(Store currentStore)
        {
            _store = currentStore;
        }

        public bool SendMail(MailMessage m)
        {
            var result = false;

            try
            {
                var configurationManager = Factory.CreateConfigurationManager();
                var smtpSettings = configurationManager.SmtpSettings;

                var _host = smtpSettings.Host;
                var _port = smtpSettings.Port;
                var _user = smtpSettings.Username;
                var _password = smtpSettings.Password;
                var _useAuth = smtpSettings.UseAuth;
                var _enableSsl = smtpSettings.EnableSsl;

                if (_store.Settings.MailServer.UseCustomMailServer)
                {
                    _host = _store.Settings.MailServer.HostAddress;
                    _user = _store.Settings.MailServer.Username;
                    _password = _store.Settings.MailServer.Password;
                    _useAuth = _store.Settings.MailServer.UseAuthentication;
                    _port = _store.Settings.MailServer.Port;
                    _enableSsl = _store.Settings.MailServer.UseSsl;
                }

                var server = new SmtpClient();
                if (!string.IsNullOrWhiteSpace(_host))
                {
                    server.Host = _host;
                }

                if (!string.IsNullOrWhiteSpace(_port))
                {
                    var altPort = 25;
                    if (int.TryParse(_port, out altPort))
                    {
                        server.Port = altPort;
                    }
                }

                if (_useAuth)
                {
                    server.UseDefaultCredentials = false;
                    server.Credentials = new NetworkCredential(_user, _password);
                }

                server.EnableSsl = _enableSsl;

                server.Send(m);

                server.Dispose();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Trace.Write(ex.Message);
                EventLog.LogEvent(ex);
            }

            return result;
        }

        public static bool SendMail(MailMessage m, Store fromStore)
        {
            if (m == null)
                return false;

            var sender = new MailServices(fromStore);
            return sender.SendMail(m);
        }

        public static string MailToLink(string email, string subject, string body)
        {
            return MailToLink(email, subject, body, email);
        }

        public static string MailToLink(string email, string subject, string body, string displayText)
        {
            var result = "<a href=\"mailto:";
            result += email;
            result += "?subject=" + subject.Replace(" ", "%20");
            result += "&body=" + body.Replace(" ", "%20");
            result += "\">" + displayText + "</a>";
            return result;
        }
    }
}