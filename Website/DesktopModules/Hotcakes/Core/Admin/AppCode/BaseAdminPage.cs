#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.Controls;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class BaseAdminPage : Page, IHccPage
    {
        #region Fields

        protected const string WizardPagePath = "~/DesktopModules/Hotcakes/Core/Admin/SetupWizard/SetupWizard.aspx";

        protected const string WizardPageStepPath =
            "~/DesktopModules/Hotcakes/Core/Admin/SetupWizard/SetupWizard.aspx?step={0}";

        protected const string DefaultCatalogPage = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Default.aspx";
        private readonly ArrayList _localizedControls = new ArrayList();

        #endregion

        #region Properties

        public string ResourceKeyName
        {
            get { return "resourcekey"; }
        }

        private string _localResourceFile;

        public virtual string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    var index = AppRelativeVirtualPath.LastIndexOf('/');
                    _localResourceFile = AppRelativeVirtualPath.Insert(index + 1, "App_LocalResources/");
                }
                return _localResourceFile;
            }
            set { _localResourceFile = value; }
        }

        public ILocalizationHelper Localization { get; set; }

        protected IMessageBox PageMessageBox { get; set; }

        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        public string PageTitle
        {
            get { return Title; }
            set { Title = value; }
        }

        public AdminTabType CurrentTab { get; set; }

        public virtual bool ForceWizardRedirect
        {
            get { return true; }
        }

        #endregion

        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            // We don't care about culuture of store at this time.
            // We simply need some non-localizable store settings to initialize thread culture.
            var currentStore = HccRequestContext.Current.CurrentStore;
            if (ForceWizardRedirect && currentStore == null && CurrentTab != AdminTabType.SetupWizard)
            {
                Response.Redirect(WizardPagePath);
            }

            HccRequestContextUtils.UpdateAdminContentCulture(HccRequestContext.Current);

            // Redirect to HTTPS if ssl required
            if (currentStore != null)
            {
                if (currentStore.Settings.ForceAdminSSL && !Request.IsSecureConnection)
                    SSL.SSLRedirect(SSL.SSLRedirectTo.SSL);

                CultureSwitch.SetCulture(currentStore);
            }

            ValidateBasicAccess();

            Localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            WebForms.MakePageNonCacheable(this);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            IterateControls(Controls, _localizedControls, Localization);
            RemoveKeyAttribute(_localizedControls);

            base.Render(writer);
        }

        #endregion

        #region Implementation

        private void ValidateBasicAccess()
        {
            if (!HccApp.MembershipServices.IsUserLoggedIn())
            {
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
            }
        }

        protected bool HasCurrentUserPermission(string permission)
        {
            return HccApp.MembershipServices.HasCurrentUserPermission(permission, HccApp);
        }

        protected void ValidateCurrentUserHasPermission(string permission)
        {
            if (!HasCurrentUserPermission(permission))
            {
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
            }
        }

        protected void ShowMessage(string message, ErrorTypes type)
        {
            switch (type)
            {
                case ErrorTypes.Ok:
                    PageMessageBox.ShowOk(message);
                    break;
                case ErrorTypes.Info:
                    PageMessageBox.ShowInformation(message);
                    break;
                case ErrorTypes.Error:
                    PageMessageBox.ShowError(message);
                    break;
                case ErrorTypes.Warning:
                    PageMessageBox.ShowWarning(message);
                    break;
            }
        }

        protected DateTime ConvertStartDateToUtc(DateTime? date)
        {
            if (date.HasValue)
                return DateHelper.ConvertStoreTimeToUtc(HccApp, date.Value.ZeroOutTime());
            return DateTime.UtcNow;
        }

        protected DateTime ConvertEndDateToUtc(DateTime? date)
        {
            if (date.HasValue)
                return DateHelper.ConvertStoreTimeToUtc(HccApp, date.Value.MaxOutTime());
            return DateTime.UtcNow;
        }

        #endregion

        #region Localization

        private void IterateControls(ControlCollection controls, ArrayList affectedControls,
            ILocalizationHelper localization)
        {
            foreach (Control control in controls)
            {
                ProcessControl(control, affectedControls, true, localization);
            }
        }

        private void ProcessControl(Control control, ArrayList affectedControls, bool includeChildren,
            ILocalizationHelper localization)
        {
            if (!control.Visible) return;

            //Perform the substitution if a key was found
            var key = GetControlAttribute(control, affectedControls, ResourceKeyName);
            if (!string.IsNullOrEmpty(key))
            {
                //Translation starts here ....
                var value = localization.GetString(key);
                if (control is Label)
                {
                    var label = (Label) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        label.Text = value;
                    }
                }
                if (control is LinkButton)
                {
                    var linkButton = (LinkButton) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var imgMatches = Regex.Matches(value,
                            "<(a|link|img|script|input|form).[^>]*(href|src|action)=(\\\"|'|)(.[^\\\"']*)(\\\"|'|)[^>]*>",
                            RegexOptions.IgnoreCase);
                        foreach (Match match in imgMatches)
                        {
                            if (match.Groups[match.Groups.Count - 2].Value.IndexOf("~", StringComparison.Ordinal) != -1)
                            {
                                var resolvedUrl = Page.ResolveUrl(match.Groups[match.Groups.Count - 2].Value);
                                value = value.Replace(match.Groups[match.Groups.Count - 2].Value, resolvedUrl);
                            }
                        }
                        linkButton.Text = value;
                        if (string.IsNullOrEmpty(linkButton.ToolTip))
                        {
                            linkButton.ToolTip = value;
                        }
                    }
                }
                if (control is HyperLink)
                {
                    var hyperLink = (HyperLink) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        hyperLink.Text = value;
                    }
                }
                if (control is ImageButton)
                {
                    var imageButton = (ImageButton) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        imageButton.AlternateText = value;
                    }
                }
                if (control is Button)
                {
                    var button = (Button) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        button.Text = value;
                    }
                }
                if (control is HtmlInputButton)
                {
                    var button = (HtmlInputButton) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        button.Value = value;
                    }
                }
                if (control is HtmlButton)
                {
                    var button = (HtmlButton) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        button.Attributes["Title"] = value;
                    }
                }
                if (control is HtmlImage)
                {
                    var htmlImage = (HtmlImage) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        htmlImage.Alt = value;
                    }
                }
                if (control is CheckBox)
                {
                    var checkBox = (CheckBox) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        checkBox.Text = value;
                    }
                    var toolTipValue = localization.GetString(key + ".ToolTip");
                    if (!string.IsNullOrEmpty(toolTipValue))
                    {
                        checkBox.ToolTip = toolTipValue;
                    }
                }
                if (control is BaseValidator)
                {
                    var baseValidator = (BaseValidator) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        baseValidator.Text = value;
                    }
                    var errorMessageValue = localization.GetString(key + ".ErrorMessage");
                    if (!string.IsNullOrEmpty(errorMessageValue))
                    {
                        baseValidator.ErrorMessage = errorMessageValue;
                    }
                }
                if (control is Image)
                {
                    var image = (Image) control;
                    if (!string.IsNullOrEmpty(value))
                    {
                        image.AlternateText = value;
                        image.ToolTip = value;
                    }
                }
            }

            //Translate listcontrol items here 
            if (control is ListControl)
            {
                var listControl = (ListControl) control;
                for (var i = 0; i <= listControl.Items.Count - 1; i++)
                {
                    var attributeCollection = listControl.Items[i].Attributes;
                    key = attributeCollection[ResourceKeyName];
                    if (key != null)
                    {
                        var value = localization.GetString(key);
                        if (!string.IsNullOrEmpty(value))
                        {
                            listControl.Items[i].Text = value;
                        }
                    }
                    if (key != null && affectedControls != null)
                    {
                        affectedControls.Add(attributeCollection);
                    }
                }
            }

            // translate table control header cells
            if (control is Table)
            {
                var table = (Table) control;
                foreach (var row in table.Rows)
                {
                    if (row is TableHeaderRow)
                    {
                        var thr = (TableHeaderRow) row;
                        foreach (var cell in thr.Cells)
                        {
                            var thc = (TableHeaderCell) cell;
                            var attributeCollection = thc.Attributes;
                            key = attributeCollection[ResourceKeyName];
                            if (key != null)
                            {
                                var value = localization.GetString(key);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    thc.Text = value;
                                }
                            }
                        }

                        return;
                    }
                }
            }

            //Process child controls
            if (includeChildren && control.HasControls())
            {
                var hccUserControl = control as HccUserControl;
                if (hccUserControl == null)
                {
                    //Pass Resource File Root through
                    IterateControls(control.Controls, affectedControls, localization);
                }
                else
                {
                    //Get Resource File Root from Controls LocalResourceFile Property
                    IterateControls(control.Controls, affectedControls, hccUserControl.Localization);
                }
            }
        }

        private string GetControlAttribute(Control control, ArrayList affectedControls, string attributeName)
        {
            AttributeCollection attributeCollection = null;
            string key = null;
            if (!(control is LiteralControl))
            {
                if (control is WebControl)
                {
                    var webControl = (WebControl) control;
                    attributeCollection = webControl.Attributes;
                    key = attributeCollection[attributeName];
                }
                else
                {
                    if (control is HtmlControl)
                    {
                        var htmlControl = (HtmlControl) control;
                        attributeCollection = htmlControl.Attributes;
                        key = attributeCollection[attributeName];
                    }
                    else
                    {
                        if (control is UserControl)
                        {
                            var userControl = (UserControl) control;
                            attributeCollection = userControl.Attributes;
                            key = attributeCollection[attributeName];
                        }
                        else
                        {
                            var controlType = control.GetType();
                            var attributeProperty = controlType.GetProperty("Attributes", typeof (AttributeCollection));
                            if (attributeProperty != null)
                            {
                                attributeCollection = (AttributeCollection) attributeProperty.GetValue(control, null);
                                key = attributeCollection[attributeName];
                            }
                        }
                    }
                }
            }
            if (key != null && affectedControls != null)
            {
                affectedControls.Add(attributeCollection);
            }
            return key;
        }

        private void RemoveKeyAttribute(ArrayList affectedControls)
        {
            if (affectedControls == null)
            {
                return;
            }
            int i;
            for (i = 0; i <= affectedControls.Count - 1; i++)
            {
                var ac = (AttributeCollection) affectedControls[i];
                ac.Remove(ResourceKeyName);
            }
        }

        #endregion
    }
}