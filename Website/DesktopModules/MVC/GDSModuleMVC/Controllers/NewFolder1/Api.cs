using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class Api
	{
		public string UsreName { get; set; }
		public string Password { get; set; }
		public string ApiKey { get; set; }
		public string SessionID { get; set; }
		public object Request { get; private set; }
		public bool Success { get; set; }
		public int StatusCode { get; set; }
		public string WCURL { get; set; }
		public Api()
		{
		}
		public Api(string UserName, string Password, string ApiKey, string SessionID, string WCURL)
		{
			this.UsreName = UserName;
			this.Password = Password;
			this.ApiKey = ApiKey;
			this.SessionID = SessionID;
			this.WCURL = WCURL;
		}
		public Api InitializeApi(int portalID)
		{
			ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(portalID, "GDSModuleMVC");
			ModuleController moduleController = new ModuleController();
			Hashtable tabModuleSettings = moduleController.GetTabModuleSettings(moduleByDefinition.TabModuleID);
			Hashtable tabModuleSettings2 = moduleByDefinition.TabModuleSettings;
			Api api = new Api();
			api.Success = false;
			bool flag = tabModuleSettings["Username"] != null && tabModuleSettings["Password"] != null && tabModuleSettings["ApiKey"] != null && tabModuleSettings["SessionID"] != null && tabModuleSettings["WCURL"] != null;
			Api result;
			if (flag)
			{
				api = new Api(tabModuleSettings["Username"].ToString(), tabModuleSettings["Password"].ToString(), tabModuleSettings["ApiKey"].ToString(), tabModuleSettings["SessionID"].ToString(), tabModuleSettings["WCURL"].ToString());
				Api api2 = api.IsValid();
				bool flag2 = api2.Success && api2.StatusCode != 0;
				if (flag2)
				{
					ModuleController moduleController2 = new ModuleController();
					resplogin resplogin = api.Execute("login", JsonConvert.SerializeObject(new reqlogin
					{
						user = tabModuleSettings["Username"].ToString(),
						password = tabModuleSettings["Password"].ToString()
					})).ToObject<resplogin>();
					bool status = resplogin.status;
					if (status)
					{
						api.Success = true;
						moduleController2.UpdateModuleSetting(moduleByDefinition.ModuleID, "SessionID", resplogin.response.sessionId);
						moduleController2.UpdateTabModuleSetting(moduleByDefinition.TabModuleID, "SessionID", resplogin.response.sessionId);
						api.SessionID = resplogin.response.sessionId;
					}
					else
					{
						bool flag3 = resplogin.errorCode == 1101;
						if (flag3)
						{
							api.Success = false;
							api.StatusCode = 1101;
						}
					}
				}
				else
				{
					bool flag4 = api2.Success && api2.StatusCode == 0;
					if (flag4)
					{
						api.StatusCode = 0;
						api.Success = true;
					}
					else
					{
						api.StatusCode = 2;
						api.Success = false;
					}
				}
				result = api;
			}
			else
			{
				api.Success = false;
				api.StatusCode = 1;
				result = api;
			}
			return result;
		}
		public JToken Execute(string ApiUrl, JObject JsonReques)
		{
			JToken result;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ApiUrl);
				bool flag = httpWebRequest != null;
				if (flag)
				{
					httpWebRequest.Method = "POST";
					httpWebRequest.ContentType = "application/json";
					httpWebRequest.Headers.Add("APIKEY", this.ApiKey);
					httpWebRequest.Accept = "application/json";
					httpWebRequest.Proxy = new WebProxy("127.0.0.1", 8888);
					string s = JsonReques.ToString();
					ASCIIEncoding asciiencoding = new ASCIIEncoding();
					byte[] bytes = asciiencoding.GetBytes(s);
					Stream requestStream = httpWebRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
					WebResponse response = httpWebRequest.GetResponse();
					Stream responseStream = response.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream);
					result = JToken.Parse(streamReader.ReadToEnd());
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				result = null;
			}
			return result;
		}
		public JToken Execute(string ApiUrl, string JsonReques)
		{
			JToken result;
			try
			{
				ModuleInfo moduleByDefinition = ServiceLocator<IModuleController, ModuleController>.Instance.GetModuleByDefinition(0, "GDSModuleMVC");
				ModuleController moduleController = new ModuleController();
				Hashtable tabModuleSettings = moduleController.GetTabModuleSettings(moduleByDefinition.TabModuleID);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(tabModuleSettings["WCURL"].ToString() + ApiUrl);
				bool flag = httpWebRequest != null;
				if (flag)
				{
					httpWebRequest.Method = "POST";
					httpWebRequest.ContentType = "application/json";
					httpWebRequest.Headers.Add("APIKEY", this.ApiKey);
					httpWebRequest.Accept = "application/json";
					httpWebRequest.Proxy = new WebProxy("127.0.0.1", 8888);
					httpWebRequest.Timeout = 60000;
					string s = JsonReques.ToString();
					UTF8Encoding utf8Encoding = new UTF8Encoding();
					byte[] bytes = utf8Encoding.GetBytes(s);
					Stream requestStream = httpWebRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
					WebResponse response = httpWebRequest.GetResponse();
					Stream responseStream = response.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream);
					result = JToken.Parse(streamReader.ReadToEnd());
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				result = null;
			}
			return result;
		}
		public string GetValue(JToken token, string Property)
		{
			return token.Children<JProperty>().FirstOrDefault((JProperty x) => x.Name == Property).Value.ToString();
		}
		public Api IsValid()
		{
			JsonConvert.SerializeObject(new reqsessionIsValid
			{
				sessionId = this.SessionID
			});
			JToken jtoken = this.Execute("sessionIsValid", JsonConvert.SerializeObject(new reqsessionIsValid
			{
				sessionId = this.SessionID
			}));
			bool flag = jtoken != null;
			Api result;
			if (flag)
			{
				ressessionIsValid ressessionIsValid = jtoken.ToObject<ressessionIsValid>();
				result = new Api
				{
					Success = true,
					StatusCode = ressessionIsValid.errorCode
				};
			}
			else
			{
				result = new Api
				{
					Success = false,
					StatusCode = 2
				};
			}
			return result;
		}
		public ressearchHotel searchHotel(reqsearchHotel Object)
		{
			ressearchHotel result;
			try
			{
				//Api api = this.InitializeApi(PortalSettings.Current.PortalId);
				Object.sessionId = this.SessionID;
				result = JsonConvert.DeserializeObject<ressearchHotel>(this.Execute("searchHotel", JsonConvert.SerializeObject(Object)).ToString(), new JsonSerializerSettings
				{
					Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(this.HandleDeserializationError)
				});
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}
		public void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
		{
			string message = errorArgs.ErrorContext.Error.Message;
			errorArgs.ErrorContext.Handled = true;
		}
		public resgetHotel getHotel(reqgetHotel Object)
		{
			resgetHotel result;
			try
			{
				bool flag = DataCache.GetCache("CityHotels" + Object.cityId) != null && ((resgetHotel)DataCache.GetCache("CityHotels" + Object.cityId)).status;
				if (flag)
				{
					resgetHotel resgetHotel = (resgetHotel)DataCache.GetCache("CityHotels" + Object.cityId);
					result = resgetHotel;
				}
				else
				{
					resgetHotel resgetHotel2 =this.Execute("getHotel", JsonConvert.SerializeObject(Object)).ToObject<resgetHotel>();
					DataCache.SetCache("CityHotels" + Object.cityId, resgetHotel2);
					result = resgetHotel2;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}
		public resgetCity getCity(reqgetCity Object)
		{
			resgetCity result;
			try
			{
				bool flag = DataCache.GetCache("CityOfCountry" + Object.countryId) != null && ((resgetCity)DataCache.GetCache("CityOfCountry" + Object.countryId)).status;
				if (flag)
				{
					resgetCity resgetCity = (resgetCity)DataCache.GetCache("CityOfCountry" + Object.countryId);
					result = resgetCity;
				}
				else
				{
					//Api api = this.InitializeApi(PortalSettings.Current.PortalId);
					//Object.sessionId = this.SessionID;
					resgetCity resgetCity2 = this.Execute("getCity", JsonConvert.SerializeObject(Object)).ToObject<resgetCity>();
					DataCache.SetCache("CityOfCountry" + Object.countryId, resgetCity2);
					result = resgetCity2;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}
		public resreserve reserveandpay(reqreserve Object)
		{
			resreserve result;
			try
			{
				Object.sessionId = this.SessionID;
				resreserve resreserve = this.Execute("reserve", JsonConvert.SerializeObject(Object)).ToObject<resreserve>();
				bool flag = resreserve.response.reserve.info.thirdPartyCode == null;
				if (flag)
				{
					resreserve.response.reserve.info.thirdPartyCode = resreserve.response.reserve.info.id;
				}
				result = resreserve;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}
		public respgetReserveInfo GetReserveInfo(int ReserveID)
		{
			
			reqReserveInfo value = new reqReserveInfo
			{
				reserveId = ReserveID,
				sessionId = this.SessionID
			};
			JToken jtoken = this.Execute("getReserveInfo", JsonConvert.SerializeObject(value));
			bool flag = jtoken != null;
			if (flag)
			{
				return jtoken.ToObject<respgetReserveInfo>();
			}
			throw new Exception("امکان اتصال به API وجود ندارد");
		}
		public resprepareForBank prepareForBank(int ReserveID)
		{
			
			reqReserveInfo value = new reqReserveInfo
			{
				reserveId = ReserveID,
				sessionId = this.SessionID
			};
			JToken jtoken = this.Execute("prepareForBank", JsonConvert.SerializeObject(value));
			bool flag = jtoken != null;
			if (flag)
			{
				return jtoken.ToObject<resprepareForBank>();
			}
			throw new Exception("امکان اتصال به API وجود ندارد");
		}
		public resfinalize finalize(int ReserveID)
		{
			reqReserveInfo value = new reqReserveInfo
			{
				reserveId = ReserveID,
				sessionId = this.SessionID
			};
			JToken jtoken = this.Execute("finalize", JsonConvert.SerializeObject(value));
			bool flag = jtoken != null;
			resfinalize result;
			if (flag)
			{
				resfinalize resfinalize = new resfinalize();
				resfinalize = jtoken.ToObject<resfinalize>();
				bool flag2 = resfinalize.response == null;
				if (flag2)
				{
					resfinalize.response = new Responsefinalize
					{
						thirdPartyCode = null,
						paidByHotelCredit = "2"
					};
				}
				result = resfinalize;
			}
			else
			{
				jtoken = this.Execute("finalize", JsonConvert.SerializeObject(value));
				bool flag3 = jtoken != null;
				if (!flag3)
				{
					throw new Exception("امکان اتصال به API وجود ندارد. لطفا با پشتیبان تماس بگیرید");
				}
				resfinalize resfinalize2 = new resfinalize();
				resfinalize2 = jtoken.ToObject<resfinalize>();
				bool flag4 = resfinalize2.response == null;
				if (flag4)
				{
					resfinalize2.response = new Responsefinalize
					{
						thirdPartyCode = null,
						paidByHotelCredit = "2"
					};
				}
				result = resfinalize2;
			}
			return result;
		}
		public resfinalize lockReserve(int ReserveID)
		{
			reqReserveInfo value = new reqReserveInfo
			{
				reserveId = ReserveID,
				sessionId = this.SessionID
			};
			JToken jtoken = this.Execute("lockReserve", JsonConvert.SerializeObject(value));
			bool flag = jtoken != null;
			if (flag)
			{
				return jtoken.ToObject<resfinalize>();
			}
			throw new Exception("امکان اتصال به API وجود ندارد");
		}
		public Api.resisFinalize isFinalize(int ReserveID)
		{
			//Api api = this.InitializeApi(PortalSettings.Current.PortalId);
			reqReserveInfo value = new reqReserveInfo
			{
				reserveId = ReserveID,
				sessionId = this.SessionID
			};
			JToken jtoken = this.Execute("isFinalize", JsonConvert.SerializeObject(value));
			bool flag = jtoken != null;
			if (flag)
			{
				return jtoken.ToObject<Api.resisFinalize>();
			}
			throw new Exception("امکان اتصال به API وجود ندارد");
		}
		public class ResponseisFinalize
		{
			public int finalize { get; set; }
			public int paidByHotelCredit { get; set; }
		}
		public class resisFinalize
		{
			public bool status { get; set; }
			public int errorCode { get; set; }
			public string description { get; set; }
			public Api.ResponseisFinalize response { get; set; }
			public string datetime { get; set; }
		}
	}
}
