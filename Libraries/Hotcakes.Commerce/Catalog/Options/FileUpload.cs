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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    public class FileUpload : IOptionProcessor
    {
        public OptionTypes GetOptionType()
        {
            return OptionTypes.FileUpload;
        }

        public string Render(Option baseOption)
        {
            return RenderWithSelection(baseOption, null);
        }

        public string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null)
        {
            var uploadControlHtml = GetUploadControlHtml(baseOption, prefix);
            return uploadControlHtml;
        }

        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var li = new LiteralControl();
            var uploaderHtml = GetUploadControlHtml(baseOption, prefix);
            li.Text = uploaderHtml;
            ph.Controls.Add(li);
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            var li = (LiteralControl)ph.FindControl("opt" + prefix + baseOption.Bvin.Replace("-", string.Empty));
            if (li != null)
            {
                result.SelectionData = li.Text.Trim();
            }

            return result;
        }

        public OptionSelection ParseFromForm(Option baseOption, NameValueCollection form, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;
            var formid = "opt" + prefix + baseOption.Bvin.Replace("-", string.Empty);
            var value = form[formid];

            if (value != null)
            {
                //TODO:
                //Setter of OptionSelection.SelectionData cleans BVIN which also replaces all '-' (dash) from rest of string
                //As temporary fix replace the '-' (dash) with invalid file character (?) and reaplce it in CartDescription function
                var uploadedFiles = value.Trim().Replace('-', '?');
                result.SelectionData = uploadedFiles;
            }
            return result;
        }

        public void SetSelectionsInPlaceholder(Option baseOption, PlaceHolder ph, OptionSelectionList selections)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Used to show the option values in view order
        /// </summary>
        /// <param name="baseOption"></param>
        /// <param name="selections"></param>
        /// <returns></returns>
        public string CartDescription(Option baseOption, OptionSelectionList selections)
        {
            if (selections == null)
                return string.Empty;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(val.SelectionData))
            {
                var files = val.SelectionData.Split('|').ToList();

                if (files.Count <= 0)
                    return string.Empty;

                var downloadLinks = string.Empty;

                foreach (var file in files)
                {
                    if (string.IsNullOrWhiteSpace(file))
                        continue;

                    var fileParam = file.Split('<');


                    //Temporary fix : Revert back of fix - Replace character ? (questionmark) with - (dash)
                    //See ParseFromForm function for root cause
                    var fileDownloadPath = fileParam[0].Replace('?', '-');


                    var fileSize = fileParam[1];
                    var fileName = Path.GetFileName(fileDownloadPath);

                    var fileHtml = "<li>" +
                                   "<a href=\"" + fileDownloadPath + "\" target=\"_blank\" class=\"fileDownLoadLink\" >" +
                                   fileName + "</a><span class=\"fileSize\" > (" + fileSize + ")</span>" +
                                   "</li>";
                    downloadLinks += fileHtml;
                }

                var bvin = baseOption.Bvin.Replace("-", string.Empty);
                downloadLinks = "<br>" +
                                "<ul class=\"fileDownLoadLinks\" >" +
                                downloadLinks +
                                "</ul>";
                return baseOption.Name + ": " + downloadLinks; //System.Web.HttpUtility.HtmlDecode(val.SelectionData);
            }

            return string.Empty;
        }

        public List<string> GetSelectionValues(Option baseOption, OptionSelectionList selections)
        {
            var retVal = new List<string>();
            if (selections == null)
            {
                return retVal;
            }

            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null)
            {
                return retVal;
            }

            if (!string.IsNullOrWhiteSpace(val.SelectionData))
            {
                var files = val.SelectionData.Split('|').ToList();

                if (files.Count <= 0)
                    return retVal;
                var downloadLinks = string.Empty;

                foreach (var file in files)
                {
                    if (string.IsNullOrWhiteSpace(file))
                        continue;

                    var fileParam = file.Split('<');

                    //Temporary fix : Revert back of fix - Replace character ? (questionmark) with - (dash)
                    //See ParseFromForm function for root cause
                    var fileDownloadPath = fileParam[0].Replace('?', '-');

                    var fileSize = fileParam[1];
                    var fileName = Path.GetFileName(fileDownloadPath);

                    var fileDetails = fileName + " (" + fileSize + ") ";
                    retVal.Add(fileDetails);
                }

                return retVal;
            }

            return retVal;
        }

        public void SetMultipleFiles(Option baseOption, bool value)
        {
            baseOption.Settings.AddOrUpdate("allowmultiplefiles", value.ToString().ToLower());
        }

        public bool GetMultipleFiles(Option baseOption)
        {
            var allowMultiUplaod = true;
            bool.TryParse(baseOption.Settings.GetSettingOrEmpty("allowmultiplefiles"), out allowMultiUplaod);
            return allowMultiUplaod;
        }


        private string GetUploadControlHtml(Option baseOption, string prefix)
        {
            var bvin = baseOption.Bvin.Replace("-", string.Empty);
            var optUniqueId = prefix + bvin;
            var hostSettings = Factory.Instance.CreateHostSettingsProvider();

            var supportedTypes = hostSettings.GetSettingValue("FileExtensions", string.Empty);
            var allowMultipleFiles = baseOption.Settings.Keys.Contains("allowmultiplefiles")
                ? baseOption.Settings["allowmultiplefiles"]
                : string.Empty;

            var sb = new StringBuilder();

            var uploadLink = "<div id=\"container_" + optUniqueId + "\" class=\"uploadContainer\">" +
                             "<div class=\"fileListHeader\" >" +
                             "<h5>Select files</h5>" +
                             "<p>" +
                             "<span>Add files to upload queue and click the start button</span>" +
                             "</p>" +
                             "</div>" +
                             "<div id=\"filelist_" + optUniqueId + "\" class=\"fileListContainer\">" +
                             "<ul class=\"fileList\" ></ul>" +
                             "</div>" +
                             "<br/>" +
                             "<a id=\"pickfiles_" + optUniqueId +
                             "\" href=\"#\" class=\"dnnSecondaryAction browseFiles\">Add files</a>" +
                             "<a id=\"uploadfiles_" + optUniqueId +
                             "\" href=\"#\" class=\"dnnSecondaryAction uploadFiles\">Start upload</a>" +
                             "<input type=\"hidden\" class=\"allowmultiplefiles\" value=\"" + allowMultipleFiles +
                             "\" />" +
                             "</div>" +
                             "<input type=\"hidden\" name=\"uploadfiletypes\" class=\"\" value=\"" + supportedTypes +
                             "\" />" +
                             "<input type=\"hidden\" name=\"opt" + optUniqueId + "\" id=\"opt" + optUniqueId + "\" />";

            var wrapper = "<div class=\"fileUploadWrapper\">" + uploadLink + "</div>";

            sb.Append(wrapper);
            return sb.ToString();
        }
    }
}