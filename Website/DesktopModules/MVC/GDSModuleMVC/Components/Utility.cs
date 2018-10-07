using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using Extensions;
using Moslem.Modules.GDS.GDSModuleMVC.Controllers;
using Moslem.Modules.GDSModuleMVC.com.payamsms.beta;
using Moslem.Modules.GDSModuleMVC.ir.retirement;
using Moslem.Modules.GDSModuleMVC.Models;
using Newtonsoft.Json;
using static Moslem.Modules.GDSModuleMVC.Components.Cards;

namespace Moslem.Modules.GDSApi.GDSModule
{
    public static class NewtonsoftJsonExtensions
    {
        public static ActionResult ToJsonResult(this object obj)
        {
            var content = new ContentResult();
            content.Content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            content.ContentType = "application/json";
            return content;
        }
    }
    public static class Utility
    {
        public static decimal GetRackAmount(respgetReserveInfo objReserveInfo)
        {
            int PriceFromReserve = 0;
            foreach (PassengerReserveInfo passengerReserveInfo2 in objReserveInfo.response.passenger)
            {
                foreach (DayPriceReserveInfo dayPriceReserveInfo5 in passengerReserveInfo2.dayPrice)
                {
                    PriceFromReserve += dayPriceReserveInfo5.rackRate;
                }
            }
            return PriceFromReserve;
        }
        public static decimal GetDiscountAmountGiftCard(respgetReserveInfo objReserveInfo, DiscountModelForCard discount)
        {
            decimal DiscountAmount = 0;
            for (int i = 0; i < discount.RoomID.Count(); i++)
            {
                if (discount.night[i] > 0)
                {
                    DiscountAmount = objReserveInfo.response.passenger.Where(p => p.roomId == discount.RoomID[i]).Select(p => p).FirstOrDefault().dayPrice[0].price * discount.night[i];
                }
            }
            return DiscountAmount;
        }
        public static int getAmountFromReserveInfo(respgetReserveInfo objReserveInfo)
        {
            int PriceFromReserve = 0;
            foreach (PassengerReserveInfo passengerReserveInfo2 in objReserveInfo.response.passenger)
            {
                foreach (DayPriceReserveInfo dayPriceReserveInfo5 in passengerReserveInfo2.dayPrice)
                {
                    PriceFromReserve += dayPriceReserveInfo5.price;
                }
            }
            return PriceFromReserve;
        }
        public static Decimal GetRetiredDiscount(decimal Amount, bool HasPartner)
        {
            decimal AmountAfterDiscount = Amount;
            if (Amount > 3300000)
            {
                AmountAfterDiscount = Amount - 3300000;
                if (HasPartner)
                {
                    if (AmountAfterDiscount > 3300000)
                        AmountAfterDiscount = AmountAfterDiscount - 3300000;
                    else
                        AmountAfterDiscount = 0;
                }
            }
            else
                AmountAfterDiscount = 0;
            return AmountAfterDiscount;
        }
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] array = new byte[4096];
            int count;
            while ((count = src.Read(array, 0, array.Length)) != 0)
            {
                dest.Write(array, 0, count);
            }
        }
        public static byte[] Zip(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (MemoryStream memoryStream2 = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(memoryStream2, CompressionMode.Compress))
                    {
                        Utility.CopyTo(memoryStream, gzipStream);
                    }
                    result = memoryStream2.ToArray();
                }
            }
            return result;
        }
        public static string Unzip(byte[] bytes)
        {
            string @string;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (MemoryStream memoryStream2 = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        Utility.CopyTo(gzipStream, memoryStream2);
                    }
                    @string = Encoding.UTF8.GetString(memoryStream2.ToArray());
                }
            }
            return @string;
        }
        private static void Main(string[] args)
        {
            byte[] bytes = Utility.Zip("StringStringStringStringStringStringStringStringStringStringStringStringStringString");
            string text = Utility.Unzip(bytes);
        }
        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
        public static string Base64Decode(string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }
        public static double GetNightShamsi(string sdate, string ndate)
        {
            CultureInfo provider = Thread.CurrentThread.CurrentCulture;
            return (DateTime.Parse(ndate, provider) - DateTime.Parse(sdate, provider)).TotalDays;
        }
        public static string ShamsiToMiladi(string Date)
        {
            CultureInfo provider = new CultureInfo("en-US");
            CultureInfo provider2 = new CultureInfo("fa-IR");
            return DateTime.Parse(Date, provider2).ToString("yyyy-MM-dd", provider);
        }
        public static string MiladiToShamsi(string Date)
        {
            DateTime georgianDate = DateTime.Parse(Date, new CultureInfo("en-US"));
            return georgianDate.ToPersianDateString();
        }
        public static string MiladiToShamsiAddDay(string Date, int Days)
        {
            DateTime georgianDate = DateTime.Parse(Date, new CultureInfo("en-US")).AddDays((double)Days);
            return georgianDate.ToPersianDateString();
        }
        public static bool runQuery(string Query)
        {
            bool result;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = connectionString;
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = Query;
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static bool SendSMS(string[] Destenition, string text)
        {
            bool result;
            try
            {
                JaxRpcMessagingServiceService jaxRpcMessagingServiceService = new JaxRpcMessagingServiceService();
                BalanceResult balance = jaxRpcMessagingServiceService.getBalance("DbL1mw0xTOPfT0nH2JFH5Q", "1357986420", 2);
                SendResult sendResult = jaxRpcMessagingServiceService.send("DbL1mw0xTOPfT0nH2JFH5Q", "1357986420", "200020060", Destenition, "", "", new string[0], 1, 2, true, DateTime.Now, text);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public static string isValidRetired(long NationalCode, long DaftarNumber)
        {
           
            string returnValue = (string)DataCache.GetCache(NationalCode.ToString() + DaftarNumber.ToString());
            if (string.IsNullOrEmpty(returnValue))
            {
                CspfService objWC = new CspfService();
                returnValue = objWC.CspfServiceMoshtarak(DaftarNumber, NationalCode, "wsiguser", "yK&49$sM28", "37.156.145.60");
                DataCache.SetCache(NationalCode.ToString() + DaftarNumber.ToString(), returnValue);
            }
            //returnValue = "1";
            return returnValue;
        }
        public static bool RetiredHasCredit(long NationalCode, long DaftarNumber)
        {
            if (isValidRetired(NationalCode, DaftarNumber) != "1")
            {
                return false;
            }
            using (IDataContext dataContext = DataContext.Instance())
            {
                if (dataContext.ExecuteQuery<RetiredCreditUsed>(CommandType.Text, "SELECT * FROM [GDS_RetiredCreditUsed] where NationalCode ='" + NationalCode + "' and DaftarNumber ='" + DaftarNumber + "' and [Finalized] = '1'").ToList<RetiredCreditUsed>().Count() < 3)
                    return true;
                else
                    return false;
            }
        }
        public static RetiredCreditUsed GetRetiredDiscountofReserve(string ReserveID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                return dataContext.ExecuteQuery<RetiredCreditUsed>(CommandType.Text, "SELECT * FROM [GDS_RetiredCreditUsed] where [ReserveID] ='" + ReserveID + "'").ToList<RetiredCreditUsed>().FirstOrDefault();
            }
        }
        public static void FinalizeRetiredCredit(string ReserveID)
        {
            using (IDataContext dataContext = DataContext.Instance())
            {
                dataContext.ExecuteScalar<RetiredCreditUsed>(CommandType.Text, "update [GDS_RetiredCreditUsed] set [Finalized] ='1' where [ReserveID] ='" + ReserveID + "'");
            }
        }

    }
}
