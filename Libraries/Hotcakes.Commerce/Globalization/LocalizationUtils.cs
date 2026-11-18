#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    /// Provides convenience lookups for shared commerce resources and Web Forms grid localization.
    /// </summary>
    /// <remarks>
    /// Resource helpers use the fixed resource paths declared in this type and retain their
    /// localization helpers for reuse. Status lookups preserve their explicit fallback to the
    /// <c>Unknown</c> resource entry. Grid methods mutate the supplied column definitions in place.
    /// </remarks>
    public static class LocalizationUtils
    {
        #region Global

        private const string GlobalResourceFile = "DesktopModules/Hotcakes/Core/App_LocalResources/Global.resx";

        private static ILocalizationHelper _globalLocalization;

        private static ILocalizationHelper GlobalLocalization => _globalLocalization ??= Factory.Instance.CreateLocalizationHelper(GlobalResourceFile);

        /// <summary>
        /// Gets a value from the fixed global resource file.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The value returned by the global localization helper.</returns>
        public static string GetGlobalResource(string key)
        {
            return GlobalLocalization.GetString(key);
        }

        #endregion

        #region OrderShippingStatuses

        private const string OrderShippingStatusesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/OrderShippingStatuses.resx";

        private static ILocalizationHelper _orderShippingStatusesLocalization;

        private static ILocalizationHelper OrderShippingStatusesLocalization => _orderShippingStatusesLocalization ??= Factory.Instance.CreateLocalizationHelper(OrderShippingStatusesResourceFile);

        /// <summary>
        /// Gets the localized display value for an order shipping status.
        /// </summary>
        /// <param name="shippingStatus">The shipping status whose resource key is used.</param>
        /// <param name="culture">The culture passed to the status resource lookup and fallback lookup.</param>
        /// <returns>The status value, or the localized <see cref="OrderShippingStatus.Unknown"/> value when the first lookup is empty.</returns>
        /// <remarks>
        /// The first call uses <see cref="ILocalizationHelper.GetFormattedString(string, object[])"/>,
        /// so the optional culture value is passed as a format argument rather than selected through
        /// the culture overload. This existing behavior is intentional documentation of the current contract.
        /// </remarks>
        public static string GetOrderShippingStatus(OrderShippingStatus shippingStatus, string culture = null)
        {
            var value = OrderShippingStatusesLocalization.GetFormattedString(shippingStatus.ToString(), culture);
            if (string.IsNullOrEmpty(value))
                value = OrderShippingStatusesLocalization.GetString(OrderShippingStatus.Unknown.ToString(), culture);
            return value;
        }

        #endregion

        #region OrderPaymentStatuses

        private const string OrderPaymentStatusesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/OrderPaymentStatuses.resx";

        private static ILocalizationHelper _orderPaymentStatusesLocalization;

        private static ILocalizationHelper OrderPaymentStatusesLocalization => _orderPaymentStatusesLocalization ??= Factory.Instance.CreateLocalizationHelper(OrderPaymentStatusesResourceFile);

        /// <summary>
        /// Gets the localized display value for an order payment status.
        /// </summary>
        /// <param name="paymentStatus">The payment status whose resource key is used.</param>
        /// <param name="culture">The culture passed to the status resource lookup and fallback lookup.</param>
        /// <returns>The status value, or the localized <see cref="OrderPaymentStatus.Unknown"/> value when the first lookup is empty.</returns>
        /// <remarks>
        /// The first call uses <see cref="ILocalizationHelper.GetFormattedString(string, object[])"/>,
        /// so the optional culture value is passed as a format argument rather than selected through
        /// the culture overload. This existing behavior is documented rather than changed.
        /// </remarks>
        public static string GetOrderPaymentStatus(OrderPaymentStatus paymentStatus, string culture = null)
        {
            var value = OrderPaymentStatusesLocalization.GetFormattedString(paymentStatus.ToString(), culture);
            if (string.IsNullOrEmpty(value))
                value = OrderPaymentStatusesLocalization.GetString(OrderPaymentStatus.Unknown.ToString(), culture);
            return value;
        }

        #endregion

        #region OrderStatuses

        private const string OrderStatusesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/OrderStatuses.resx";

        private static ILocalizationHelper _orderStatusesLocalization;

        private static ILocalizationHelper OrderStatusesLocalization => _orderStatusesLocalization ??= Factory.Instance.CreateLocalizationHelper(OrderStatusesResourceFile);

        /// <summary>
        /// Gets the localized value for an order status key.
        /// </summary>
        /// <param name="orderStatus">The order status resource key.</param>
        /// <param name="culture">The culture name used for the lookup.</param>
        /// <returns>The localized order status value.</returns>
        public static string GetOrderStatus(string orderStatus, string culture = null)
        {
            return OrderStatusesLocalization.GetString(orderStatus, culture);
        }

        #endregion

        #region RecurringIntervalTypes

        private const string RecurringIntervalTypesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/RecurringIntervalTypes.resx";

        private static ILocalizationHelper _recurringIntervalTypesLocalization;

        private static ILocalizationHelper RecurringIntervalTypesLocalization => _recurringIntervalTypesLocalization ??= Factory.Instance.CreateLocalizationHelper(RecurringIntervalTypesResourceFile);

        /// <summary>
        /// Gets the localized display value for a recurring interval type.
        /// </summary>
        /// <param name="intervalType">The interval type whose name is used as the resource key.</param>
        /// <returns>The formatted localized interval value.</returns>
        public static string GetRecurringInterval(RecurringIntervalType intervalType)
        {
            return RecurringIntervalTypesLocalization.GetFormattedString(intervalType.ToString());
        }

        /// <summary>
        /// Gets the localized recurring interval value converted to lower case using the current culture.</summary>
        /// <param name="intervalType">The interval type whose name is used as the resource key.</param>
        /// <returns>The lower-case formatted localized interval value.</returns>
        public static string GetRecurringIntervalLower(RecurringIntervalType intervalType)
        {
            return RecurringIntervalTypesLocalization.GetFormattedString(intervalType.ToString()).ToLower();
        }

        #endregion

        #region SalesPeriods

        private const string SalesPeriodsResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/SalesPeriods.resx";

        private static ILocalizationHelper _salesPeriodsLocalization;

        private static ILocalizationHelper SalesPeriodsLocalization => _salesPeriodsLocalization ??= Factory.Instance.CreateLocalizationHelper(SalesPeriodsResourceFile);

        /// <summary>
        /// Gets the localized display value for a sales period.
        /// </summary>
        /// <param name="period">The sales period whose name is used as the resource key.</param>
        /// <returns>The formatted localized sales-period value.</returns>
        public static string GetSalesPeriod(SalesPeriod period)
        {
            return SalesPeriodsLocalization.GetFormattedString(period.ToString());
        }

        /// <summary>
        /// Gets the localized sales-period value converted to lower case using the current culture.
        /// </summary>
        /// <param name="period">The sales period whose name is used as the resource key.</param>
        /// <returns>The lower-case formatted localized sales-period value.</returns>
        public static string GetSalesPeriodLower(SalesPeriod period)
        {
            return SalesPeriodsLocalization.GetFormattedString(period.ToString()).ToLower();
        }

        #endregion

        #region ActionTypes

        private const string ActionTypesResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/ActionTypes.resx";

        private static ILocalizationHelper _actionTypesLocalization;

        private static ILocalizationHelper ActionTypesLocalization => _actionTypesLocalization ??= Factory.Instance.CreateLocalizationHelper(ActionTypesResourceFile);

        /// <summary>
        /// Gets the localized display value for an action type and method name.
        /// </summary>
        /// <param name="actionType">The action type whose name is used as the resource key.</param>
        /// <param name="methodName">The method name supplied as the format argument.</param>
        /// <returns>The formatted action value, or the localized <see cref="ActionType.Unknown"/> value when it is empty.</returns>
        public static string GetActionType(ActionType actionType, string methodName)
        {
            var value = ActionTypesLocalization.GetFormattedString(actionType.ToString(), methodName);
            if (string.IsNullOrEmpty(value))
                value = ActionTypesLocalization.GetString(ActionType.Unknown.ToString());
            return value;
        }

        #endregion

        #region PaymentMethods

        private const string PaymentMethodsResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/PaymentMethods.resx";

        private static ILocalizationHelper _paymentMethodsLocalization;

        private static ILocalizationHelper PaymentMethodsLocalization => _paymentMethodsLocalization ??= Factory.Instance.CreateLocalizationHelper(PaymentMethodsResourceFile);

        /// <summary>
        /// Gets the friendly name for a payment method from the fixed payment-method resource file.
        /// </summary>
        /// <param name="methodName">The payment method resource key.</param>
        /// <returns>The localized friendly name.</returns>
        public static string GetPaymentMethodFriendlyName(string methodName)
        {
            return PaymentMethodsLocalization.GetString(methodName);
        }

        #endregion

        #region TaxProvides

        private static readonly string TaxProvidersResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/TaxProviders.resx";

        private static ILocalizationHelper _taxProvidersLocalization;

        private static ILocalizationHelper TaxProvidersLocalization => _taxProvidersLocalization ??= Factory.Instance.CreateLocalizationHelper(TaxProvidersResourceFile);

        /// <summary>
        /// Gets the friendly name for a tax provider from the fixed tax-provider resource file.
        /// </summary>
        /// <param name="providerName">The tax provider resource key.</param>
        /// <returns>The localized friendly name.</returns>
        public static string GetTaxProviderFriendlyName(string providerName)
        {
            return TaxProvidersLocalization.GetString(providerName);
        }

        #endregion

        #region QuickbooksExport

        private const string QuickbooksExportResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/QuickbooksExport.resx";

        private static ILocalizationHelper _quickbooksExportLocalization;

        private static ILocalizationHelper QuickbooksExportLocalization => _quickbooksExportLocalization ??= Factory.Instance.CreateLocalizationHelper(QuickbooksExportResourceFile);

        /// <summary>
        /// Gets a QuickBooks export resource value.
        /// </summary>
        /// <param name="key">The QuickBooks export resource key.</param>
        /// <returns>The localized resource value.</returns>
        public static string GetQuickbooksExportString(string key)
        {
            return QuickbooksExportLocalization.GetString(key);
        }

        /// <summary>
        /// Gets and formats a QuickBooks export resource value.
        /// </summary>
        /// <param name="key">The QuickBooks export resource key.</param>
        /// <param name="args">The format arguments passed to the localization helper.</param>
        /// <returns>The formatted localized resource value.</returns>
        public static string GetQuickbooksExportFormattedString(string key, params object[] args)
        {
            return QuickbooksExportLocalization.GetFormattedString(key, args);
        }

        #endregion

        #region Web Forms

        /// <summary>
        /// Localizes the header and footer text of each column in a Web Forms data grid in place.
        /// </summary>
        /// <param name="dataGrid">The data grid whose columns are mutated; null is ignored.</param>
        /// <param name="localization">The helper used to look up column text; null is ignored.</param>
        /// <remarks>
        /// Each lookup appends <c>.HeaderText</c> or <c>.FooterText</c> to the column's current text.
        /// A column is changed only when the lookup returns a non-empty value; otherwise its existing
        /// text is preserved.
        /// </remarks>
        public static void LocalizeDataGrid(DataGrid dataGrid, ILocalizationHelper localization)
        {
            if (dataGrid == null || localization == null) return;

            foreach (DataGridColumn column in dataGrid.Columns)
            {
                var headerText = localization.GetString(column.HeaderText + ".HeaderText");
                if (!string.IsNullOrEmpty(headerText))
                    column.HeaderText = headerText;
                var footerText = localization.GetString(column.FooterText + ".FooterText");
                if (!string.IsNullOrEmpty(footerText))
                    column.FooterText = footerText;
            }
        }

        /// <summary>
        /// Localizes the header and footer text of each column in a Web Forms grid view in place.
        /// </summary>
        /// <param name="gridView">The grid view whose columns are mutated; null is ignored.</param>
        /// <param name="localization">The helper used to look up column text; null is ignored.</param>
        /// <remarks>
        /// Each lookup appends <c>.HeaderText</c> or <c>.FooterText</c> to the column's current text.
        /// A column is changed only when the lookup returns a non-empty value; otherwise its existing
        /// text is preserved.
        /// </remarks>
        public static void LocalizeGridView(GridView gridView, ILocalizationHelper localization)
        {
            if (gridView == null || localization == null) return;

            foreach (DataControlField column in gridView.Columns)
            {
                var headerText = localization.GetString(column.HeaderText + ".HeaderText");
                if (!string.IsNullOrEmpty(headerText))
                    column.HeaderText = headerText;
                var footerText = localization.GetString(column.FooterText + ".FooterText");
                if (!string.IsNullOrEmpty(footerText))
                    column.FooterText = footerText;
            }
        }

        #endregion
    }
}