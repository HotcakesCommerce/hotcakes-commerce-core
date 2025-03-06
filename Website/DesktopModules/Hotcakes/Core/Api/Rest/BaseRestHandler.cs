#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class BaseRestHandler : IRestHandler
    {
        public BaseRestHandler(HotcakesApplication app)
        {
            HccApp = app;
        }

        public HotcakesApplication HccApp { get; set; }

        public virtual string GetAction(string parameters, NameValueCollection querystring)
        {
            throw new NotImplementedException();
        }

        public virtual string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            throw new NotImplementedException();
        }

        public virtual string PutAction(string parameters, NameValueCollection querystring, string postdata)
        {
            throw new NotImplementedException();
        }

        public virtual string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            throw new NotImplementedException();
        }

        public Guid FirstGuidParameter(string allParams)
        {
            Guid p;
            var str = FirstParameter(allParams);
            return Guid.TryParse(str, out p) ? p : Guid.Empty;
        }

        public long? FirstLongParameter(string allParams)
        {
            long p;
            var str = FirstParameter(allParams);
            return long.TryParse(str, out p) ? (long?) p : null;
        }

        public string FirstParameter(string allParams)
        {
            return GetParameterByIndex(0, allParams);
        }

        public string GetParameterByIndex(int index, string allParams)
        {
            var result = string.Empty;
            if (allParams == null) return result;

            if (allParams.Trim().Length > 0)
            {
                if (index < 0) index = 0;

                var parts = allParams.Split('/');
                if (parts.Length - 1 >= index)
                {
                    result = parts[index];
                }
            }
            return result;
        }

        protected string JsonApiResponse<T>(T obj, List<ApiError> errors = null)
        {
            var response = new ApiResponse<T>();
            response.Content = obj;
            return Json.ObjectToJson(response);
        }

        protected string JsonApiResponseException(Exception ex)
        {
            return JsonApiResponseError("EXCEPTION", ex.Message);
        }

        protected string JsonApiResponseError(string code, string error)
        {
            var response = new ApiResponse<object>();
            response.Errors = new List<ApiError> {new ApiError {Code = code, Description = error}};
            return Json.ObjectToJson(response);
        }
    }
}