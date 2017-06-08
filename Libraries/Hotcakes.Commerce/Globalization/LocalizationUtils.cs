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

using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Payment;
using Telerik.Web.UI;

namespace Hotcakes.Commerce.Globalization
{
    public static class LocalizationUtils
    {
        #region Global

        private const string GlobalResourceFile = "DesktopModules/Hotcakes/Core/App_LocalResources/Global.resx";

        private static ILocalizationHelper _globalLocalization;

        private static ILocalizationHelper GlobalLocalization
        {
            get
            {
                if (_globalLocalization == null)
                    _globalLocalization = Factory.Instance.CreateLocalizationHelper(GlobalResourceFile);
                return _globalLocalization;
            }
        }

        public static string GetGlobalResource(string key)
        {
            return GlobalLocalization.GetString(key);
        }

        #endregion

        #region OrderShippingStatuses

        private const string OrderShippingStatusesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/OrderShippingStatuses.resx";

        private static ILocalizationHelper _orderShippingStatusesLocalization;

        private static ILocalizationHelper OrderShippingStatusesLocalization
        {
            get
            {
                if (_orderShippingStatusesLocalization == null)
                    _orderShippingStatusesLocalization =
                        Factory.Instance.CreateLocalizationHelper(OrderShippingStatusesResourceFile);
                return _orderShippingStatusesLocalization;
            }
        }

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

        private static ILocalizationHelper OrderPaymentStatusesLocalization
        {
            get
            {
                if (_orderPaymentStatusesLocalization == null)
                    _orderPaymentStatusesLocalization =
                        Factory.Instance.CreateLocalizationHelper(OrderPaymentStatusesResourceFile);
                return _orderPaymentStatusesLocalization;
            }
        }

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

        private static ILocalizationHelper OrderStatusesLocalization
        {
            get
            {
                if (_orderStatusesLocalization == null)
                    _orderStatusesLocalization = Factory.Instance.CreateLocalizationHelper(OrderStatusesResourceFile);
                return _orderStatusesLocalization;
            }
        }

        public static string GetOrderStatus(string orderStatus, string culture = null)
        {
            return OrderStatusesLocalization.GetString(orderStatus, culture);
        }

        #endregion

        #region RecurringIntervalTypes

        private const string RecurringIntervalTypesResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/RecurringIntervalTypes.resx";

        private static ILocalizationHelper _recurringIntervalTypesLocalization;

        private static ILocalizationHelper RecurringIntervalTypesLocalization
        {
            get
            {
                if (_recurringIntervalTypesLocalization == null)
                    _recurringIntervalTypesLocalization =
                        Factory.Instance.CreateLocalizationHelper(RecurringIntervalTypesResourceFile);
                return _recurringIntervalTypesLocalization;
            }
        }

        public static string GetRecurringInterval(RecurringIntervalType intervalType)
        {
            return OrderPaymentStatusesLocalization.GetFormattedString(intervalType.ToString());
        }

        public static string GetRecurringIntervalLower(RecurringIntervalType intervalType)
        {
            return OrderPaymentStatusesLocalization.GetFormattedString(intervalType.ToString()).ToLower();
        }

        #endregion

        #region SalesPeriods

        private const string SalesPeriodsResourceFile =
            "DesktopModules/Hotcakes/Core/App_LocalResources/SalesPeriods.resx";

        private static ILocalizationHelper _salesPeriodsLocalization;

        private static ILocalizationHelper SalesPeriodsLocalization
        {
            get
            {
                if (_salesPeriodsLocalization == null)
                    _salesPeriodsLocalization = Factory.Instance.CreateLocalizationHelper(SalesPeriodsResourceFile);
                return _salesPeriodsLocalization;
            }
        }

        public static string GetSalesPeriod(SalesPeriod period)
        {
            return SalesPeriodsLocalization.GetFormattedString(period.ToString());
        }

        public static string GetSalesPeriodLower(SalesPeriod period)
        {
            return SalesPeriodsLocalization.GetFormattedString(period.ToString()).ToLower();
        }

        #endregion

        #region ActionTypes

        private const string ActionTypesResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/ActionTypes.resx";

        private static ILocalizationHelper _actionTypesLocalization;

        private static ILocalizationHelper ActionTypesLocalization
        {
            get
            {
                if (_actionTypesLocalization == null)
                    _actionTypesLocalization = Factory.Instance.CreateLocalizationHelper(ActionTypesResourceFile);
                return _actionTypesLocalization;
            }
        }

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

        private static ILocalizationHelper PaymentMethodsLocalization
        {
            get
            {
                if (_paymentMethodsLocalization == null)
                    _paymentMethodsLocalization = Factory.Instance.CreateLocalizationHelper(PaymentMethodsResourceFile);
                return _paymentMethodsLocalization;
            }
        }

        public static string GetPaymentMethodFriendlyName(string methodName)
        {
            return PaymentMethodsLocalization.GetString(methodName);
        }

        #endregion

        #region TaxProvides

        private static readonly string TaxProvidersResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/TaxProviders.resx";

        private static ILocalizationHelper _taxProvidersLocalization;

        private static ILocalizationHelper TaxProvidersLocalization
        {
            get
            {
                if (_taxProvidersLocalization == null)
                    _taxProvidersLocalization = Factory.Instance.CreateLocalizationHelper(TaxProvidersResourceFile);
                return _taxProvidersLocalization;
            }
        }

        public static string GetTaxProviderFriendlyName(string providerName)
        {
            return TaxProvidersLocalization.GetString(providerName);
        }

        #endregion

        #region QuickbooksExport

        private const string QuickbooksExportResourceFile =
            "DesktopModules/Hotcakes/Core/Admin/App_LocalResources/QuickbooksExport.resx";

        private static ILocalizationHelper _quickbooksExportLocalization;

        private static ILocalizationHelper QuickbooksExportLocalization
        {
            get
            {
                if (_quickbooksExportLocalization == null)
                    _quickbooksExportLocalization =
                        Factory.Instance.CreateLocalizationHelper(QuickbooksExportResourceFile);
                return _quickbooksExportLocalization;
            }
        }

        public static string GetQuickbooksExportString(string key)
        {
            return QuickbooksExportLocalization.GetString(key);
        }

        public static string GetQuickbooksExportFormattedString(string key, params object[] args)
        {
            return QuickbooksExportLocalization.GetFormattedString(key, args);
        }

        #endregion

        #region Web Forms

        public static void LocalizeDataGrid(DataGrid dataGrid, ILocalizationHelper localization)
        {
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

        public static void LocalizeGridView(GridView gridView, ILocalizationHelper localization)
        {
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

        public static void LocalizeRadGrid(RadGrid radGrid, ILocalizationHelper localization)
        {
            foreach (GridColumn column in radGrid.Columns)
            {
                var headerText = localization.GetString(column.UniqueName + ".HeaderText");
                if (!string.IsNullOrEmpty(headerText))
                    column.HeaderText = headerText;
                var footerText = localization.GetString(column.UniqueName + ".FooterText");
                if (!string.IsNullOrEmpty(footerText))
                    column.FooterText = footerText;
            }
        }

        #endregion
    }
}