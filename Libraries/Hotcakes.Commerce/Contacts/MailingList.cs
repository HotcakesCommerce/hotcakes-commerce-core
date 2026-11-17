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
using System.Net.Mail;
using System.Text;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Represents a store-scoped mailing list and its in-memory members.</summary>
    public class MailingList
    {
        /// <summary>Initializes an empty list with default identifiers, timestamps, and member storage.</summary>
        public MailingList()
        {
            Id = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            Name = string.Empty;
            IsPrivate = false;
            Members = new List<MailingListMember>();
            StoreId = 0;
        }

        /// <summary>Gets or sets the list identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the UTC timestamp of the last list update.</summary>
        public DateTime LastUpdatedUtc { get; set; }
        /// <summary>Gets or sets the display name of the list.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets whether the list is private.</summary>
        public bool IsPrivate { get; set; }
        /// <summary>Gets or sets the members currently held by the list.</summary>
        public List<MailingListMember> Members { get; set; }
        /// <summary>Gets or sets the store identifier that owns the list.</summary>
        public long StoreId { get; set; }

        // Mailing List Send Functions
        /// <summary>
        /// Renders and sends the supplied template to each member. A null template is ignored;
        /// send failures are logged and do not stop subsequent members. The <paramref name="sendAsync"/>
        /// argument is retained for compatibility and does not alter the current implementation.
        /// </summary>
        /// <param name="t">The HTML template to render for each member.</param>
        /// <param name="sendAsync">A compatibility parameter indicating asynchronous sending.</param>
        /// <param name="app">The application context used for rendering and store mail delivery.</param>
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

        /// <summary>
        /// Creates a preview message using the first member and the supplied template.
        /// An empty <see cref="MailMessage"/> is returned when there is no member or template.
        /// </summary>
        /// <param name="t">The HTML template to render.</param>
        /// <param name="app">The application context used for rendering.</param>
        /// <returns>A rendered message addressed to the first member, or an empty message.</returns>
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
        /// <summary>Finds the single member whose email exactly matches the supplied value.</summary>
        /// <param name="email">The email address to match.</param>
        /// <returns>The matching member, or <see langword="null"/> when none exists.</returns>
        public MailingListMember FindMemberByEmail(string email)
        {
            var m = (from mem in Members
                where mem.EmailAddress == email
                select mem).SingleOrDefault();
            return m;
        }

        /// <summary>Determines whether a member with the exact email exists in this list.</summary>
        /// <param name="email">The email address to check.</param>
        /// <returns><see langword="true"/> when a matching member exists; otherwise, <see langword="false"/>.</returns>
        public bool CheckMembership(string email)
        {
            var m = FindMemberByEmail(email);
            if (m != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>Replaces the email address of the member matching the old address, when found.</summary>
        /// <param name="oldEmail">The existing email address.</param>
        /// <param name="newEmail">The replacement email address.</param>
        public void UpdateMemberEmail(string oldEmail, string newEmail)
        {
            var m = FindMemberByEmail(oldEmail);
            if (m != null)
            {
                m.EmailAddress = newEmail;
            }
        }

        /// <summary>Removes the member matching the supplied email address, when found.</summary>
        /// <param name="email">The email address to remove.</param>
        public void RemoveMemberByEmail(string email)
        {
            var m = FindMemberByEmail(email);
            if (m != null)
            {
                Members.Remove(m);
            }
        }

        /// <summary>Removes the member matching the supplied member identifier, when found.</summary>
        /// <param name="id">The member identifier to remove.</param>
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
        /// <summary>
        /// Exports members as newline-separated comma-delimited text. Email-only output contains one address
        /// per line; full output contains email, last name, and first name in that order.
        /// </summary>
        /// <param name="onlyExportEmail">Whether to omit names from each exported line.</param>
        /// <returns>The exported text, or an empty string for an empty list.</returns>
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

        /// <summary>
        /// Imports newline-separated comma-delimited member data and appends a new member for every non-null line.
        /// The first three columns are interpreted as email, last name, and first name; values are not trimmed or
        /// deduplicated, and the parsed list identifier is not copied to the appended member.
        /// </summary>
        /// <param name="inputText">The text to read line by line.</param>
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
