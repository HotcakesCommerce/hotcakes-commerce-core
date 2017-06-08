using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment.Gateways;

namespace Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.Shift4
{

	partial class Edit : HccCreditCardGatewayPart
	{
		public override void LoadData()
		{
			Shift4Settings settings = new Shift4Settings();
			settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

			chkUtgSecure.Checked = settings.UtgSecure;
			txtUtgServer.Text = settings.UtgIp;
			txtUtgPort.Text = settings.UtgPort;
			txtSerialNumber.Text = settings.SerialNumber;
			txtUsername.Text = settings.Username;
			txtPassword.Text = settings.Password;
			txtMerchantId.Text = settings.MerchantId;
			txtVendor.Text = settings.Vendor;
			chkDebugMode.Checked = settings.DebugMode;
		}

		public override void SaveData()
		{
			Shift4Settings settings = new Shift4Settings();
			settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

			settings.UtgSecure = chkUtgSecure.Checked;
			settings.UtgIp = txtUtgServer.Text.Trim();
			settings.UtgPort = txtUtgPort.Text.Trim();
			settings.SerialNumber = txtSerialNumber.Text.Trim();
			settings.Username = txtUsername.Text.Trim();
			settings.Password = txtPassword.Text.Trim();
			settings.MerchantId = txtMerchantId.Text.Trim();
			settings.Vendor = txtVendor.Text.Trim();
			settings.DebugMode = chkDebugMode.Checked;

			HccApp.CurrentStore.Settings.PaymentSettingsSet(GatewayId, settings);
			HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
		}

	}
}
