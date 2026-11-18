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

namespace Hotcakes.Commerce.Payment
{
    /// <summary>
    /// Discovers built-in and extension <see cref="PaymentMethod"/> types and filters them for store checkout.
    /// The registry is reflection-based and initialized once; method identifiers are stable serialized values.
    /// </summary>
    public class PaymentMethods
    {
        /// <summary>Stable identifier for the built-in credit-card method.</summary>
        public const string CreditCardId = "4A807645-4B9D-43f1-BC07-9F233B4E713C";
        /// <summary>Stable identifier for the built-in telephone method.</summary>
        public const string TelephoneId = "9FD35C50-CDCB-42ac-9549-14119BECBD0C";
        /// <summary>Stable identifier for the built-in check method.</summary>
        public const string CheckId = "494A61C8-D7E7-457f-B293-4838EF010C32";
        /// <summary>Stable identifier for the built-in purchase-order method.</summary>
        public const string PurchaseOrderId = "26C948F3-22EF-4bcb-9AE9-DEB9839BF4A7";
        /// <summary>Stable identifier for the built-in company-account method.</summary>
        public const string CompanyAccountId = "43AE5D2D-A62B-4EB3-BAAF-176EB509C9B5";
        /// <summary>Stable identifier for the built-in cash-on-delivery method.</summary>
        public const string CashOnDeliveryId = "EE171EFD-9E4A-4eda-AD70-4CB99F28E06C";

        /// <summary>Stable identifier for the built-in PayPal Express method.</summary>
        public const string PaypalExpressId = "33eeba60-e5b7-4864-9b57-3f8d614f8301";
        /// <summary>Stable identifier for the built-in Moneris method.</summary>
        public const string MonerisId = "900823BF-136A-4DF7-BBCC-ADE04E43F797";
        /// <summary>Stable identifier for the built-in Ogone method.</summary>
        public const string OgoneId = "F2E3D35A-B6F1-4CFE-AA40-67CDC4DBE46E";

        static PaymentMethods()
        {
            if (Methods == null)
            {
                Methods = new List<PaymentMethod>();

                var type = typeof (PaymentMethod);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var methodTypes = new List<Type>();

                foreach (var assembly in assemblies)
                {
                    var assemblyTypes = GetLoadableTypes(assembly).Where(t => type.IsAssignableFrom(t) && !t.IsAbstract);
                    methodTypes.AddRange(assemblyTypes);
                }

                foreach (var methodType in methodTypes)
                {
                    Methods.Add((PaymentMethod) Activator.CreateInstance(methodType));
                }
                Methods = Methods.OrderBy(g => g.SortIndex).ToList();
            }
        }

        private static List<PaymentMethod> Methods { get; set; }

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

        /// <summary>
        /// Gets every discovered payment method, including methods not enabled for the current store.
        /// </summary>
        /// <returns>The cached methods ordered by <see cref="PaymentMethod.SortIndex"/>.</returns>
        public static List<PaymentMethod> AvailableMethods()
        {
            return Methods;
        }

        /// <summary>
        /// Returns methods whose identifiers are enabled in the store settings, optionally retaining only recurring-capable methods.
        /// </summary>
        /// <param name="currentStore">The store whose enabled-method list is applied.</param>
        /// <param name="showRecurringPaymentMethods">When <see langword="true"/>, excludes methods without recurring-billing support.</param>
        /// <returns>A new filtered list; a missing enabled list produces an empty list.</returns>
        public static List<PaymentMethod> EnabledMethods(Store currentStore, bool showRecurringPaymentMethods = false)
        {
            var result = new List<PaymentMethod>();

            var enabledList = currentStore.Settings.PaymentMethodsEnabled;
            if (enabledList != null)
            {
                foreach (var method in Methods.Where(method => enabledList.Contains(method.MethodId)))
                {
                    if (!showRecurringPaymentMethods || method.SupportRecurringBilling)
                    {
                        result.Add(method);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Finds a discovered payment method by identifier using case-insensitive comparison.
        /// </summary>
        /// <param name="methodId">The stable method identifier.</param>
        /// <returns>The matching method, or <see langword="null"/> when no method matches.</returns>
        public static PaymentMethod Find(string methodId)
        {
            return
                Methods.FirstOrDefault(dpm => string.Equals(dpm.MethodId, methodId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
