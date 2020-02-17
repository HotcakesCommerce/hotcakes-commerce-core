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
using System.IO;
using System.Text;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Membership;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.CommerceDTO.v1.Taxes;
using Hotcakes.Web;

namespace Hotcakes.CommerceDTO.v1.Client
{
    /// <summary>
    ///     This is the primary class through which all REST API communications are managed.
    /// </summary>
    /// <remarks>Calls to this class will be routed and processed by the appropriate REST API handler</remarks>
    [Serializable]
    public class Api
    {
        #region Search Manager

        /// <summary>
        ///     Allows you to manually execute the indexing of a product. This is useful after updating a product.
        /// </summary>
        /// <param name="productBvin">String - the unique ID or Bvin of the product to index</param>
        /// <returns>Boolean - if true, the product was found and the product was indexed</returns>
        public ApiResponse<bool> SearchManagerIndexProduct(string productBvin)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "searchmanager/products/" + Enc(productBvin) + "?key=" + Enc(key), string.Empty);
            return response;
        }

        #endregion

        #region Utilities

        /// <summary>
        ///     Accepts a value to parse and return as a URL-safe slug to use for URLs
        /// </summary>
        /// <param name="input">String - the value to parse and return as a URL-safe slug</param>
        /// <returns>String - a URL-safe slug, but no verification has been done to ensure that the slug is unique</returns>
        public ApiResponse<string> UtilitiesSlugify(string input)
        {
            var result = new ApiResponse<string>();
            result = RestHelper.PostRequest<ApiResponse<string>>(fullApiUri + "utilities/slugify?key=" + Enc(key), input);
            return result;
        }

        #endregion

        #region Constructor

        private string Enc(string input)
        {
            // The web request class actuall does encoding for us
            // so we only need to encode the "/" character
            return input.Replace("/", "%2F");
            //return System.Web.HttpUtility.UrlEncode(input);
        }

        private readonly string baseUri = string.Empty;
        private readonly string key = string.Empty;
        private readonly string fullApiUri = string.Empty;

        /// <summary>
        ///     Creates an instance of this class with local properties set-up for you
        /// </summary>
        /// <param name="baseWebSiteUri">String - this should be the domain for your site, such as http://mysite.com</param>
        /// <param name="apiKey">String - this is the API key you created in your store administration area</param>
        /// <remarks>Instantiating this class will determine the API end point URLs for you.</remarks>
        public Api(string baseWebSiteUri, string apiKey)
        {
            baseUri = baseWebSiteUri;
            if (!baseUri.EndsWith("/"))
            {
                baseUri += "/";
            }
            fullApiUri = Path.Combine(baseUri, "DesktopModules/Hotcakes/API/rest/v1/");
            key = apiKey;
        }

        #endregion

        #region Categories

        /// <summary>
        ///     Returns a list of categories for the store sorted the same as you have sorted in the store admin area.
        /// </summary>
        /// <returns>List of CategorySnapshotDTO - a list of the populated CategorySnapshotDTO objects for all store categories</returns>
        public ApiResponse<List<CategorySnapshotDTO>> CategoriesFindAll()
        {
            var result = new ApiResponse<List<CategorySnapshotDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<CategorySnapshotDTO>>>(fullApiUri + "categories/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns all of the categories that the specified product belongs to in the store.
        /// </summary>
        /// <param name="productBvin">String - the unique ID of the product you wish to search on</param>
        /// <returns>List of CategorySnapshotDTO - a list of the populated category objects that match the product</returns>
        public ApiResponse<List<CategorySnapshotDTO>> CategoriesFindForProduct(string productBvin)
        {
            var result = new ApiResponse<List<CategorySnapshotDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<CategorySnapshotDTO>>>(fullApiUri + "categories/?key=" + Enc(key) +
                                                                              "&byproduct=" + Enc(productBvin));
            return result;
        }

        /// <summary>
        ///     Returns any category matching the category ID provided.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the category</param>
        /// <returns>CategoryDTO - a populated instance of the category you were looking for</returns>
        public ApiResponse<CategoryDTO> CategoriesFind(string bvin)
        {
            var result = new ApiResponse<CategoryDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<CategoryDTO>>(fullApiUri + "categories/" + Enc(bvin) + "?key=" +
                                                                Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns the category that matches the URL slug provided.
        /// </summary>
        /// <param name="slug">String - the last portion of the URL after the final slash</param>
        /// <returns>CategoryDTO - a populated instance of the category that matches the slug</returns>
        public ApiResponse<CategoryDTO> CategoriesFindBySlug(string slug)
        {
            var result = new ApiResponse<CategoryDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<CategoryDTO>>(fullApiUri + "categories/ANY" + "?key=" + Enc(key) +
                                                                "&byslug=" + Enc(slug));
            return result;
        }

        /// <summary>
        ///     Creates the category with the provided attributes and returns the object back to you.
        /// </summary>
        /// <param name="item">CategoryDTO - an instance of the category</param>
        /// <returns>CategoryDTO - the saved instance of the category with updated properties</returns>
        public ApiResponse<CategoryDTO> CategoriesCreate(CategoryDTO item)
        {
            var result = new ApiResponse<CategoryDTO>();
            result = RestHelper.PostRequest<ApiResponse<CategoryDTO>>(fullApiUri + "categories/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the category matching the given ID with the provided attributes and returns the object back to you.
        /// </summary>
        /// <param name="item">CategoryDTO - a pre-populated instance of category with your changes applied</param>
        /// <returns>CategoryDTO - an updated instance of the category from the stie with your saved changes</returns>
        public ApiResponse<CategoryDTO> CategoriesUpdate(CategoryDTO item)
        {
            var result = new ApiResponse<CategoryDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CategoryDTO>>(
                    fullApiUri + "categories/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Deletes the category matching the given category ID and returns true if successful.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the category</param>
        /// <returns>Boolean - if true, the category was successfully deleted</returns>
        public ApiResponse<bool> CategoriesDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "categories/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     Deletes all of the categories in the store, provided that this has been allowed in the store admin API page.
        /// </summary>
        /// <returns>Boolean - if true, the action happened as intended</returns>
        /// <remarks>Requires additional permissions in the store API administration</remarks>
        public ApiResponse<bool> CategoriesClearAll()
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "categories/?key=" + Enc(key),
                string.Empty);
            return result;
        }

        #endregion

        #region Price Groups

        /// <summary>
        ///     This end point allows you to return all of the price groups for the store.
        /// </summary>
        /// <returns>List of PriceGroupDTO - a listing of all of the price groups in the store</returns>
        public ApiResponse<List<PriceGroupDTO>> PriceGroupsFindAll()
        {
            var result = new ApiResponse<List<PriceGroupDTO>>();
            result = RestHelper.GetRequest<ApiResponse<List<PriceGroupDTO>>>(fullApiUri + "pricegroups/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Use this end point to find a specific price group.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the price group you want to find</param>
        /// <returns>PriceGroupDTO - a populated instance of the price group from the site</returns>
        public ApiResponse<PriceGroupDTO> PriceGroupsFind(string bvin)
        {
            var result = new ApiResponse<PriceGroupDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<PriceGroupDTO>>(fullApiUri + "pricegroups/" + Enc(bvin) + "?key=" +
                                                                  Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to create a new price group and returns a copy of the price group to you.
        /// </summary>
        /// <param name="item">PriceGroupDTO - a new instance of the price group with your initial updates applied</param>
        /// <returns>PriceGroupDTO - the saved price group from the site</returns>
        public ApiResponse<PriceGroupDTO> PriceGroupsCreate(PriceGroupDTO item)
        {
            var result = new ApiResponse<PriceGroupDTO>();
            result = RestHelper.PostRequest<ApiResponse<PriceGroupDTO>>(fullApiUri + "pricegroups/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Returns a list of categories for the store sorted the same as you have sorted in the store admin area.
        /// </summary>
        /// <param name="item">PriceGroupDTO - a populated instance of the price group with your updates</param>
        /// <returns>PriceGroupDTO - a new instance of the price group from the site with your changes applied</returns>
        public ApiResponse<PriceGroupDTO> PriceGroupsUpdate(PriceGroupDTO item)
        {
            var result = new ApiResponse<PriceGroupDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<PriceGroupDTO>>(
                    fullApiUri + "pricegroups/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Deletes the specified price group and returns the status of the deletion to you.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the price group</param>
        /// <returns>Boolean - if successful, true will be returned</returns>
        public ApiResponse<bool> PriceGroupsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "pricegroups/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Customer Accounts

        /// <summary>
        ///     Find all customer accounts in the store
        /// </summary>
        /// <returns>List of CustomerAccountDTO</returns>
        public ApiResponse<List<CustomerAccountDTO>> CustomerAccountsFindAll()
        {
            var result = new ApiResponse<List<CustomerAccountDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<CustomerAccountDTO>>>(fullApiUri + "customeraccounts/?key=" +
                                                                             Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to get a count of all of the customer accounts in the store
        /// </summary>
        /// <returns>Long - the total number of customer accounts in the store</returns>
        /// <remarks>This is simply a count of all users on the website</remarks>
        public ApiResponse<long> CustomerAccountsCountOfAll()
        {
            var result = new ApiResponse<long>();
            result =
                RestHelper.GetRequest<ApiResponse<long>>(fullApiUri + "customeraccounts/?key=" + Enc(key) +
                                                         "&countonly=1");
            return result;
        }

        /// <summary>
        ///     Allows you to request a listing of customer accounts for use in a paged display
        /// </summary>
        /// <param name="pageNumber">Integer - the page number you are requesting</param>
        /// <param name="pageSize">Integer - the number of records per page</param>
        /// <returns>List of CustomerAccountDTO</returns>
        public ApiResponse<List<CustomerAccountDTO>> CustomerAccountsFindAllByPage(int pageNumber, int pageSize)
        {
            var result = new ApiResponse<List<CustomerAccountDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<CustomerAccountDTO>>>(fullApiUri + "customeraccounts/?key=" +
                                                                             Enc(key) + "&page=" + pageNumber +
                                                                             "&pagesize=" + pageSize);
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific customer account using the unique ID (bvin)
        /// </summary>
        /// <param name="bvin">String - the unique ID of the customer account</param>
        /// <returns>CustomerAccountDTO - an instance of the customer account</returns>
        public ApiResponse<CustomerAccountDTO> CustomerAccountsFind(string bvin)
        {
            var result = new ApiResponse<CustomerAccountDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<CustomerAccountDTO>>(fullApiUri + "customeraccounts/" + Enc(bvin) +
                                                                       "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific customer account using their email address
        /// </summary>
        /// <param name="email">String - the email address of the customer account</param>
        /// <returns>CustomerAccountDTO - an instance of the customer account</returns>
        /// <remarks>
        ///     If there are multiple customers with the same email address, the first one found will be returned. Only
        ///     reliable when the CMS requires a unique email address.
        /// </remarks>
        public ApiResponse<CustomerAccountDTO> CustomerAccountsFindByEmail(string email)
        {
            var result = new ApiResponse<CustomerAccountDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<CustomerAccountDTO>>(fullApiUri + "customeraccounts/?key=" + Enc(key) +
                                                                       "&email=" + Enc(email));
            return result;
        }

        /// <summary>
        ///     Creates a customer account using the information provided
        /// </summary>
        /// <param name="item">CustomerAccountDTO - an instance of the (to be saved) customer account</param>
        /// <returns>CustomerAccountDTO - an instance of the customer account loaded from the site</returns>
        public ApiResponse<CustomerAccountDTO> CustomerAccountsCreate(CustomerAccountDTO item)
        {
            var result = new ApiResponse<CustomerAccountDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CustomerAccountDTO>>(
                    fullApiUri + "customeraccounts/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates a customer account using the information provided by matching the unique customer ID (bvin) to an existing
        ///     customer.
        /// </summary>
        /// <param name="item">CustomerAccountDTO - an updated instance of the customer account</param>
        /// <returns>CustomerAccountDTO - an instance of the customer account loaded from the site</returns>
        public ApiResponse<CustomerAccountDTO> CustomerAccountsUpdate(CustomerAccountDTO item)
        {
            var result = new ApiResponse<CustomerAccountDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CustomerAccountDTO>>(
                    fullApiUri + "customeraccounts/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Allows you to create a customer account while also specifying the user's password.
        /// </summary>
        /// <param name="item">CustomerAccountDTO - an updated instance of the customer account</param>
        /// <param name="clearpassword">String - the clear text version of the password you want the customer to have</param>
        /// <returns>CustomerAccountDTO - an instance of the customer account loaded from the site</returns>
        /// <remarks>
        ///     This should generally not be used, as the password will be part of the URL of the end point. However, it will
        ///     never be transmitted via clear text afterward.
        /// </remarks>
        public ApiResponse<CustomerAccountDTO> CustomerAccountsCreateWithPassword(CustomerAccountDTO item,
            string clearpassword)
        {
            var result = new ApiResponse<CustomerAccountDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CustomerAccountDTO>>(
                    fullApiUri + "customeraccounts/?key=" + Enc(key) + "&pwd=" + Enc(clearpassword),
                    Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Allows you to delete a customer account from your store.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the customer account</param>
        /// <returns>Boolean - if true, the deletion of the user account was successful</returns>
        /// <remarks>
        ///     This is a permanent deletion in the store, including related items, such as wish lists. This would be a soft
        ///     delete for the CMS.
        /// </remarks>
        public ApiResponse<bool> CustomerAccountsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "customeraccounts/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     Deletes all of the customer accounts in the store, provided that this has been allowed in the store admin API page.
        /// </summary>
        /// <returns>Boolean - if true, the action happened as intended</returns>
        /// <remarks>Requires additional permissions in the store API administration</remarks>
        public ApiResponse<bool> CustomerAccountsClearAll()
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "customeraccounts/?key=" + Enc(key),
                string.Empty);
            return result;
        }

        #endregion

        #region Affiliates

        /// <summary>
        ///     Allows you to find all affiliates in the store
        /// </summary>
        /// <returns>List of AffiliateDTO</returns>
        public ApiResponse<List<AffiliateDTO>> AffiliatesFindAll()
        {
            var result = new ApiResponse<List<AffiliateDTO>>();
            result = RestHelper.GetRequest<ApiResponse<List<AffiliateDTO>>>(fullApiUri + "affiliates/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns a specific affiliate in the store matching the given ID
        /// </summary>
        /// <param name="id">Long - the unique ID of the affiliate</param>
        /// <returns>AffiliateDTO</returns>
        public ApiResponse<AffiliateDTO> AffiliatesFind(long id)
        {
            var result = new ApiResponse<AffiliateDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<AffiliateDTO>>(fullApiUri + "affiliates/" + id + "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Creates a new affiliate in your store and returns the object back to you
        /// </summary>
        /// <param name="item">AffiliateDTO - a populated instance of the affiliate to add</param>
        /// <returns>AffiliateDTO - a hydrated instance of the saved affiliate loaded from the data source</returns>
        public ApiResponse<AffiliateDTO> AffiliatesCreate(AffiliateDTO item)
        {
            var result = new ApiResponse<AffiliateDTO>();
            result = RestHelper.PostRequest<ApiResponse<AffiliateDTO>>(fullApiUri + "affiliates/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the affiliate record with the information provided
        /// </summary>
        /// <param name="item">AffiliateDTO - a populated instance of the affiliate to update</param>
        /// <returns>AffiliateDTO - a hydrated instance of the saved affiliate loaded from the data source</returns>
        public ApiResponse<AffiliateDTO> AffiliatesUpdate(AffiliateDTO item)
        {
            var result = new ApiResponse<AffiliateDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<AffiliateDTO>>(
                    fullApiUri + "affiliates/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Deletes the specified affiliate from the store that matches the given ID
        /// </summary>
        /// <param name="id">Long - the unique ID of the affiliate</param>
        /// <returns>Boolean - if true, the affiliate was successfully deleted</returns>
        public ApiResponse<bool> AffiliatesDelete(long id)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "affiliates/" + id + "?key=" + Enc(key),
                string.Empty);
            return result;
        }

        #endregion

        #region AffiliateReferrals

        /// <summary>
        ///     Use this end point to find all referrals for a specific affiliate in the store.
        /// </summary>
        /// <param name="id">Long - the unique ID of the affiliate you wish to query against</param>
        /// <returns>List of AffiliateReferralDTO</returns>
        public ApiResponse<List<AffiliateReferralDTO>> AffiliateReferralsFindForAffiliate(long id)
        {
            var result = new ApiResponse<List<AffiliateReferralDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<AffiliateReferralDTO>>>(fullApiUri + "affiliates/" + id +
                                                                               "/referrals?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Creates a new referral for the specified affiliate.
        /// </summary>
        /// <param name="item">AffiliateReferralDTO - a populated instance of the affiliate referral</param>
        /// <returns>AffiliateReferralDTO</returns>
        public ApiResponse<AffiliateReferralDTO> AffiliateReferralsCreate(AffiliateReferralDTO item)
        {
            var result = new ApiResponse<AffiliateReferralDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<AffiliateReferralDTO>>(
                    fullApiUri + "affiliates/" + item.AffiliateId + "/referrals?key=" + Enc(key),
                    Json.ObjectToJson(item));
            return result;
        }

        #endregion

        #region Tax Schedules

        /// <summary>
        ///     Allows you to ask for all tax schedules that might be in the store.
        /// </summary>
        /// <returns>List of TaxScheduleDTO</returns>
        public ApiResponse<List<TaxScheduleDTO>> TaxSchedulesFindAll()
        {
            var result = new ApiResponse<List<TaxScheduleDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<TaxScheduleDTO>>>(fullApiUri + "taxschedules/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns a specific tax schedule from the store.
        /// </summary>
        /// <param name="id">Long - the unique ID of the tax schedule</param>
        /// <returns>TaxScheduleDTO</returns>
        public ApiResponse<TaxScheduleDTO> TaxSchedulesFind(long id)
        {
            var result = new ApiResponse<TaxScheduleDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<TaxScheduleDTO>>(fullApiUri + "taxschedules/" + id + "?key=" +
                                                                   Enc(key));
            return result;
        }

        /// <summary>
        ///     You can create a new tax schedule using this end point.
        /// </summary>
        /// <param name="item">TaxScheduleDTO - a opulated instand of the tax schedule to add</param>
        /// <returns>TaxScheduleDTO - A hydrated instance of the tax schedule loaded from the data source</returns>
        public ApiResponse<TaxScheduleDTO> TaxSchedulesCreate(TaxScheduleDTO item)
        {
            var result = new ApiResponse<TaxScheduleDTO>();
            result = RestHelper.PostRequest<ApiResponse<TaxScheduleDTO>>(fullApiUri + "taxschedules/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates an existing tax schedule with the information provided
        /// </summary>
        /// <param name="item">TaxScheduleDTO - a opulated instand of the tax schedule to update</param>
        /// <returns>TaxScheduleDTO - A hydrated instance of the tax schedule loaded from the data source</returns>
        public ApiResponse<TaxScheduleDTO> TaxSchedulesUpdate(TaxScheduleDTO item)
        {
            var result = new ApiResponse<TaxScheduleDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<TaxScheduleDTO>>(
                    fullApiUri + "taxschedules/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Deletes the specified tax schedule from the store that matches the given ID
        /// </summary>
        /// <param name="id">Long - the unique ID of the tax schedule</param>
        /// <returns>Boolean - if true, the tax schedule was successfully deleted</returns>
        public ApiResponse<bool> TaxSchedulesDelete(long id)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(
                fullApiUri + "taxschedules/" + id + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Taxes

        /// <summary>
        ///     Allows you to find all taxes in the store for all tax zones.
        /// </summary>
        /// <returns>TaxDTO</returns>
        public ApiResponse<List<TaxDTO>> TaxesFindAll()
        {
            var result = new ApiResponse<List<TaxDTO>>();
            result = RestHelper.GetRequest<ApiResponse<List<TaxDTO>>>(fullApiUri + "taxes/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Find a specific tax zone using the ID of the tax zone.
        /// </summary>
        /// <param name="id">The unique ID of the tax zone you want to find.</param>
        /// <returns>TaxDTO</returns>
        public ApiResponse<TaxDTO> TaxesFind(long id)
        {
            var result = new ApiResponse<TaxDTO>();
            result = RestHelper.GetRequest<ApiResponse<TaxDTO>>(fullApiUri + "taxes/" + id + "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Create a tax zone for a tax schedule to calculate taxes for a matching order.
        /// </summary>
        /// <param name="item">A populated instance of TaxDTO used to create a new tax record.</param>
        /// <returns>TaxDTO</returns>
        public ApiResponse<TaxDTO> TaxesCreate(TaxDTO item)
        {
            var result = new ApiResponse<TaxDTO>();
            result = RestHelper.PostRequest<ApiResponse<TaxDTO>>(fullApiUri + "taxes/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the tax record wtih the information specified.
        /// </summary>
        /// <param name="item">A populated instance of TaxDTO used to update the existing tax record.</param>
        /// <returns>TaxDTO</returns>
        public ApiResponse<TaxDTO> TaxesUpdate(TaxDTO item)
        {
            var result = new ApiResponse<TaxDTO>();
            result = RestHelper.PostRequest<ApiResponse<TaxDTO>>(fullApiUri + "taxes/" + item.Id + "?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Deletes the tax zone from the store permanently.
        /// </summary>
        /// <param name="id">The unique ID of the tax zone you want to delete.</param>
        /// <returns>Boolean</returns>
        public ApiResponse<bool> TaxesDelete(long id)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "taxes/" + id + "?key=" + Enc(key),
                string.Empty);
            return result;
        }

        #endregion

        #region Vendors

        /// <summary>
        ///     Allows you to find all vendors that are in your store.
        /// </summary>
        /// <returns>List of VendorManufacturerDTO</returns>
        public ApiResponse<List<VendorManufacturerDTO>> VendorFindAll()
        {
            var result = new ApiResponse<List<VendorManufacturerDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<VendorManufacturerDTO>>>(fullApiUri + "vendors/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Find a specific vendor using the Bvin of the the respective vendor.
        /// </summary>
        /// <param name="bvin">The unique ID of the vendor you are looking for.</param>
        /// <returns>VendorManufactureDTO</returns>
        public ApiResponse<VendorManufacturerDTO> VendorFind(string bvin)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<VendorManufacturerDTO>>(fullApiUri + "vendors/" + Enc(bvin) + "?key=" +
                                                                          Enc(key));
            return result;
        }

        /// <summary>
        ///     Create a vendor for your store to assign to products in your catalog.
        /// </summary>
        /// <param name="item">A populated instance of VendorManufacturerDTO used to create a new vendor.</param>
        /// <returns>VendorManufacturerDTO</returns>
        public ApiResponse<VendorManufacturerDTO> VendorCreate(VendorManufacturerDTO item)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result = RestHelper.PostRequest<ApiResponse<VendorManufacturerDTO>>(
                fullApiUri + "vendors/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the vendor record with the information specified.
        /// </summary>
        /// <param name="item">A populated instance of VendorManufacturerDTO used to update the existing vendor record.</param>
        /// <returns>VendorManufacturerDTO</returns>
        public ApiResponse<VendorManufacturerDTO> VendorUpdate(VendorManufacturerDTO item)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<VendorManufacturerDTO>>(
                    fullApiUri + "vendors/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the vendor/manufacturer from the store.
        /// </summary>
        /// <param name="bvin">The unique ID of the vendor/manufacturer you want to delete.</param>
        /// <returns>Boolean</returns>
        public ApiResponse<bool> VendorDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "vendors/" + Enc(bvin) + "?key=" + Enc(key),
                    string.Empty);
            return result;
        }

        #endregion

        #region Manufacturers

        /// <summary>
        ///     Allows you to find all manufacturers that are in your store.
        /// </summary>
        /// <returns>List of VendorManufacturerDTO</returns>
        public ApiResponse<List<VendorManufacturerDTO>> ManufacturerFindAll()
        {
            var result = new ApiResponse<List<VendorManufacturerDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<VendorManufacturerDTO>>>(fullApiUri + "manufacturers/?key=" +
                                                                                Enc(key));
            return result;
        }

        /// <summary>
        ///     Find a specific manufacturer using the Bvin of the the respective manufacturer.
        /// </summary>
        /// <param name="bvin">The unique ID of the manufacturer you are looking for.</param>
        /// <returns>VendorManufactureDTO</returns>
        public ApiResponse<VendorManufacturerDTO> ManufacturerFind(string bvin)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<VendorManufacturerDTO>>(fullApiUri + "manufacturers/" + Enc(bvin) +
                                                                          "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Create a manufacturer for your store to assign to products in your catalog.
        /// </summary>
        /// <param name="item">A populated instance of VendorManufacturerDTO used to create a new manufacturer.</param>
        /// <returns>VendorManufacturerDTO</returns>
        public ApiResponse<VendorManufacturerDTO> ManufacturerCreate(VendorManufacturerDTO item)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<VendorManufacturerDTO>>(
                    fullApiUri + "manufacturers/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the manufacturer record with the information specified.
        /// </summary>
        /// <param name="item">A populated instance of VendorManufacturerDTO used to update the existing manufacturer record.</param>
        /// <returns>VendorManufacturerDTO</returns>
        public ApiResponse<VendorManufacturerDTO> ManufacturerUpdate(VendorManufacturerDTO item)
        {
            var result = new ApiResponse<VendorManufacturerDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<VendorManufacturerDTO>>(
                    fullApiUri + "manufacturers/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the manufacturer from the store.
        /// </summary>
        /// <param name="bvin">The unique ID of the manufacturer you want to delete.</param>
        /// <returns>Boolean</returns>
        public ApiResponse<bool> ManufacturerDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "manufacturers/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region ProductTypes

        /// <summary>
        ///     Used to allow you to find all instances of product types in the store.
        /// </summary>
        /// <returns>List of ProductTypeDTO</returns>
        public ApiResponse<List<ProductTypeDTO>> ProductTypesFindAll()
        {
            var result = new ApiResponse<List<ProductTypeDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductTypeDTO>>>(fullApiUri + "producttypes/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Useful to find a single product type in the store.
        /// </summary>
        /// <param name="bvin">The unique ID of the product type to find.</param>
        /// <returns>ProductTypeDTO</returns>
        public ApiResponse<ProductTypeDTO> ProductTypesFind(string bvin)
        {
            var result = new ApiResponse<ProductTypeDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductTypeDTO>>(fullApiUri + "producttypes/" + Enc(bvin) + "?key=" +
                                                                   Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to create a new product type for products to use in your store.
        /// </summary>
        /// <param name="item">ProductTypeDTO - a populated instance of the product type to save.</param>
        /// <returns>ProductTypeDTO - a new instance of ProductTypeDTO loaded from the data source.</returns>
        public ApiResponse<ProductTypeDTO> ProductTypesCreate(ProductTypeDTO item)
        {
            var result = new ApiResponse<ProductTypeDTO>();
            result = RestHelper.PostRequest<ApiResponse<ProductTypeDTO>>(fullApiUri + "producttypes/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates an existing product type using the product type information given.
        /// </summary>
        /// <param name="item">ProductTypeDTO - a populated instance of the product type to update the existing one with.</param>
        /// <returns>ProductTypeDTO - a new instance of the updated product type loaded from the data source.</returns>
        public ApiResponse<ProductTypeDTO> ProductTypesUpdate(ProductTypeDTO item)
        {
            var result = new ApiResponse<ProductTypeDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductTypeDTO>>(
                    fullApiUri + "producttypes/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified product type from the store.
        /// </summary>
        /// <param name="bvin">The unique ID of the product type to delete</param>
        /// <returns>If true, the deletion was successful.</returns>
        public ApiResponse<bool> ProductTypesDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "producttypes/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     Allows you to associate a product property to a property type.
        /// </summary>
        /// <param name="typeBvin">The unique ID of the product type to update.</param>
        /// <param name="propertyId">The unique ID of the product property to assign to the type.</param>
        /// <param name="sortOrder">
        ///     The order that this property should be displayed in the UI when listed with other product
        ///     properties.
        /// </param>
        /// <returns>If true, the new product property association was successfully saved.</returns>
        public ApiResponse<bool> ProductTypesAddProperty(string typeBvin, long propertyId, int sortOrder)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "producttypes/" + Enc(typeBvin) + "/properties/" + propertyId + "/" + sortOrder +
                    "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     When called, this method will remove the association of the product property from the product type.
        /// </summary>
        /// <param name="typeBvin">The unique ID of the product type to update.</param>
        /// <param name="propertyId">The unique ID of the product property to remove from the product type.</param>
        /// <returns>If true, the product property was successfully removed from the product type.</returns>
        public ApiResponse<bool> ProductTypesRemoveProperty(string typeBvin, long propertyId)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "producttypes/" + Enc(typeBvin) + "/properties/" + propertyId + "?key=" + Enc(key),
                    string.Empty);
            return result;
        }

        #endregion

        #region Product Properties

        /// <summary>
        ///     This method allows you to return a listing of all product properties in the store.
        /// </summary>
        /// <returns>List of ProductPropertyDTO</returns>
        public ApiResponse<List<ProductPropertyDTO>> ProductPropertiesFindAll()
        {
            var result = new ApiResponse<List<ProductPropertyDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductPropertyDTO>>>(fullApiUri + "productproperties/?key=" +
                                                                             Enc(key));
            return result;
        }

        /// <summary>
        ///     Used to find a single product property.
        /// </summary>
        /// <param name="id">The unique ID used to find the product property.</param>
        /// <returns>ProductPropertyDTO</returns>
        public ApiResponse<ProductPropertyDTO> ProductPropertiesFind(long id)
        {
            var result = new ApiResponse<ProductPropertyDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductPropertyDTO>>(fullApiUri + "productproperties/" + id + "?key=" +
                                                                       Enc(key));
            return result;
        }

        /// <summary>
        ///     Used to find all product properties for the specified product.
        /// </summary>
        /// <param name="productBvin">The unique ID used to find the product.</param>
        /// <returns>List of ProductPropertyDTO</returns>
        public ApiResponse<List<ProductPropertyDTO>> ProductPropertiesForProduct(string productBvin)
        {
            var result = new ApiResponse<List<ProductPropertyDTO>>();

            result =
                RestHelper.GetRequest<ApiResponse<List<ProductPropertyDTO>>>(fullApiUri + "productproperties/?key=" +
                                                                             Enc(key) + "&byproduct=" + Enc(productBvin));
            
            return result;
        }

        /// <summary>
        ///     Allows you to create a new property to be used with product types.
        /// </summary>
        /// <param name="item">A populated instance of ProductPropertyDTO used to create a new product property.</param>
        /// <returns>ProductPropertyDTO - A populated instance of the product property loaded from the data source.</returns>
        public ApiResponse<ProductPropertyDTO> ProductPropertiesCreate(ProductPropertyDTO item)
        {
            var result = new ApiResponse<ProductPropertyDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductPropertyDTO>>(
                    fullApiUri + "productproperties/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Allows you to update an existing product property.
        /// </summary>
        /// <param name="item">A populated instance of ProductPropertyDTO used to update the existing product property.</param>
        /// <returns>ProductPropertyDTO - an updated instance of the product property loaded from the data source.</returns>
        public ApiResponse<ProductPropertyDTO> ProductPropertiesUpdate(ProductPropertyDTO item)
        {
            var result = new ApiResponse<ProductPropertyDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductPropertyDTO>>(
                    fullApiUri + "productproperties/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified product property from the store.
        /// </summary>
        /// <param name="id">The unique ID used to find the product property to delete.</param>
        /// <returns>If true, the product property was successfully deleted.</returns>
        public ApiResponse<bool> ProductPropertiesDelete(long id)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productproperties/" + id + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     Allows you to set the value of a product property for a specific product.
        /// </summary>
        /// <param name="id">The unique ID used to find the product property.</param>
        /// <param name="productBvin">The unique ID of the product associated with this product property.</param>
        /// <param name="propertyValue">The value of the product property to save.</param>
        /// <param name="choiceId">The choice associated with this value. (Not currently used.)</param>
        /// <returns>If true, the value was updated successfully.</returns>
        public ApiResponse<bool> ProductPropertiesSetValueForProduct(long id, string productBvin, string propertyValue,
            long choiceId)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productproperties/" + id + "/valuesforproduct/" + Enc(productBvin) + "/?key=" +
                    Enc(key), propertyValue);
            return result;
        }

        #endregion

        #region Product Options

        /// <summary>
        ///     This method allows you to find all instances of product options in the store.
        /// </summary>
        /// <returns>List of OptionDTO for the entire store</returns>
        public ApiResponse<List<OptionDTO>> ProductOptionsFindAll()
        {
            var result = new ApiResponse<List<OptionDTO>>();
            result = RestHelper.GetRequest<ApiResponse<List<OptionDTO>>>(fullApiUri + "productoptions/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     When called, this method allows you to find only the product options that match the specified product.
        /// </summary>
        /// <param name="productBvin">The unique ID or primary key of the product.</param>
        /// <returns>List of OptionDTO for the product specified.</returns>
        public ApiResponse<List<OptionDTO>> ProductOptionsFindAllByProductId(string productBvin)
        {
            var result = new ApiResponse<List<OptionDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<OptionDTO>>>(fullApiUri + "productoptions/?key=" + Enc(key) +
                                                                    "&productbvin=" + productBvin);
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific product option using its ID.
        /// </summary>
        /// <param name="bvin">The unique ID or primary key of the option.</param>
        /// <returns>OptionDTO matching the specified ID</returns>
        public ApiResponse<OptionDTO> ProductOptionsFind(string bvin)
        {
            var result = new ApiResponse<OptionDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<OptionDTO>>(fullApiUri + "productoptions/" + Enc(bvin) + "?key=" +
                                                              Enc(key));
            return result;
        }

        /// <summary>
        ///     This method allows you to create a new product option in the store.
        /// </summary>
        /// <param name="item">OptionDTO - a pre-populated instance of the option used to save as a new option</param>
        /// <returns>OptionDTO - a new instance of the option populated from the database.</returns>
        public ApiResponse<OptionDTO> ProductOptionsCreate(OptionDTO item)
        {
            var result = new ApiResponse<OptionDTO>();
            result = RestHelper.PostRequest<ApiResponse<OptionDTO>>(fullApiUri + "productoptions/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     When called, this method allows you to save changes to a specific product option.
        /// </summary>
        /// <param name="item">OptionDTO - an updated instance of the product option that includes your updates.</param>
        /// <returns>OptionDTO - a populated intance of the product option loaded from database that should reflect your updates.</returns>
        public ApiResponse<OptionDTO> ProductOptionsUpdate(OptionDTO item)
        {
            var result = new ApiResponse<OptionDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<OptionDTO>>(
                    fullApiUri + "productoptions/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified product option from the store.
        /// </summary>
        /// <param name="bvin">The unique ID or primary key of the product option to delete.</param>
        /// <returns>If true, the deletion occurred successfully.</returns>
        public ApiResponse<bool> ProductOptionsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productoptions/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     This method allows you to assign a product option to the specified product and optionally generate variants for the
        ///     product using the available options.
        /// </summary>
        /// <param name="newOptionBvin">The unique ID or primary key of the product option to assign to a product.</param>
        /// <param name="productBvin">The unique ID or primary key of the product to assign the option to.</param>
        /// <param name="generateVariants">If true, all possible variants for the product will be generated.</param>
        /// <returns>If true, the product option was successfully assigned to the product.</returns>
        /// <remarks>
        ///     You should generally not generate variants in this method and call the ProductOptionsGenerateAllVariants
        ///     method after all options have been added to the product instead.
        /// </remarks>
        public ApiResponse<bool> ProductOptionsAssignToProduct(string newOptionBvin, string productBvin,
            bool generateVariants)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.PostRequest<ApiResponse<bool>>(fullApiUri + "productoptions/"
                                                                + Enc(newOptionBvin)
                                                                + "/products/"
                                                                + Enc(productBvin)
                                                                + "?key=" + Enc(key)
                                                               + "&generatevariants=" + (generateVariants ? "1" : "0"),
                string.Empty);
            return result;
        }

        /// <summary>
        /// This method allows you to unassign a product option from the specified product.
        /// </summary>
        /// <param name="optionBvin">The unique ID or primary key of the product option to unassign.</param>
        /// <param name="productBvin">The unique ID or primary key of the product to unassign the option from.</param>
        /// <returns>If true, the product option was successfully unassigned from the product.</returns>
        public ApiResponse<bool> ProductOptionsUnassignFromProduct(string optionBvin, string productBvin)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();
            result = RestHelper.PostRequest<ApiResponse<bool>>(this.fullApiUri + "productoptions/"
                                                                + Enc(optionBvin)
                                                                + "/products/"
                                                                + Enc(productBvin)
                                                                + "?key=" + Enc(key)
                                                                + "&unassign=1", string.Empty);
            return result;
        }
        /// <summary>
        /// Generates all possible variants for the specified product.
        /// </summary>
        /// <param name="productBvin">he unique ID or primary key of the product to assign the option to.</param>
        /// <returns>If true, the variants for the product were successfully created.</returns>
        /// <remarks>If no variants were created, then there were no possible variants to create.</remarks>
        public ApiResponse<bool> ProductOptionsGenerateAllVariants(string productBvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productoptions/0/generateonly/" + Enc(productBvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Product Variants
        
        /// <summary>
        ///     Updates an existing product variant with the SKU provided
        /// </summary>
        /// <param name="data">ProductVariantSkuUpdateDTO - the posted product variant object used to update the catalog</param>
        /// <returns>
        ///     Boolean - If true, the product was found and the variant data matched. Therefore the updated variant SKU was
        ///     updated successfully.
        /// </returns>
        public ApiResponse<bool> ProductVariantUpdateSku(ProductVariantSkuUpdateDTO data)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productvariantssku/" + Enc(data.ProductBvin) + "?key=" + Enc(key),
                    Json.ObjectToJson(data));
            return result;
        }

        public ApiResponse<List<VariantDTO>> ProductVariantsFindByProduct(string productId)
        {
            return Get<List<VariantDTO>>("productvariant", null, "productid=" + productId);
        }

        #endregion

        #region Products

        /// <summary>
        ///     Finds and returns all of the products in the catalog.
        /// </summary>
        /// <returns>Liist of ProductDTO</returns>
		public ApiResponse<List<ProductDTO>> ProductsFindAll()
        {
            var products = new ApiResponse<List<ProductDTO>>();
            var result = RestHelper.GetRequest<ApiResponse<PageOfProducts>>(fullApiUri + "products/?key=" + Enc(key));
			if (result.Content != null)
			{
				products.Content = result.Content.Products;
			}
			products.Errors = result.Errors;
			return products;
        }

        /// <summary>
        ///     Finds all of the products that are assigned to the specified category and returns the specific page.
        /// </summary>
        /// <param name="categoryBvin">String - the unique ID of the category</param>
        /// <param name="pageNumber">Integer - the page number of the results to return</param>
        /// <param name="pageSize">Integer - the number of results to return per page</param>
        /// <returns>PageOfProducts - an object that holds a paged collection of products and related information</returns>
        public ApiResponse<PageOfProducts> ProductsFindForCategory(string categoryBvin, int pageNumber, int pageSize)
        {
            var result = new ApiResponse<PageOfProducts>();
            result =
                RestHelper.GetRequest<ApiResponse<PageOfProducts>>(fullApiUri + "products/?key=" + Enc(key) +
                                                                   "&bycategory=" + Enc(categoryBvin) + "&page=" +
                                                                   pageNumber + "&pagesize=" + pageSize);
            return result;
        }

        /// <summary>
        ///     Finds all products in the store and returns only the requested page.
        /// </summary>
        /// <param name="pageNumber">Integer - the page number of the results to return</param>
        /// <param name="pageSize">Integer - the number of results to return per page</param>
        /// <returns>PageOfProducts - an object that holds a paged collection of products and related information</returns>
        public ApiResponse<PageOfProducts> ProductsFindPage(int pageNumber, int pageSize)
        {
            var result = new ApiResponse<PageOfProducts>();
            result =
                RestHelper.GetRequest<ApiResponse<PageOfProducts>>(fullApiUri + "products/?key=" + Enc(key) + "&page=" +
                                                                   pageNumber + "&pagesize=" + pageSize);
            return result;
        }

        /// <summary>
        ///     Returns the total count of products in the store.
        /// </summary>
        /// <returns>Long - the total number of products in the catalog</returns>
        public ApiResponse<long> ProductsCountOfAll()
        {
            var result = new ApiResponse<long>();
            result = RestHelper.GetRequest<ApiResponse<long>>(fullApiUri + "products/?key=" + Enc(key) + "&countonly=1");
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific product in the catalog using the products ID or Bvin.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the product you are looking for</param>
        /// <returns>ProductDTO - an instance of the product</returns>
        public ApiResponse<ProductDTO> ProductsFind(string bvin)
        {
            var result = new ApiResponse<ProductDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductDTO>>(fullApiUri + "products/" + Enc(bvin) + "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific product in the catalog using the product SKU.
        /// </summary>
        /// <param name="sku">String - the unique SKU for the product</param>
        /// <returns>ProductDTO - an instance of the product</returns>
        public ApiResponse<ProductDTO> ProductsFindBySku(string sku)
        {
            var result = new ApiResponse<ProductDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductDTO>>(fullApiUri + "products/ANY?key=" + Enc(key) + "&bysku=" +
                                                               Enc(sku));
            return result;
        }

        /// <summary>
        ///     Allows you to find a specific product in the catalog using the slug of the product.
        /// </summary>
        /// <param name="slug">String - the unique slug of the product</param>
        /// <returns>ProductDTO - an instance of the product</returns>
        public ApiResponse<ProductDTO> ProductsBySlug(string slug)
        {
            var result = new ApiResponse<ProductDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductDTO>>(fullApiUri + "products/ANY?key=" + Enc(key) + "&byslug=" +
                                                               Enc(slug));
            return result;
        }

        /// <summary>
        ///     Creates a new product in the store using the posted data.
        /// </summary>
        /// <param name="item">ProductDTO - the posted product object to create in the store</param>
        /// <param name="imageData">Byte - the image data for the primary product image</param>
        /// <returns>ProductDTO - an instance of the product</returns>
        public ApiResponse<ProductDTO> ProductsCreate(ProductDTO item, byte[] imageData)
        {
            var result = new ApiResponse<ProductDTO>();
            result = RestHelper.PostRequest<ApiResponse<ProductDTO>>(fullApiUri + "products/?key=" + Enc(key),
                Json.ObjectToJson(item));

            if (result.Content != null)
            {
                ProductsMainImageUpload(result.Content.Bvin, result.Content.ImageFileSmall, imageData);
            }

            if (imageData == null)
            {
                result.Errors.Add(new ApiError("WARNING", "No image was included in the Post."));
            }
            return result;
        }

        /// <summary>
        ///     Updates an existing product in the catalog using the posted product object.
        /// </summary>
        /// <param name="item">ProductDTO - the posted product object to update in the catalog</param>
        /// <returns>ProductDTO - an updated instance of the product</returns>
        public ApiResponse<ProductDTO> ProductsUpdate(ProductDTO item)
        {
            var result = new ApiResponse<ProductDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductDTO>>(
                    fullApiUri + "products/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified product from the catalog.
        /// </summary>
        /// <param name="bvin">String - the unique ID of the product</param>
        /// <returns>Boolean - if true, the product was successfully deleted</returns>
        public ApiResponse<bool> ProductsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + "products/" + Enc(bvin) + "?key=" + Enc(key),
                    string.Empty);
            return result;
        }

        /// <summary>
        ///     Permanently deletes all of the products in the store, provided that this has been allowed in the store admin API
        ///     page.
        /// </summary>
        /// <param name="howMany">
        ///     Integer - The number of products you want to delete at once. Supply the total count except if the
        ///     catalog deletion will take longer than an HttpRequest allows.
        /// </param>
        /// <returns>ClearProductsData - a populated response detailing how many products were deleted</returns>
        /// <remarks>Requires additional permissions in the store API administration</remarks>
        public ApiResponse<ClearProductsData> ProductsClearAll(int howMany)
        {
            var result = new ApiResponse<ClearProductsData>();
            result =
                RestHelper.DeleteRequest<ApiResponse<ClearProductsData>>(
                    fullApiUri + "products/?key=" + Enc(key) + "&howmany=" + howMany, string.Empty);
            return result;
        }

        #endregion

        #region Product Relationships

        /// <summary>
        ///     Used to find a specific product relationship in the store catalog.
        /// </summary>
        /// <param name="id">The unique ID of the product relationship.</param>
        /// <returns>ProductRelationshipDTO</returns>
        public ApiResponse<ProductRelationshipDTO> ProductRelationshipsFind(long id)
        {
            var result = new ApiResponse<ProductRelationshipDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductRelationshipDTO>>(fullApiUri + "productrelationships/" + id +
                                                                           "?key=" + Enc(key));
            return result;
        }

		/// <summary>
        ///     Products the relationships find.
		/// </summary>
		/// <param name="productBvin">The product identifier.</param>
		/// <returns></returns>
        public ApiResponse<List<ProductRelationshipDTO>> ProductRelationshipsForProduct(string productBvin)
        {
            var result = new ApiResponse<List<ProductRelationshipDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductRelationshipDTO>>>(fullApiUri + "productrelationships/" +
                                                                                 "?key=" + Enc(key) + "&byproduct=" +
                                                                                 Enc(productBvin));
            return result;
        }

        /// <summary>
        ///     Allows you to create a new product relationship in the store.
        /// </summary>
        /// <param name="item">A populated instance of ProductRelationshipDTO</param>
        /// <returns>ProductRelationshipDTO updated from the data source</returns>
        public ApiResponse<ProductRelationshipDTO> ProductRelationshipsCreate(ProductRelationshipDTO item)
        {
            var result = new ApiResponse<ProductRelationshipDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductRelationshipDTO>>(
                    fullApiUri + "productrelationships/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     A shortcut method to creating a product relationship that requires less information up front. It simply calls
        ///     ProductRelationshipsCreate for you.
        /// </summary>
        /// <param name="productBvin">String - the unique ID of the primary product</param>
        /// <param name="otherProductBvin">String - the unique ID of the product to relate to the primary product</param>
        /// <param name="isSubstitute">
        ///     Boolean - should be passed as False, but this property is not currently used in the
        ///     application.
        /// </param>
        /// <returns>If true, the product relationship was created successfully</returns>
        public ApiResponse<bool> ProductRelationshipsQuickCreate(string productBvin, string otherProductBvin,
            bool isSubstitute)
        {
            var response = new ApiResponse<bool>();

            var r = new ProductRelationshipDTO
            {
                IsSubstitute = isSubstitute,
                ProductId = productBvin,
                RelatedProductId = otherProductBvin
            };

            var result = ProductRelationshipsCreate(r);
            if (result.Content != null)
            {
                if (result.Content.Id > 0)
                {
                    response.Content = true;
                }
            }

            response.Errors = result.Errors;
            return response;
        }

        /// <summary>
        ///     When used, you are able to update an existing product relationship.
        /// </summary>
        /// <param name="item">A populated instance of ProductRelationshipDTO with updates made to it.</param>
        /// <returns>An updated instance of ProductRelationshipDTO from the data source.</returns>
        public ApiResponse<ProductRelationshipDTO> ProductRelationshipsUpdate(ProductRelationshipDTO item)
        {
            var result = new ApiResponse<ProductRelationshipDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductRelationshipDTO>>(
                    fullApiUri + "productrelationships/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     The products that are specified become unrelated from one another. This is the equivalent of a delete.
        /// </summary>
        /// <param name="productBvin">String - the unique ID of the primary product</param>
        /// <param name="otherProductBvin">String - the unique ID of the product to relate to the primary product</param>
        /// <returns>If true, a relation was found and was successfully undone.</returns>
        public ApiResponse<bool> ProductRelationshipsUnrelate(string productBvin, string otherProductBvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productrelationships/" + Enc(productBvin) + "/" + Enc(otherProductBvin) + "?key=" +
                    Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Image Upload Helpers

        public ApiResponse<bool> ProductsMainImageUpload(string productBvin, string fileName, byte[] imageData)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productmainimage/" + Enc(productBvin) + "?key=" + Enc(key) + "&filename=" +
                    Enc(fileName),
                    Json.ObjectToJson(imageData));
            return response;
        }

        public ApiResponse<bool> CategoriesImagesIconUpload(string categoryBvin, string fileName, byte[] imageData)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "categoriesimagesicon/" + Enc(categoryBvin) + "?key=" + Enc(key) + "&filename=" +
                    Enc(fileName),
                    Json.ObjectToJson(imageData));
            return response;
        }

        public ApiResponse<bool> CategoriesImagesBannerUpload(string categoryBvin, string fileName, byte[] imageData)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "categoriesimagesbanner/" + Enc(categoryBvin) + "?key=" + Enc(key) + "&filename=" +
                    Enc(fileName),
                    Json.ObjectToJson(imageData));
            return response;
        }

        #endregion

        #region Product Files

        public ApiResponse<List<ProductFileDTO>> ProductFilesFindForProduct(string bvin)
        {
            var result = new ApiResponse<List<ProductFileDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductFileDTO>>>(fullApiUri + "productfilesxproducts/" +
                                                                         Enc(bvin) + "?key=" + Enc(key));
            return result;
        }

        public ApiResponse<ProductFileDTO> ProductFilesFind(string bvin)
        {
            var result = new ApiResponse<ProductFileDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductFileDTO>>(fullApiUri + "productfiles/" + Enc(bvin) + "?key=" +
                                                                   Enc(key));
            return result;
        }

        public ApiResponse<ProductFileDTO> ProductFilesCreate(ProductFileDTO item)
        {
            var result = new ApiResponse<ProductFileDTO>();
            result = RestHelper.PostRequest<ApiResponse<ProductFileDTO>>(fullApiUri + "productfiles/?key=" + Enc(key),
                Json.ObjectToJson(item));
            return result;
        }

        public ApiResponse<ProductFileDTO> ProductFilesUpdate(ProductFileDTO item)
        {
            var result = new ApiResponse<ProductFileDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductFileDTO>>(
                    fullApiUri + "productfiles/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        public ApiResponse<bool> ProductFilesDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productfiles/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        public ApiResponse<bool> ProductFilesAddToProduct(string productBvin, string fileBvin, int availableMinutes,
            int maxDownloads)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productfilesxproducts/" + Enc(fileBvin) + "/" + Enc(productBvin) + "?key=" + Enc(key) +
                    "&minutes=" + availableMinutes + "&downloads=" + maxDownloads, string.Empty);
            return result;
        }

        public ApiResponse<bool> ProductFilesRemoveFromProduct(string productBvin, string fileBvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productfilesxproducts/" + Enc(fileBvin) + "/" + Enc(productBvin) + "?key=" + Enc(key),
                    string.Empty);
            return result;
        }

        public ApiResponse<bool> ProductFilesDataUploadFirstPart(string fileBvin, string fileName, string description,
            byte[] data)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productfilesdata/" + Enc(fileBvin) + "?key=" + Enc(key) + "&first=1" + "&filename=" +
                    Enc(fileName) + "&description=" + Enc(description),
                    Json.ObjectToJson(data));
            return response;
        }

        public ApiResponse<bool> ProductFilesDataUploadAdditionalPart(string fileBvin, string fileName, byte[] moreData)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productfilesdata/" + Enc(fileBvin) + "?key=" + Enc(key) + "&first=0" + "&filename=" +
                    Enc(fileName),
                    Json.ObjectToJson(moreData));
            return response;
        }

        #endregion

        #region Product Inventory

        /// <summary>
        ///     Returns a single record representing the inventory levels of a product.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product inventory record to look for</param>
        /// <returns>ProductInventoryDTO - a saved instance retrieved from the data source.</returns>
        public ApiResponse<ProductInventoryDTO> ProductInventoryFind(string bvin)
        {
            var result = new ApiResponse<ProductInventoryDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductInventoryDTO>>(fullApiUri + "productinventory/" + Enc(bvin) +
                                                                        "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Allows you to create an inventory record for a specific product or product variant.
        /// </summary>
        /// <param name="item">A populated instance of a ProductInventoryDTO object with intended updates applied.</param>
        /// <returns>ProductInventoryDTO - a saved instance retrieved from the data source.</returns>
        public ApiResponse<ProductInventoryDTO> ProductInventoryCreate(ProductInventoryDTO item)
        {
            var result = new ApiResponse<ProductInventoryDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductInventoryDTO>>(
                    fullApiUri + "productinventory/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the inventory levels of a product or product variant.
        /// </summary>
        /// <param name="item">A populated instance of a ProductInventoryDTO object with intended updates applied.</param>
        /// <returns>ProductInventoryDTO - a saved instance retrieved from the data source.</returns>
        public ApiResponse<ProductInventoryDTO> ProductInventoryUpdate(ProductInventoryDTO item)
        {
            return Post("productinventory", item.Bvin, item);
        }

        /// <summary>
        ///     Permanently deletes the inventory levels for a product.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product inventory record.</param>
        /// <returns>Boolean - If true, the product inventory data was successfully deleted.</returns>
        public ApiResponse<bool> ProductInventoryDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productinventory/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        /// <summary>
        ///     Returns a list of Product Inventories.
        /// </summary>
        /// <returns>
        ///     List of ProductInventoryDTO - a list of the populated ProductInventoryDTO objects for all store Product
        ///     Inventory
        /// </returns>
        public ApiResponse<List<ProductInventoryDTO>> ProductInventoryFindAll()
        {
            var result = new ApiResponse<List<ProductInventoryDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductInventoryDTO>>>(fullApiUri + "productinventory/?key=" +
                                                                              Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns all of the inventories that the specified product belongs to in the store.
        /// </summary>
        /// <param name="productBvin">String - the unique ID of the product you wish to search on</param>
        /// <returns>List of ProductInventoryDTO - a list of the populated Product inventory  objects that match the product</returns>
        public ApiResponse<List<ProductInventoryDTO>> ProductInventoryFindForProduct(string productBvin)
        {
            var result = new ApiResponse<List<ProductInventoryDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductInventoryDTO>>>(fullApiUri + "productinventory/?key=" +
                                                                              Enc(key) + "&byproduct=" +
                                                                              Enc(productBvin));
            return result;
        }

        #endregion

        #region Product Images

        public ApiResponse<List<ProductImageDTO>> ProductImagesFindAll()
        {
            var result = new ApiResponse<List<ProductImageDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductImageDTO>>>(fullApiUri + "productimages/?key=" + Enc(key));
            return result;
        }

        public ApiResponse<List<ProductImageDTO>> ProductImagesFindAllByProduct(string productBvin)
        {
            var result = new ApiResponse<List<ProductImageDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductImageDTO>>>(fullApiUri + "productimages/?key=" + Enc(key) +
                                                                          "&byproduct=" + Enc(productBvin));
            return result;
        }

        public ApiResponse<ProductImageDTO> ProductImagesFind(string bvin)
        {
            var result = new ApiResponse<ProductImageDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductImageDTO>>(fullApiUri + "productimages/" + Enc(bvin) + "?key=" +
                                                                    Enc(key));
            return result;
        }

        public ApiResponse<ProductImageDTO> ProductImagesCreate(ProductImageDTO item, byte[] data)
        {
            var result = new ApiResponse<ProductImageDTO>();
            result = RestHelper.PostRequest<ApiResponse<ProductImageDTO>>(
                fullApiUri + "productimages/?key=" + Enc(key), Json.ObjectToJson(item));
            if (data != null)
            {
                if (result.Content != null && result.Errors.Count < 1)
                {
                    ProductImagesUpload(item.ProductId, result.Content.Bvin, result.Content.FileName, data);
                }
            }
            return result;
        }

        public ApiResponse<ProductImageDTO> ProductImagesUpdate(ProductImageDTO item)
        {
            var result = new ApiResponse<ProductImageDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductImageDTO>>(
                    fullApiUri + "productimages/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        public ApiResponse<bool> ProductImagesDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productimages/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        public ApiResponse<bool> ProductImagesUpload(string productBvin, string imageBvin, string fileName,
            byte[] imageData)
        {
            var response = new ApiResponse<bool>();
            response =
                RestHelper.PostRequest<ApiResponse<bool>>(
                    fullApiUri + "productimagesupload/" + Enc(productBvin) + "/" + Enc(imageBvin) + "?key=" + Enc(key)
                                                        + "&filename=" + Enc(fileName)
                    , Json.ObjectToJson(imageData));
            return response;
        }

        #endregion

        #region  Product Volume Discounts

        /// <summary>
        ///     Finds a single product volume discount by it's ID or Bvin.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product volume discount.</param>
        /// <returns>ProductVolumeDiscountDTO</returns>
        public ApiResponse<ProductVolumeDiscountDTO> ProductVolumeDiscountsFind(string bvin)
        {
            var result = new ApiResponse<ProductVolumeDiscountDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductVolumeDiscountDTO>>(fullApiUri + "productvolumediscounts/" +
                                                                             Enc(bvin) + "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Creates a new product volume discount and returns the new product volume discount back to you.
        /// </summary>
        /// <param name="item">ProductVolumeDiscountDTO - a populated instance of the product volume discount object.</param>
        /// <returns>ProductVolumeDiscountDTO</returns>
        public ApiResponse<ProductVolumeDiscountDTO> ProductVolumeDiscountsCreate(ProductVolumeDiscountDTO item)
        {
            var result = new ApiResponse<ProductVolumeDiscountDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductVolumeDiscountDTO>>(
                    fullApiUri + "productvolumediscounts/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Allows you to update an existing product volume discount and returns a copy of the updated object back to you.
        /// </summary>
        /// <param name="item">
        ///     ProductVolumeDiscountDTO - a populated instance of the product volume discount with the desired
        ///     updates.
        /// </param>
        /// <returns></returns>
        public ApiResponse<ProductVolumeDiscountDTO> ProductVolumeDiscountsUpdate(ProductVolumeDiscountDTO item)
        {
            var result = new ApiResponse<ProductVolumeDiscountDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductVolumeDiscountDTO>>(
                    fullApiUri + "productvolumediscounts/" + Enc(item.Bvin) + "?key=" + Enc(key),
                    Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes a product volune discount using its ID or Bvin.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product volume discount.</param>
        /// <returns>Boolean - if true, the product volume discount was successfully deleted.</returns>
        public ApiResponse<bool> ProductVolumeDiscountsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productvolumediscounts/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Product Reviews

        /// <summary>
        ///     Allows you to return all of the reviews in the store.
        /// </summary>
        /// <returns>List of ProductReviewDTO</returns>
        public ApiResponse<List<ProductReviewDTO>> ProductReviewsFindAll()
        {
            var result = new ApiResponse<List<ProductReviewDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductReviewDTO>>>(fullApiUri + "productreviews/?key=" +
                                                                           Enc(key));
            return result;
        }

        /// <summary>
        ///     Returns a list of all of the reviews that were created for the specified product.
        /// </summary>
        /// <param name="productBvin">String - the unique ID or Bvin of the product</param>
        /// <returns>List or ProductReviewDTO</returns>
        public ApiResponse<List<ProductReviewDTO>> ProductReviewsByProduct(string productBvin)
        {
            var result = new ApiResponse<List<ProductReviewDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<ProductReviewDTO>>>(fullApiUri + "productreviews/?key=" +
                                                                           Enc(key) + "&productbvin=" + Enc(productBvin));
            return result;
        }

        /// <summary>
        ///     Allows you to find a specified product review using its unique ID or Bvin.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product review.</param>
        /// <returns>ProductReviewDTO - the product review matching the given Bvin.</returns>
        public ApiResponse<ProductReviewDTO> ProductReviewsFind(string bvin)
        {
            var result = new ApiResponse<ProductReviewDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<ProductReviewDTO>>(fullApiUri + "productreviews/" + Enc(bvin) +
                                                                     "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Creates a new product review for a product in the store.
        /// </summary>
        /// <param name="item">ProductReviewDTO - a populated product review object.</param>
        /// <returns>ProductReviewDTO - a fresh instance of the product review object loaded from the data source.</returns>
        public ApiResponse<ProductReviewDTO> ProductReviewsCreate(ProductReviewDTO item)
        {
            var result = new ApiResponse<ProductReviewDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductReviewDTO>>(fullApiUri + "productreviews/?key=" + Enc(key),
                    Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the specified product review using the product review object provided.
        /// </summary>
        /// <param name="item">ProductReviewDTO - a populated product review object with the intended updates.</param>
        /// <returns>ProductReviewDTO - a fresh instance of the product review object loaded from the data source.</returns>
        public ApiResponse<ProductReviewDTO> ProductReviewsUpdate(ProductReviewDTO item)
        {
            var result = new ApiResponse<ProductReviewDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<ProductReviewDTO>>(
                    fullApiUri + "productreviews/" + Enc(item.Bvin) + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes a product review from the store. This should only be done as a last resort.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the product review.</param>
        /// <returns>Boolean - if true, the deletion was executed successfully.</returns>
        public ApiResponse<bool> ProductReviewsDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "productreviews/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Category Product Association

        /// <summary>
        ///     Allows you to find a specific category/product association.
        /// </summary>
        /// <param name="id">The unique ID or Bvin of the category/product association</param>
        /// <returns>CategoryProductAssociationDTO</returns>
        public ApiResponse<CategoryProductAssociationDTO> CategoryProductAssociationsFind(long id)
        {
            var result = new ApiResponse<CategoryProductAssociationDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<CategoryProductAssociationDTO>>(fullApiUri +
                                                                                  "categoryproductassociations/" + id +
                                                                                  "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Adds the specified product to the specified category using an instance of the CategoryProductAssociationDTO object.
        /// </summary>
        /// <param name="item">A populated instance of the category/product association used to create a new one in the store.</param>
        /// <returns>CategoryProductAssociationDTO</returns>
        public ApiResponse<CategoryProductAssociationDTO> CategoryProductAssociationsCreate(
            CategoryProductAssociationDTO item)
        {
            var result = new ApiResponse<CategoryProductAssociationDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CategoryProductAssociationDTO>>(
                    fullApiUri + "categoryproductassociations/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Adds the specified product to the specified category using only the ID of the respective category and product.
        /// </summary>
        /// <param name="productBvin">The unique ID or Bvin of the product to associate with the category</param>
        /// <param name="categoryBvin">The unique ID or Bvin of the category to associate with the product</param>
        /// <returns>Boolean = if true, the category/product association was successfully created.</returns>
        public ApiResponse<bool> CategoryProductAssociationsQuickCreate(string productBvin, string categoryBvin)
        {
            var response = new ApiResponse<bool>();

            var a = new CategoryProductAssociationDTO
            {
                CategoryId = categoryBvin,
                ProductId = productBvin
            };

            var result = CategoryProductAssociationsCreate(a);
            if (result.Content != null)
            {
                if (result.Content.Id > 0)
                {
                    response.Content = true;
                }
            }
            response.Errors = result.Errors;
            return response;
        }

        /// <summary>
        ///     Updates an existing product/category relationship or association.
        /// </summary>
        /// <param name="item">
        ///     A populated instance of the category/product association used to update an existing one in the
        ///     store.
        /// </param>
        /// <returns>CategoryProductAssociationDTO</returns>
        public ApiResponse<CategoryProductAssociationDTO> CategoryProductAssociationsUpdate(
            CategoryProductAssociationDTO item)
        {
            var result = new ApiResponse<CategoryProductAssociationDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<CategoryProductAssociationDTO>>(
                    fullApiUri + "categoryproductassociations/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently removes the association between the product and the category.
        /// </summary>
        /// <param name="productBvin">The unique ID or Bvin of the product to associate with the category</param>
        /// <param name="categoryBvin">The unique ID or Bvin of the category to associate with the product</param>
        /// <returns>Boolean - if true, the category/product association was successfully unrelated.</returns>
        public ApiResponse<bool> CategoryProductAssociationsUnrelate(string productBvin, string categoryBvin)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "categoryproductassociations/" + Enc(productBvin) + "/" + Enc(categoryBvin) + "?key=" +
                    Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Orders

        /// <summary>
        ///     Allows you to find all orders in the store.
        /// </summary>
        /// <returns>List of OrderSnapshotDTO</returns>
        public ApiResponse<List<OrderSnapshotDTO>> OrdersFindAll()
        {
            var result = new ApiResponse<List<OrderSnapshotDTO>>();
            result = RestHelper.GetRequest<ApiResponse<List<OrderSnapshotDTO>>>(fullApiUri + "orders/?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Enables you to find a specific order in your store.
        /// </summary>
        /// <param name="bvin">String - the unique ID or bvin of the order to delete.</param>
        /// <returns></returns>
        public ApiResponse<OrderDTO> OrdersFind(string bvin)
        {
            var result = new ApiResponse<OrderDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<OrderDTO>>(fullApiUri + "orders/" + Enc(bvin) + "?key=" + Enc(key));
            return result;
        }

		/// <summary>
        ///     Creates a new order in your store.
		/// </summary>
		/// <param name="item">OrderDTO - a populated instance of a new order</param>
		/// <param name="recalculateOrder">if set to <c>true</c> [recalculate order].</param>
		/// <returns>
        ///     OrderDTO - a populated instance of an order loaded from the data source.
		/// </returns>
        public ApiResponse<OrderDTO> OrdersCreate(OrderDTO item, bool recalculateOrder = false)
        {
            return Post("orders", null, item, recalculateOrder ? "recalc=1" : "recalc=0");
        }

		/// <summary>
        ///     Updates an existing order with the given changes.
		/// </summary>
		/// <param name="item">OrderDTO - a populated instance of an updated order</param>
		/// <param name="recalculateOrder">if set to <c>true</c> [recalculate order].</param>
		/// <returns>
        ///     OrderDTO - a populated instance of an order loaded from the data source.
		/// </returns>
        public ApiResponse<OrderDTO> OrdersUpdate(OrderDTO item, bool recalculateOrder = false)
        {
            return Post("orders", item.Bvin, item, recalculateOrder ? "recalc=1" : "recalc=0");
        }

        /// <summary>
        ///     Permanently deletes an order from the store.
        /// </summary>
        /// <param name="bvin">String - the unique ID or bvin of the order to delete.</param>
        /// <returns>Boolean - if true, the order was successfully found and deleted.</returns>
        public ApiResponse<bool> OrdersDelete(string bvin)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(
                fullApiUri + "orders/" + Enc(bvin) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Order Transactions

        /// <summary>
        ///     Returns all transactions for the specified order.
        /// </summary>
        /// <param name="bvin">String - the unique ID or Bvin of the order to find transactions for.</param>
        /// <returns>List of OrderTransactionDTO</returns>
        public ApiResponse<List<OrderTransactionDTO>> OrderTransactionsFindForOrder(string bvin)
        {
            var result = new ApiResponse<List<OrderTransactionDTO>>();
            result =
                RestHelper.GetRequest<ApiResponse<List<OrderTransactionDTO>>>(fullApiUri + "ordertransactions/?key=" +
                                                                              Enc(key) + "&orderbvin=" + Enc(bvin));
            return result;
        }

        /// <summary>
        ///     Allows you to look for and return a specific transaction from the store.
        /// </summary>
        /// <param name="id">String - the transaction ID or bvin to look for.</param>
        /// <returns>OrderTransactionDTO</returns>
        public ApiResponse<OrderTransactionDTO> OrderTransactionsFind(Guid id)
        {
            var result = new ApiResponse<OrderTransactionDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<OrderTransactionDTO>>(fullApiUri + "ordertransactions/" +
                                                                        Enc(id.ToString()) + "?key=" + Enc(key));
            return result;
        }

        /// <summary>
        ///     Creates a new transaction for an order in the store.
        /// </summary>
        /// <param name="item">A populated instance of OrderTransactionDTO to save for an order.</param>
        /// <returns>OrderTransactionDTO</returns>
        public ApiResponse<OrderTransactionDTO> OrderTransactionsCreate(OrderTransactionDTO item)
        {
            var result = new ApiResponse<OrderTransactionDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<OrderTransactionDTO>>(
                    fullApiUri + "ordertransactions/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates an existing transaction with the changes in the given transaction.
        /// </summary>
        /// <param name="item">A populated instance of OrderTransactionDTO used to save changes to an existing transaction.</param>
        /// <returns>OrderTransactionDTO</returns>
        public ApiResponse<OrderTransactionDTO> OrderTransactionsUpdate(OrderTransactionDTO item)
        {
            var result = new ApiResponse<OrderTransactionDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<OrderTransactionDTO>>(
                    fullApiUri + "ordertransactions/" + Enc(item.Id.ToString()) + "?key=" + Enc(key),
                    Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified transaction from the store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApiResponse<bool> OrderTransactionsDelete(Guid id)
        {
            var result = new ApiResponse<bool>();
            result =
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "ordertransactions/" + Enc(id.ToString()) + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Wish List Items

        /// <summary>
        ///     Allows you to find a specific wish list item using its ID.
        /// </summary>
        /// <param name="id">Long - the unique ID or primary key of the wish list item</param>
        /// <returns>WishListItemDTO</returns>
        public ApiResponse<WishListItemDTO> WishListItemsFind(long id)
        {
            var result = new ApiResponse<WishListItemDTO>();
            result =
                RestHelper.GetRequest<ApiResponse<WishListItemDTO>>(fullApiUri + "wishlistitems/" + id + "?key=" +
                                                                    Enc(key));
            return result;
        }

        /// <summary>
        ///     Look for and return all wish list items matching the given customer ID.
        /// </summary>
        /// <param name="customerId">ID number for the customer to find wish list items for</param>
        /// <returns>List of WishListItemDTO</returns>
        public ApiResponse<List<WishListItemDTO>> WishListItemsFindForCustomer(string customerId)
        {
            return Get<List<WishListItemDTO>>("wishlistitems", null, "id=" + customerId);
        }

        /// <summary>
        ///     Look for and return all wish list items for the store.
        /// </summary>
        /// <returns>List of WishListItemDTO</returns>
        public ApiResponse<List<WishListItemDTO>> WishListItemsFindAll()
        {
            return Get<List<WishListItemDTO>>("wishlistitems", null, string.Empty);
        }

        /// <summary>
        ///     Creates a new wish list item.
        /// </summary>
        /// <param name="item">A populated instance of the wish list item to save.</param>
        /// <returns>WishListItemDTO - a fresh version of the wish list item from the data source.</returns>
        public ApiResponse<WishListItemDTO> WishListItemsCreate(WishListItemDTO item)
        {
            var result = new ApiResponse<WishListItemDTO>();
            result = RestHelper.PostRequest<ApiResponse<WishListItemDTO>>(
                fullApiUri + "wishlistitems/?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Updates the wish list item with the given wish list object.
        /// </summary>
        /// <param name="item">An updated version of the wish list item</param>
        /// <returns>WishListItemDTO - a fresh version of the wish list item updated from the data source.</returns>
        public ApiResponse<WishListItemDTO> WishListItemsUpdate(WishListItemDTO item)
        {
            var result = new ApiResponse<WishListItemDTO>();
            result =
                RestHelper.PostRequest<ApiResponse<WishListItemDTO>>(
                    fullApiUri + "wishlistitems/" + item.Id + "?key=" + Enc(key), Json.ObjectToJson(item));
            return result;
        }

        /// <summary>
        ///     Permanently deletes the specified wish list item.
        /// </summary>
        /// <param name="id">Long - the unique ID or primary key of the wish list item</param>
        /// <returns>Boolean - if true, the wish list item was successfully deleted</returns>
        public ApiResponse<bool> WishListItemsDelete(long id)
        {
            var result = new ApiResponse<bool>();
            result = RestHelper.DeleteRequest<ApiResponse<bool>>(
                fullApiUri + "wishlistitems/" + id + "?key=" + Enc(key), string.Empty);
            return result;
        }

        #endregion

        #region Catalog Roles

        /// <summary>
        ///     Returns a collection of the catalog roles that are assigned to the specified product.
        /// </summary>
        /// <param name="id">The unique ID of the product to find catalog roles in</param>
        /// <returns>List of CatalogRoleDTO</returns>
        public ApiResponse<List<CatalogRoleDTO>> CatalogRoleFindByProduct(string id)
        {
            return RestHelper.GetRequest<ApiResponse<List<CatalogRoleDTO>>>(fullApiUri
                                                                            + "catalogroles/" + id + "?key=" + Enc(key) +
                                                                            "&bytype=product&bvin=" + Enc(id));
        }

        /// <summary>
        ///     Returns a collection of the catalog roles that are assigned to the specified category.
        /// </summary>
        /// <param name="id">The unique ID of the category to find catalog roles in</param>
        /// <returns>List of CatalogRoleDTO</returns>
        public ApiResponse<List<CatalogRoleDTO>> CatalogRoleFindByCategory(string id)
        {
            return RestHelper.GetRequest<ApiResponse<List<CatalogRoleDTO>>>(fullApiUri
                                                                            + "catalogroles/" + id + "?key=" + Enc(key) +
                                                                            "&bytype=category&bvin=" + Enc(id));
        }

        /// <summary>
        ///     Returns a collection of the catalog roles that are assigned to the specified product type.
        /// </summary>
        /// <param name="id">The unique ID of the product type to find catalog roles in</param>
        /// <returns>List of CatalogRoleDTO</returns>
        public ApiResponse<List<CatalogRoleDTO>> CatalogRoleFindByProductType(string id)
        {
            return RestHelper.GetRequest<ApiResponse<List<CatalogRoleDTO>>>(fullApiUri
                                                                            + "catalogroles/" + id + "?key=" + Enc(key) +
                                                                            "&bytype=producttype&bvin=" + Enc(id));
        }

        /// <summary>
        ///     Allows you to create a new catalog role assignment.
        /// </summary>
        /// <param name="item">CatalogRoleDTO - a populated instance of catalog role</param>
        /// <returns>CatalogRoleDTO - a fresh instance of catalog role from the data source</returns>
        public ApiResponse<CatalogRoleDTO> CatalogRoleCreate(CatalogRoleDTO item)
        {
            return RestHelper.PostRequest<ApiResponse<CatalogRoleDTO>>(fullApiUri
                                                                       + "catalogroles/?key=" + Enc(key),
                Json.ObjectToJson(item));
        }

        /// <summary>
        ///     Permanently deletes the specified catalog role.
        /// </summary>
        /// <param name="id">The unique ID or primary key of the catalog role.</param>
        /// <returns>Boolean - if true, the catalog role was successfully deleted.</returns>
        public ApiResponse<bool> CatalogRoleDelete(long id)
        {
            return RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri
                + "catalogroles/" + id + "?key=" + Enc(key), string.Empty);
        }

        #endregion

        #region Gift Cards

        /// <summary>
        ///     Allows you to find all gift cards that are in the store.
        /// </summary>
        /// <returns>List of GiftCardDTO</returns>
        public ApiResponse<List<GiftCardDTO>> GiftCardFindAll()
        {
            return RestHelper.GetRequest<ApiResponse<List<GiftCardDTO>>>(fullApiUri + "giftcards/?key=" + Enc(key));
        }

        /// <summary>
        ///     Returns all of the gift cards in the store, but in a paged format.
        /// </summary>
        /// <param name="pageNumber">the page number of results that you want to have</param>
        /// <param name="pageSize">the size of the pages you want to have</param>
        /// <returns>List of GiftCardDTO</returns>
        public ApiResponse<List<GiftCardDTO>> GiftCardFindAllByPage(int pageNumber, int pageSize)
        {
            return
                RestHelper.GetRequest<ApiResponse<List<GiftCardDTO>>>(fullApiUri + "giftcards/?key=" + Enc(key) +
                                                                      "&page=" + pageNumber + "&pagesize=" + pageSize);
        }

        /// <summary>
        ///     Look for and return a specific gift card by it's ID (not card number).
        /// </summary>
        /// <param name="id">The unique ID of the gift card (not the card number)</param>
        /// <returns>GiftCardDTO</returns>
        public ApiResponse<GiftCardDTO> GiftCardFind(long id)
        {
            return
                RestHelper.GetRequest<ApiResponse<GiftCardDTO>>(fullApiUri + "giftcards/" + Enc(id.ToString()) + "?key=" +
                                                                Enc(key));
        }

        /// <summary>
        ///     Creates a new gift card in the store, with the posted properties.
        /// </summary>
        /// <param name="item">A populated instance of GiftCardDTO as you want it to be added to the store</param>
        /// <returns>GiftCardDTO</returns>
        public ApiResponse<GiftCardDTO> GiftCardCreate(GiftCardDTO item)
        {
            return RestHelper.PostRequest<ApiResponse<GiftCardDTO>>(fullApiUri + "giftcards/?key=" + Enc(key),
                Json.ObjectToJson(item));
        }

        /// <summary>
        ///     Updates the specified gift card with the populated gift card object.
        /// </summary>
        /// <param name="item">A populated instance of GiftCardDTO as you want the updates to be applied in the store</param>
        /// <returns>GiftCardDTO</returns>
        public ApiResponse<GiftCardDTO> GiftCardUpdate(GiftCardDTO item)
        {
            return
                RestHelper.PostRequest<ApiResponse<GiftCardDTO>>(
                    fullApiUri + "giftcards/" + Enc(item.GiftCardId.ToString()) + "?key=" + Enc(key),
                    Json.ObjectToJson(item));
        }

        /// <summary>
        ///     Permanently deletes the specified gift card from the store.
        /// </summary>
        /// <param name="id">The unique ID of the gift card (not the card number)</param>
        /// <returns>Boolean - if true, the delete was successful</returns>
        public ApiResponse<bool> GiftCardDelete(long id)
        {
            return
                RestHelper.DeleteRequest<ApiResponse<bool>>(
                    fullApiUri + "giftcards/" + Enc(id.ToString()) + "?key=" + Enc(key), string.Empty);
        }

        /// <summary>
        ///     Gets a count of all available gift cards in the store to help determine page counts for GiftCardFindAllByPage.
        /// </summary>
        /// <returns>Long</returns>
        public ApiResponse<long> GiftCardCountOfAll()
        {
            var result = new ApiResponse<long>();
            result = RestHelper.GetRequest<ApiResponse<long>>(fullApiUri + "giftcards/?key=" + Enc(key) + "&countonly=1");
            return result;
        }

        #endregion

        #region Implementation

        private ApiResponse<T> Get<T>(string method, string id, params string[] parameters)
        {
            var url = GetBaseUrl(method, id);

            foreach (var p in parameters)
            {
                url.Append("&");
                url.Append(p);
            }

            return RestHelper.GetRequest<ApiResponse<T>>(fullApiUri + url);
        }

        private ApiResponse<T> Post<T>(string method, string id, T item, params string[] parameters)
        {
            var url = GetBaseUrl(method, id);

            foreach (var p in parameters)
            {
                url.Append("&");
                url.Append(p);
            }

            return RestHelper.PostRequest<ApiResponse<T>>(fullApiUri + url, Json.ObjectToJson(item));
        }

        private ApiResponse<bool> Delete<T>(string method, string id)
        {
            var url = GetBaseUrl(method, id);
            return RestHelper.DeleteRequest<ApiResponse<bool>>(fullApiUri + url, string.Empty);
        }

        private StringBuilder GetBaseUrl(string method, string id)
        {
            var url = new StringBuilder(method + "/");

            if (!string.IsNullOrEmpty(id))
            {
                url.Append(Enc(id));
            }

            url.Append("?key=" + Enc(key));

            return url;
        }

        #endregion
    }
}
