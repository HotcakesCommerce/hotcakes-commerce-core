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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Payment;
using Hotcakes.Payment.Gateways;
using Hotcakes.Payment.RecurringGateways;

namespace Hotcakes.Commerce.Payment
{
    /// <summary>
    /// Discovers payment and recurring-payment gateways from loaded assemblies and resolves store selections.
    /// Gateway instances are created by reflection, cached, and ordered by name.
    /// </summary>
    public class PaymentGateways
    {
        #region Constructor

        static PaymentGateways()
        {
            if (Gateways == null)
            {
                Gateways = LoadTypesToList<PaymentGateway>().OrderBy(g => g.Name).ToList();
            }
            if (RecurringGateways == null)
            {
                RecurringGateways = LoadTypesToList<RecurringPaymentGateway>().OrderBy(g => g.Name).ToList();
            }
        }

        #endregion

        #region Properties

        private static List<PaymentGateway> Gateways { get; set; }
        private static List<RecurringPaymentGateway> RecurringGateways { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets all discovered payment gateways.
        /// </summary>
        /// <returns>The cached payment gateways ordered by name.</returns>
        public static List<PaymentGateway> FindAll()
        {
            return Gateways;
        }

        /// <summary>
        /// Gets all discovered recurring-payment gateways.
        /// </summary>
        /// <returns>The cached recurring gateways ordered by name.</returns>
        public static List<RecurringPaymentGateway> FindAllRecurringBilling()
        {
            return RecurringGateways;
        }

        /// <summary>
        /// Finds a payment gateway, falling back to the recurring-gateway registry when needed.
        /// </summary>
        /// <param name="id">The gateway identifier.</param>
        /// <returns>The matching gateway from either registry, or <see langword="null"/>.</returns>
        public static GatewayBase Find(string id)
        {
            GatewayBase gw = Gateways.FirstOrDefault(m => m.Id == id);

            if (gw == null)
            {
                gw = RecurringGateways.FirstOrDefault(m => m.Id == id);
            }

            return gw;
        }

        /// <summary>
        /// Resolves the store's configured credit-card gateway, falling back to <see cref="TestGateway"/>, then merges its stored settings.
        /// </summary>
        /// <param name="store">The store whose payment settings select and configure the gateway.</param>
        /// <returns>The selected or fallback payment gateway with merged settings.</returns>
        public static PaymentGateway CurrentPaymentProcessor(Store store)
        {
            PaymentGateway gateway = null;
            var sett = store.Settings;

            gateway = Find(sett.PaymentCreditCardGateway) as PaymentGateway;

            if (gateway == null)
            {
                gateway = new TestGateway();
            }

            var mSett = sett.PaymentSettingsGet(sett.PaymentCreditCardGateway);
            gateway.BaseSettings.Merge(mSett);

            return gateway;
        }

        /// <summary>
        /// Resolves the store's configured recurring gateway, falling back to <see cref="TestRecurringGateway"/>, then merges its stored settings.
        /// </summary>
        /// <param name="store">The store whose recurring-payment settings select and configure the gateway.</param>
        /// <returns>The selected or fallback recurring gateway with merged settings.</returns>
        public static RecurringPaymentGateway CurrentRecurringBillingProcessor(Store store)
        {
            RecurringPaymentGateway gateway = null;
            var sett = store.Settings;

            gateway = Find(sett.PaymentReccuringGateway) as RecurringPaymentGateway;

            if (gateway == null)
            {
                gateway = new TestRecurringGateway();
            }

            var mSett = sett.PaymentSettingsGet(sett.PaymentReccuringGateway);
            gateway.BaseSettings.Merge(mSett);

            return gateway;
        }

        #endregion

        #region Implementation

        private static IList<T> LoadTypesToList<T>() where T : class
        {
            var objects = new List<T>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var type = typeof (T);
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(GetLoadableTypes(assembly).Where(t => type.IsAssignableFrom(t) && !t.IsAbstract));
            }

            foreach (var t in types)
            {
                objects.Add((T) Activator.CreateInstance(t));
            }
            return objects;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        #endregion
    }
}
