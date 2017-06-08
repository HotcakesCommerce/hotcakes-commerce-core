using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes.Payment.Gateways
{
	[Serializable]
	public class Shift4Settings : MethodSettings
	{
		public string SerialNumber
		{
			get { return GetSettingOrEmpty("SerialNumber"); }
			set { this.AddOrUpdate("SerialNumber", value); }
		}
		public string Username
		{
			get { return GetSettingOrEmpty("Username"); }
			set { this.AddOrUpdate("Username", value); }
		}
		public string Password
		{
			get { return GetSettingOrEmpty("Password"); }
			set { this.AddOrUpdate("Password", value); }
		}
		public string MerchantId
		{
			get { return GetSettingOrEmpty("MerchantId"); }
			set { this.AddOrUpdate("MerchantId", value); }
		}
		public string Vendor
		{
			get { return GetSettingOrEmpty("Vendor"); }
			set { this.AddOrUpdate("Vendor", value); }
		}
		public bool UtgSecure
		{
			get { return GetBoolSetting("UtgSecure"); }
			set { this.SetBoolSetting("UtgSecure", value); }
		}
		public string UtgIp
		{
			get { return GetSettingOrEmpty("UtgIp"); }
			set { this.AddOrUpdate("UtgIp", value); }
		}
		public string UtgPort
		{
			get { return GetSettingOrEmpty("UtgPort"); }
			set { this.AddOrUpdate("UtgPort", value); }
		}
		public bool DebugMode
		{
			get { return GetBoolSetting("DebugMode"); }
			set { SetBoolSetting("DebugMode", value); }
		}
	}
}
