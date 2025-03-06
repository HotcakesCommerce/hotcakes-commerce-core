#region License

// Distributed under the MIT License
// ============================================================
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

namespace Hotcakes.Shipping.Ups.Models.Responses
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RatesResponse
    {
        [JsonProperty("RateResponse")]
        public RateResponse RateResponse { get; set; }
    }

    public partial class RateResponse
    {
        [JsonProperty("Response")]
        public Response Response { get; set; }

        [JsonProperty("RatedShipment")]
        public List<RatedShipment> RatedShipment { get; set; }
    }

    public partial class RatedShipment
    {
        [JsonProperty("Service")]
        public ResponseStatus Service { get; set; }

        [JsonProperty("RatedShipmentAlert")]
        public List<ResponseStatus> RatedShipmentAlert { get; set; }

        [JsonProperty("BillingWeight")]
        public BillingWeight BillingWeight { get; set; }

        [JsonProperty("TransportationCharges")]
        public BaseServiceCharge TransportationCharges { get; set; }

        [JsonProperty("BaseServiceCharge")]
        public BaseServiceCharge BaseServiceCharge { get; set; }

        [JsonProperty("ServiceOptionsCharges")]
        public BaseServiceCharge ServiceOptionsCharges { get; set; }

        [JsonProperty("TotalCharges")]
        public BaseServiceCharge TotalCharges { get; set; }

        [JsonProperty("GuaranteedDelivery", NullValueHandling = NullValueHandling.Ignore)]
        public GuaranteedDelivery GuaranteedDelivery { get; set; }

        [JsonProperty("RatedPackage")]
        public List<RatedPackage> RatedPackage { get; set; }
    }

    public partial class BaseServiceCharge
    {
        [JsonProperty("CurrencyCode")]
        public CurrencyCode CurrencyCode { get; set; }

        [JsonProperty("MonetaryValue")]
        public string MonetaryValue { get; set; }
    }

    public partial class BillingWeight
    {
        [JsonProperty("UnitOfMeasurement")]
        public ResponseStatus UnitOfMeasurement { get; set; }

        [JsonProperty("Weight")]
        public string Weight { get; set; }
    }

    public partial class ResponseStatus
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    public partial class GuaranteedDelivery
    {
        [JsonProperty("BusinessDaysInTransit")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BusinessDaysInTransit { get; set; }

        [JsonProperty("DeliveryByTime", NullValueHandling = NullValueHandling.Ignore)]
        public string DeliveryByTime { get; set; }
    }

    public partial class RatedPackage
    {
        [JsonProperty("TransportationCharges")]
        public BaseServiceCharge TransportationCharges { get; set; }

        [JsonProperty("BaseServiceCharge")]
        public BaseServiceCharge BaseServiceCharge { get; set; }

        [JsonProperty("ServiceOptionsCharges")]
        public BaseServiceCharge ServiceOptionsCharges { get; set; }

        [JsonProperty("ItemizedCharges", NullValueHandling = NullValueHandling.Ignore)]
        public List<ItemizedCharge> ItemizedCharges { get; set; }

        [JsonProperty("TotalCharges")]
        public BaseServiceCharge TotalCharges { get; set; }

        [JsonProperty("Weight")]
        public string Weight { get; set; }

        [JsonProperty("BillingWeight")]
        public BillingWeight BillingWeight { get; set; }
    }

    public partial class ItemizedCharge
    {
        [JsonProperty("Code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Code { get; set; }

        [JsonProperty("CurrencyCode")]
        public CurrencyCode CurrencyCode { get; set; }

        [JsonProperty("MonetaryValue")]
        public string MonetaryValue { get; set; }

        [JsonProperty("SubType", NullValueHandling = NullValueHandling.Ignore)]
        public string SubType { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("ResponseStatus")]
        public ResponseStatus ResponseStatus { get; set; }

        [JsonProperty("Alert")]
        public List<ResponseStatus> Alert { get; set; }

        [JsonProperty("TransactionReference")]
        public TransactionReference TransactionReference { get; set; }
    }

    public partial class TransactionReference
    {
        [JsonProperty("CustomerContext")]
        public string CustomerContext { get; set; }
    }

    public enum CurrencyCode { Usd };

    public enum CodeEnum { Lbs, The01, The02, The03 };

    public enum Description { Empty, Pounds, Success, YourInvoiceMayVaryFromTheDisplayedReferenceRates };

    public partial struct CodeUnion
    {
        public CodeEnum? Enum;
        public long? Integer;

        public static implicit operator CodeUnion(CodeEnum Enum) => new CodeUnion { Enum = Enum };
        public static implicit operator CodeUnion(long Integer) => new CodeUnion { Integer = Integer };
    }

    public partial class RatesResponse
    {
        public static RatesResponse FromJson(string json) => JsonConvert.DeserializeObject<RatesResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RatesResponse self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CurrencyCodeConverter.Singleton,
                CodeUnionConverter.Singleton,
                CodeEnumConverter.Singleton,
                DescriptionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CurrencyCodeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CurrencyCode) || t == typeof(CurrencyCode?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "USD")
            {
                return CurrencyCode.Usd;
            }
            throw new Exception("Cannot unmarshal type CurrencyCode");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CurrencyCode)untypedValue;
            if (value == CurrencyCode.Usd)
            {
                serializer.Serialize(writer, "USD");
                return;
            }
            throw new Exception("Cannot marshal type CurrencyCode");
        }

        public static readonly CurrencyCodeConverter Singleton = new CurrencyCodeConverter();
    }

    internal class CodeUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CodeUnion) || t == typeof(CodeUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "01":
                            return new CodeUnion { Enum = CodeEnum.The01 };
                        case "02":
                            return new CodeUnion { Enum = CodeEnum.The02 };
                        case "03":
                            return new CodeUnion { Enum = CodeEnum.The03 };
                        case "LBS":
                            return new CodeUnion { Enum = CodeEnum.Lbs };
                    }
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return new CodeUnion { Integer = l };
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type CodeUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (CodeUnion)untypedValue;
            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case CodeEnum.The01:
                        serializer.Serialize(writer, "01");
                        return;
                    case CodeEnum.The02:
                        serializer.Serialize(writer, "02");
                        return;
                    case CodeEnum.The03:
                        serializer.Serialize(writer, "03");
                        return;
                    case CodeEnum.Lbs:
                        serializer.Serialize(writer, "LBS");
                        return;
                }
            }
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value.ToString());
                return;
            }
            throw new Exception("Cannot marshal type CodeUnion");
        }

        public static readonly CodeUnionConverter Singleton = new CodeUnionConverter();
    }

    internal class CodeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CodeEnum) || t == typeof(CodeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "01":
                    return CodeEnum.The01;
                case "02":
                    return CodeEnum.The02;
                case "03":
                    return CodeEnum.The03;
                case "LBS":
                    return CodeEnum.Lbs;
            }
            throw new Exception("Cannot unmarshal type CodeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CodeEnum)untypedValue;
            switch (value)
            {
                case CodeEnum.The01:
                    serializer.Serialize(writer, "01");
                    return;
                case CodeEnum.The02:
                    serializer.Serialize(writer, "02");
                    return;
                case CodeEnum.The03:
                    serializer.Serialize(writer, "03");
                    return;
                case CodeEnum.Lbs:
                    serializer.Serialize(writer, "LBS");
                    return;
            }
            throw new Exception("Cannot marshal type CodeEnum");
        }

        public static readonly CodeEnumConverter Singleton = new CodeEnumConverter();
    }

    internal class DescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Description) || t == typeof(Description?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return Description.Empty;
                case "Pounds":
                    return Description.Pounds;
                case "Success":
                    return Description.Success;
                case "Your invoice may vary from the displayed reference rates":
                    return Description.YourInvoiceMayVaryFromTheDisplayedReferenceRates;
            }
            throw new Exception("Cannot unmarshal type Description");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Description)untypedValue;
            switch (value)
            {
                case Description.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case Description.Pounds:
                    serializer.Serialize(writer, "Pounds");
                    return;
                case Description.Success:
                    serializer.Serialize(writer, "Success");
                    return;
                case Description.YourInvoiceMayVaryFromTheDisplayedReferenceRates:
                    serializer.Serialize(writer, "Your invoice may vary from the displayed reference rates");
                    return;
            }
            throw new Exception("Cannot marshal type Description");
        }

        public static readonly DescriptionConverter Singleton = new DescriptionConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}