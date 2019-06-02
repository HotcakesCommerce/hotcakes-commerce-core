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
using System.Linq;
using System.Reflection;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Payment
{
    public class GiftCardGateways
    {
        static GiftCardGateways()
        {
            if (Gateways == null)
            {
                Gateways = new List<GiftCardGateway>();

                var type = typeof (GiftCardGateway);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var gatewayTypes = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    var assemblyTypes = GetLoadableTypes(assembly).Where(t => type.IsAssignableFrom(t) && !t.IsAbstract);
                    gatewayTypes.AddRange(assemblyTypes);
                }

                foreach (var gatewayType in gatewayTypes)
                {
                    Gateways.Add((GiftCardGateway) Activator.CreateInstance(gatewayType));
                }
                Gateways = Gateways.OrderBy(g => g.Name).ToList();
            }
        }

        private static List<GiftCardGateway> Gateways { get; set; }

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

        public static List<GiftCardGateway> FindAll()
        {
            return Gateways;
        }

        public static GiftCardGateway Find(string id)
        {
            return Gateways.FirstOrDefault(m => m.Id == id);
        }
    }
}