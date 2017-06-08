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
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for affiliate management
    /// </summary>
    [Serializable]
    public class AffiliatesHandler : BaseRestHandler
    {
        public AffiliatesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many affiliate to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected keys will be byproduct
        ///     and byslug.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     AffiliateDTO or list of AffiliateDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List all
                var response = new ApiResponse<List<AffiliateDTO>>();

                var results = HccApp.ContactServices.Affiliates.FindAllPaged(1, int.MaxValue);
                var dto = new List<AffiliateDTO>();

                foreach (var item in results)
                {
                    dto.Add(item.ToDto());
                }
                response.Content = dto;
                data = response.ObjectToJson();
            }
            else
            {
                // Find One Specific Category OR List of Referrals

                var ids = FirstParameter(parameters);
                long id = 0;
                long.TryParse(ids, out id);
                var isReferrals = GetParameterByIndex(1, parameters);

                if (isReferrals.Trim().ToLowerInvariant() == "referrals")
                {
                    // Referrals handler
                    var totalCount = 0;
                    var refs =
                        HccApp.ContactServices.AffiliateReferrals.FindByCriteria(
                            new AffiliateReferralSearchCriteria {AffiliateId = id}, 1, int.MaxValue, ref totalCount);
                    var resultRefs = new List<AffiliateReferralDTO>();
                    foreach (var r in refs)
                    {
                        resultRefs.Add(r.ToDto());
                    }
                    var responseA = new ApiResponse<List<AffiliateReferralDTO>> {Content = resultRefs};
                    data = responseA.ObjectToJson();
                }
                else
                {
                    // Affiliate Handler
                    var response = new ApiResponse<AffiliateDTO>();
                    var item = HccApp.ContactServices.Affiliates.Find(id);
                    if (item == null)
                    {
                        response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check id and try again."));
                    }
                    else
                    {
                        response.Content = item.ToDto();
                    }
                    data = response.ObjectToJson();
                }
            }

            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update an affiliate
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the affiliate ID and that this is an update, otherwise it assumes to create an
        ///     affiliate.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the AffiliateDTO object</param>
        /// <returns>AffiliateDTO - Serialized (JSON) version of the affiliate</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);
            var isReferrals = GetParameterByIndex(1, parameters);

            if (isReferrals.Trim().ToLowerInvariant() == "referrals")
            {
                // Create Referral
                var responseA = new ApiResponse<AffiliateReferralDTO>();
                AffiliateReferralDTO postedItemA = null;
                try
                {
                    postedItemA = Json.ObjectFromJson<AffiliateReferralDTO>(postdata);
                }
                catch (Exception ex)
                {
                    responseA.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                    return responseA.ObjectToJson();
                }
                var itemA = new AffiliateReferral();
                itemA.FromDto(postedItemA);
                HccApp.ContactServices.AffiliateReferrals.Create2(itemA, true);
                responseA.Content = itemA.ToDto();
                data = responseA.ObjectToJson();
            }
            else
            {
                // Create/Update Affiliate
                var responseB = new ApiResponse<AffiliateDTO>();
                AffiliateDTO postedItem = null;
                try
                {
                    postedItem = Json.ObjectFromJson<AffiliateDTO>(postdata);
                }
                catch (Exception ex)
                {
                    responseB.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                    return responseB.ObjectToJson();
                }

                var item = new Affiliate();
                item.FromDto(postedItem);

                if (id < 1)
                {
                    var existing = HccApp.ContactServices.Affiliates.FindByAffiliateId(item.AffiliateId);
                    if (existing == null || existing.Id < 1)
                    {
                        // 20140408 - Updated to call the current repository method, instead of allowing the error to be thrown
                        var result = CreateUserStatus.None;
                        try
                        {
                            // set a temporary password
                            item.Password = PasswordGenerator.GeneratePassword(8);

                            // create the affiliate
                            HccApp.ContactServices.Affiliates.Create(item, ref result);
                        }
                        catch (Exception createException)
                        {
                            // return the exception to the calling method
                            responseB.Errors.Add(new ApiError
                            {
                                Code = "EXCEPTION",
                                Description = createException.Message
                            });
                        }

                        if (result == CreateUserStatus.Success)
                        {
                            // send the affiliate
                            responseB.Content = item.ToDto();
                        }
                        else
                        {
                            // send a response why the affiliate wasn't created
                            responseB.Errors.Add(new ApiError
                            {
                                Code = "USERNOTCREATED",
                                Description = result.ToString()
                            });
                        }
                        id = item.Id;
                    }
                    else
                    {
                        try
                        {
                            responseB.Content = existing.ToDto();
                            id = existing.Id;
                        }
                        catch (Exception exUpdate)
                        {
                            responseB.Errors.Add(new ApiError {Code = "EXCEPTION", Description = exUpdate.Message});
                        }
                    }
                }
                else
                {
                    try
                    {
                        HccApp.ContactServices.Affiliates.Update(item);
                    }
                    catch (Exception ex)
                    {
                        responseB.Errors.Add(new ApiError {Code = "EXCEPTION", Description = ex.Message});
                    }

                    id = item.Id;
                    responseB.Content = item.ToDto();
                }
                data = responseB.ObjectToJson();
            }

            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete an affiliate from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (affiliate ID) is
        ///     expected.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. Not used in this method.</param>
        /// <param name="postdata">This parameter is not used in this method</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain either True or False,
        ///     depending on success of the deletion
        /// </returns>
        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);
            var response = new ApiResponse<bool>();

            try
            {
                response.Content = HccApp.ContactServices.Affiliates.Delete(id);
            }
            catch (Exception ex)
            {
                response.Content = false;
                response.Errors.Add(new ApiError {Code = "EXCEPTION", Description = ex.Message});
            }

            data = response.ObjectToJson();
            return data;
        }
    }
}