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
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using ErrorTypes = Hotcakes.Modules.Core.Admin.AppCode.ErrorTypes;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
	public partial class Products_Import : BaseAdminPage
	{
		#region Fields

		protected string TempFilePath
		{
            get { return ViewState["TempFilePath"] as string; }
            set { ViewState["TempFilePath"] = value; }
		}

		#endregion

		#region Event Handlers

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			PageTitle = Localization.GetString("PageTitle");
			CurrentTab = AdminTabType.Catalog;
			ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			PageMessageBox = ucMessageBox;

			lnkLoadFile.Click += lnkLoadFile_Click;
			lnkStartImporting.Click += lnkStartImporting_Click;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

        private void lnkLoadFile_Click(object sender, EventArgs e)
		{
			if (fuExcelFile.PostedFile == null ||
				Path.GetExtension(fuExcelFile.PostedFile.FileName).ToUpper() != ".XLSX")
			{
				lblLoadInfo.Text = string.Empty;
			    chkImportOverride.Visible = false;
				lnkStartImporting.Visible = false;
				pnlImportResult.Visible = false;
				ShowMessage(Localization.GetString("InvalidFileType"), ErrorTypes.Error);
			}
			else
			{
				TempFilePath = DiskStorage.UploadTempFile(HccApp.CurrentStore.Id, fuExcelFile.PostedFile, ".XLSX");
				lblLoadInfo.Text = string.Format(Localization.GetString("ReadyToImport"), fuExcelFile.FileName);
			    chkImportOverride.Visible = true;
				lnkStartImporting.Visible = true;
				lnkStartImporting.Enabled = true;
				pnlImportResult.Visible = false;
			}
		}

        private void lnkStartImporting_Click(object sender, EventArgs e)
		{
            var asyncTask = Task.Factory.StartNew(
				DoImport,
                new ImportConfiguration
				{
					HccRequestContext = HccRequestContext.Current,
					HttpContext = Context,
					Session = Session,
					DnnPortalSettings = PortalSettings.Current
				});
			pnlImportResult.Visible = true;
			lnkStartImporting.Enabled = false;
			ScriptManager.RegisterStartupScript(this, GetType(), "hcGetProgress", "hcGetProgress();", true);
		}

		#endregion

		#region Implementation / import

		protected void DoImport(object objConfiguration)
		{
			var conf = objConfiguration as ImportConfiguration;

			HccRequestContext.Current = conf.HccRequestContext;
			DnnGlobal.SetPortalSettings(conf.DnnPortalSettings);
			Factory.HttpContext = conf.HttpContext;
			CultureSwitch.SetCulture(HccApp.CurrentStore, conf.DnnPortalSettings);

			var manager = new SessionManager(conf.Session);
			manager.AdminProductImportLog = null;
			manager.AdminProductImportProgress = 0;

			var catImport = new CatalogImport(HccApp);
			catImport.UpdateExistingProducts = chkImportOverride.Checked;

            // added import path so version 2.xx knows where import images are stored
            catImport.ImagesImportPath = string.Format("~/Portals/{0}/Hotcakes/Data/import/", conf.DnnPortalSettings.PortalId);

			catImport.ImportFromExcel(TempFilePath, (p, l) =>
				{
					lock (manager.AdminProductImportLog)
					{
                    manager.AdminProductImportProgress = Math.Round(p*100);

						if (!string.IsNullOrEmpty(l))
						{
							var log = manager.AdminProductImportLog;
							log.Add(l);
							manager.AdminProductImportLog = log;
						}
					}
				});
		}

		private class ImportConfiguration
		{
			public HccRequestContext HccRequestContext { get; set; }
			public HttpContext HttpContext { get; set; }
			public HttpSessionState Session { get; set; }
			public PortalSettings DnnPortalSettings { get; set; }
		}

		#endregion
	}
}