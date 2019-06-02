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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Plugins.FroogleFeed
{
    partial class Default : BaseAdminPage
    {
        private const string FroogleComponentId = "9E26F02A-975F-4206-9CB5-EC614FD6725F";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadSettings();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Froogle/GoogleBase Feed";
            CurrentTab = AdminTabType.Plugins;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        private void LoadSettings()
        {
            //this.gbaseUsernameField.Text = settingsManager.GetSetting("GoogleBaseUsername");
            //string pass = settingsManager.GetSetting("GoogleBasePassword");
            //if (pass.Trim().Length > 0) {
            //    this.gbasePasswordField.Text = "**********";
            //}
            //this.gbaseFileNameField.Text = settingsManager.GetSetting("GoogleBaseFileName");
            //if (this.gbaseFileNameField.Text == string.Empty) {
            //    this.gbaseFileNameField.Text = "Froogle.txt";
            //}
            //this.gbaseFtpField.Text = settingsManager.GetSetting("GoogleBaseFtp");
            //if (this.gbaseFtpField.Text == string.Empty) {
            //    this.gbaseFtpField.Text = "ftp://uploads.google.com";
            //}

            //string condition = settingsManager.GetSetting("Condition");
            //if (this.lstCondition.Items.FindByValue(condition) != null) {
            //    this.lstCondition.ClearSelection();
            //    this.lstCondition.Items.FindByValue(condition).Selected = true;
            //}
        }

        private void SaveSettings()
        {
            //settingsManager.SaveSetting("GoogleBaseUsername", this.gbaseUsernameField.Text.Trim(), "hcc", "GoogleBaseFeed", "");
            //if (this.gbasePasswordField.Text.Trim() != "**********") {
            //    settingsManager.SaveSetting("GoogleBasePassword", this.gbasePasswordField.Text.Trim(), "hcc", "GoogleBaseFeed", "");
            //}
            //settingsManager.SaveSetting("GoogleBaseFileName", this.gbaseFileNameField.Text.Trim(), "hcc", "GoogleBaseFeed", "");
            //settingsManager.SaveSetting("GoogleBaseFtp", this.gbaseFtpField.Text.Trim(), "hcc", "GoogleBaseFeed", "");
            //if (this.gbasePasswordField.Text != "") {
            //    this.gbasePasswordField.Text = "**********";
            //}
            //settingsManager.SaveSetting("Condition", this.lstCondition.SelectedItem.Value, "hcc", "GoogleBaseFeed", "");
        }

        private string SafeString(string input)
        {
            var result = input.Replace("\t", string.Empty);
            result = result.Replace(Environment.NewLine, " ");
            result = result.Replace("\r", " ");
            result = result.Replace("\n", " ");
            return result;
        }

        private string SafeBool(bool input)
        {
            if (input)
            {
                return "True";
            }
            return "False";
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            SaveSettings();
            BuildFeed();
        }

        private void BuildFeed()
        {
            OutputField.Text = string.Empty;

            var expirationDays = 30;
            var expDate = DateTime.Now.AddDays(expirationDays);
            var expirationDate = expDate.Year + "-";
            if (expDate.Month < 10)
            {
                expirationDate += "0";
            }
            expirationDate += expDate.Month + "-";
            if (expDate.Day < 10)
            {
                expirationDate += "0";
            }
            expirationDate += expDate.Day.ToString();

            var sb = new StringBuilder();
            WriteHeaders(ref sb);

            var prods = HccApp.CatalogServices.Products.FindAllPagedWithCache(1, 3000);
            foreach (var p in prods)
            {
                if (p.Status == ProductStatus.Active)
                {
                    WriteItem(p, ref sb, expirationDate);
                }
            }

            OutputField.Text = sb.ToString();
            lblStatus.Text = "Feed Generated!";
        }

        private void WriteHeaders(ref StringBuilder sb)
        {
            sb.Append("c:bvin:string\t");
            sb.Append("id\t");
            sb.Append("title\t");
            sb.Append("description\t");
            sb.Append("image_link\t");
            sb.Append("link\t");
            sb.Append("price\t");
            sb.Append("price_type\t");
            sb.Append("quantity\t");
            sb.Append("payment_accepted\t");
            sb.Append("payment_notes\t");
            sb.Append("shipping\t");
            sb.Append("expiration_date\t");
            sb.Append("actor\t");
            sb.Append("apparel_type\t");
            sb.Append("artist\t");
            sb.Append("author\t");
            sb.Append("brand\t");
            sb.Append("color\t");
            sb.Append("condition\t");
            sb.Append("model_number\t");
            sb.Append("format\t");
            sb.Append("isbn\t");
            sb.Append("memory\t");
            sb.Append("pickup\t");
            sb.Append("processor_speed\t");
            sb.Append("size\t");
            sb.Append("tax_percent\t");
            sb.Append("tax_region\t");
            sb.Append("upc\t");
            sb.Append("product_type\t");
            sb.Append("condition");
            sb.Append(Environment.NewLine);
        }

        private void WriteItem(Product p, ref StringBuilder sb, string expirationDate)
        {
            sb.Append(SafeString(p.Bvin));
            sb.Append("\t");
            sb.Append(SafeString(p.Sku));
            sb.Append("\t");
            sb.Append(SafeString(p.ProductName));
            sb.Append("\t");
            sb.Append(Text.TrimToLength(SafeString(p.LongDescription), 10000));
            sb.Append("\t");
            sb.Append(RemoteImageUrl(p.ImageFileMedium));
            sb.Append("\t");
            sb.Append(SafeString(RemoteUrl(p)));
            sb.Append("\t");
            sb.Append(SafeString(p.GetCurrentPrice(string.Empty, 0m, null, string.Empty).ToString()));
            sb.Append("\t");
            sb.Append(SafeString("starting"));
            // Price Type
            sb.Append("\t");
            sb.Append(SafeString("1"));
            sb.Append("\t");

            sb.Append(string.Empty);
            // payment_accepted - cash,check,GoogleCheckout,Visa,MasterCard,AmericanExpress,Discover,wiretransfer
            sb.Append("\t");
            sb.Append(string.Empty);
            // payment_notes
            sb.Append("\t");

            var props = HccApp.CatalogServices.ProductPropertiesFindForType(p.ProductTypeId);

            sb.Append(SafeString(PropertyMatcher("shipping", props, p.Bvin)));
            sb.Append("\t");

            var expirationDays = 30;
            var expDate = DateTime.Now.AddDays(expirationDays);
            sb.Append(SafeString(expirationDate));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("actor", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("apparel_type", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("artist", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("author", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("brand", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("color", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("condition", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("model_number", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("format", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("isbn", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("memory", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("pickup", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("processor_speed", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("size", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("tax_percent", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("tax_region", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("upc", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(PropertyMatcher("product_type", props, p.Bvin)));
            sb.Append("\t");
            sb.Append(SafeString(lstCondition.SelectedItem.Value));
            sb.Append(Environment.NewLine);
        }

        private string RemoteImageUrl(string localImage)
        {
            var temp = localImage.Replace("\\", "/");
            if (temp.ToLower().StartsWith("http://") == false)
            {
                return Path.Combine(HccApp.CurrentStore.RootUrl(), temp.TrimStart('/'));
            }
            return temp;
        }

        private string RemoteUrl(Product p)
        {
            var temp = UrlRewriter.BuildUrlForProduct(p);
            temp = temp.Replace("\\", "/");

            if (temp.ToLower().StartsWith("http://") == false)
            {
                return Path.Combine(HccApp.CurrentStore.RootUrl(), temp.TrimStart('/'));
            }
            return temp;
        }

        private string PropertyMatcher(string googleBaseName, List<ProductProperty> props, string productId)
        {
            var result = string.Empty;

            if (props != null)
            {
                for (var i = 0; i <= props.Count - 1; i++)
                {
                    if (props[i].PropertyName.Trim().ToLower() == googleBaseName.Trim().ToLower())
                    {
                        result = HccApp.CatalogServices.ProductPropertyValues.GetPropertyValue(productId, props[i]);
                        break;
                    }
                }
            }

            return result;
        }

        protected void btnGenerateAndSend_Click(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            SaveSettings();
            BuildFeed();
            SendFeed();
        }

        private void SendFeed()
        {
            try
            {
                var writerInfo = GetTemporaryFileInfo();
                File.WriteAllText(writerInfo.FullName, OutputField.Text.Trim());
                UploadFile(writerInfo.FullName, gbaseFileNameField.Text.Trim());
                lblStatus.Text = "Feed Generated and Sent via FTP!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        #region " URL Functions "

        private string CleanNameForUrl(string input)
        {
            var result = input;

            result = result.Replace(" ", "-");
            result = result.Replace("\"", string.Empty);
            result = result.Replace("&", "and");
            result = result.Replace("?", string.Empty);
            result = result.Replace("=", string.Empty);
            result = result.Replace("/", string.Empty);
            result = result.Replace("\\", string.Empty);
            result = result.Replace("%", string.Empty);
            result = result.Replace("#", string.Empty);
            result = result.Replace("*", string.Empty);
            result = result.Replace("!", string.Empty);
            result = result.Replace("$", string.Empty);
            result = result.Replace("+", "-plus-");
            result = result.Replace(",", "-");
            result = result.Replace("@", "-at-");
            result = result.Replace(":", "-");
            result = result.Replace(";", "-");
            result = result.Replace(">", string.Empty);
            result = result.Replace("<", string.Empty);
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("~", string.Empty);
            result = result.Replace("|", "-");
            result = result.Replace("^", string.Empty);
            result = result.Replace("[", string.Empty);
            result = result.Replace("]", string.Empty);
            result = result.Replace("`", string.Empty);
            result = result.Replace("'", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace(".", string.Empty);

            return result;
        }

        #endregion

        #region " File Helpers "

        private FileInfo GetTemporaryFileInfo()
        {
            string tempFileName;
            FileInfo myFileInfo;
            try
            {
                tempFileName = Path.GetTempFileName();
                myFileInfo = new FileInfo(tempFileName);
                myFileInfo.Attributes = FileAttributes.Temporary;
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return null;
            }
            return myFileInfo;
        }

        #endregion

        #region " FTP Classes "

        private bool UploadFile(string localFilename, string targetFilename)
        {
            //1. check source
            if (!File.Exists(localFilename))
            {
                throw new ApplicationException("File " + localFilename + " not found");
            }
            //copy to FI
            var fi = new FileInfo(localFilename);
            return Upload(fi, targetFilename);
        }

        private bool Upload(FileInfo fi, string targetFilename)
        {
            //copy the file specified to target file: target file can be full path or just filename (uses current dir)

            var host = gbaseFtpField.Text.Trim();
            if (host.EndsWith("/") == false)
            {
                host = host + "/";
            }
            var URI = host + targetFilename;
            //perform copy
            var ftp = GetRequest(URI);

            //Set request to upload a file in binary
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            ftp.UseBinary = true;

            //Notify FTP of the expected size
            ftp.ContentLength = fi.Length;

            //create byte array to store: ensure at least 1 byte!
            const int BufferSize = 2048;
            var content = new byte[BufferSize - 1];
            int dataRead;

            //open file for reading 
            using (var fs = fi.OpenRead())
            {
                try
                {
                    //open request to send
                    using (var rs = ftp.GetRequestStream())
                    {
                        do
                        {
                            dataRead = fs.Read(content, 0, BufferSize);
                            rs.Write(content, 0, dataRead);
                        } while (!(dataRead < BufferSize));
                    }
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                }
            }
            ftp = null;
            return true;
        }

        private FtpWebRequest GetRequest(string URI)
        {
            //create request
            var result = (FtpWebRequest) WebRequest.Create(URI);
            //Set the login details
            result.Credentials = GetCredentials();
            //Do not keep alive (stateless mode)
            result.KeepAlive = false;
            return result;
        }

        private ICredentials GetCredentials()
        {
            var pass = gbasePasswordField.Text.Trim();

            return new NetworkCredential(gbaseUsernameField.Text.Trim(), pass);
        }

        #endregion
    }
}