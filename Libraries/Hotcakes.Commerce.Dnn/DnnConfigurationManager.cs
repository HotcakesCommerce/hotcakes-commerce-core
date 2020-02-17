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
using DotNetNuke.Entities.Host;
using Hotcakes.Commerce.Configuration;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnConfigurationManager : IConfigurationManager
    {
        public SmtpSettings SmtpSettings
        {
            get
            {
                var smtpSettings = new SmtpSettings();

                var smtpHostParts = Host.SMTPServer.Split(':');
                smtpSettings.Host = smtpHostParts[0];
                if (smtpHostParts.Length > 1)
                {
                    smtpSettings.Port = smtpHostParts[1];
                }

                switch (Host.SMTPAuthentication)
                {
                    case "":
                    case "0": //anonymous
                        smtpSettings.UseAuth = false;
                        break;
                    case "1": //basic
                        smtpSettings.UseAuth = true;
                        break;
                    case "2": //NTLM
                        smtpSettings.UseAuth = false;
                        break;
                }

                smtpSettings.EnableSsl = Host.EnableSMTPSSL;
                smtpSettings.Username = Host.SMTPUsername;
                smtpSettings.Password = Host.SMTPPassword;

                return smtpSettings;
            }
        }
    }
}