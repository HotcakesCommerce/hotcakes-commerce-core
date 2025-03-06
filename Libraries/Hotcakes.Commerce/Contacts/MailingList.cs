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
using System.Linq;
using System.Net.Mail;
using System.Text;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Contacts
{
    public class MailingList
    {
        public MailingList()
        {
            Id = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            Name = string.Empty;
            IsPrivate = false;
            Members = new List<MailingListMember>();
            StoreId = 0;
        }

        public long Id { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public List<MailingListMember> Members { get; set; }
        public long StoreId { get; set; }

        // Mailing List Send Functions
        public void SendToList(HtmlTemplate t, bool sendAsync, HotcakesApplication app)
        {
            if (t == null)
                return;

            foreach (var m in Members)
            {
                try
                {
                    var copy = t.ReplaceTagsInTemplate(app.CurrentRequestContext, m);
                    var msg = copy.ConvertToMailMessage(m.EmailAddress);
                    MailServices.SendMail(msg, app.CurrentStore);
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                }
            }
        }

        public MailMessage PreviewMessage(HtmlTemplate t, HotcakesApplication app)
        {
            var result = new MailMessage();

            if (Members.Count > 0)
            {
                if (t != null)
                {
                    var copy = t.ReplaceTagsInTemplate(app.CurrentRequestContext, Members[0]);
                    result = copy.ConvertToMailMessage(Members[0].EmailAddress);
                }
            }

            return result;
        }

        // Member Functions
        public MailingListMember FindMemberByEmail(string email)
        {
            var m = (from mem in Members
                where mem.EmailAddress == email
                select mem).SingleOrDefault();
            return m;
        }

        public bool CheckMembership(string email)
        {
            var m = FindMemberByEmail(email);
            if (m != null)
            {
                return true;
            }
            return false;
        }

        public void UpdateMemberEmail(string oldEmail, string newEmail)
        {
            var m = FindMemberByEmail(oldEmail);
            if (m != null)
            {
                m.EmailAddress = newEmail;
            }
        }

        public void RemoveMemberByEmail(string email)
        {
            var m = FindMemberByEmail(email);
            if (m != null)
            {
                Members.Remove(m);
            }
        }

        public void RemoveMemberById(long id)
        {
            var m = (from mem in Members
                where mem.Id == id
                select mem).SingleOrDefault();
            if (m != null)
            {
                Members.Remove(m);
            }
        }

        // Import Export Functions
        public string ExportToCommaDelimited(bool onlyExportEmail)
        {
            var result = string.Empty;

            var sb = new StringBuilder();
            for (var i = 0; i <= Members.Count - 1; i++)
            {
                sb.Append(Members[i].EmailAddress);
                if (onlyExportEmail == false)
                {
                    sb.Append(", ");
                    sb.Append(Members[i].LastName);
                    sb.Append(", ");
                    sb.Append(Members[i].FirstName);
                }
                sb.Append("\n");
            }
            result = sb.ToString();

            return result;
        }

        public void ImportFromCommaDelimited(string inputText)
        {
            var sw = new StringReader(inputText);
            var splitCharacter = ",";
            var lineToProcess = string.Empty;
            lineToProcess = sw.ReadLine();

            while (lineToProcess != null)
            {
                var lineValues = lineToProcess.Split(splitCharacter.ToCharArray());
                if (lineValues.Length > 0)
                {
                    var mm = new MailingListMember();
                    mm.EmailAddress = lineValues[0];
                    mm.ListId = Id;
                    if (lineValues.Length > 1)
                    {
                        mm.LastName = lineValues[1];
                    }
                    if (lineValues.Length > 2)
                    {
                        mm.FirstName = lineValues[2];
                    }
                    Members.Add(new MailingListMember
                    {
                        LastName = mm.LastName,
                        FirstName = mm.FirstName,
                        EmailAddress = mm.EmailAddress
                    });
                }
                lineToProcess = sw.ReadLine();
            }
            sw.Dispose();
        }
    }
}