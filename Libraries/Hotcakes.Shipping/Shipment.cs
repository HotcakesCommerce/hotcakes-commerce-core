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
using Hotcakes.Web.Geography;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Describes the items and addresses involved in a shipment.
    /// </summary>
    /// <seealso cref="IShipment" />
    /// <seealso cref="SimpleAddress" />
    [Serializable]
    public class Shipment : IShipment
    {
        /// <summary>
        /// Initializes a shipment with empty item and address values.
        /// </summary>
        public Shipment()
        {
            SourceAddress = new SimpleAddress();
            DestinationAddress = new SimpleAddress();
            Items = new List<IShippable>();
        }

        /// <summary>
        /// Gets or sets the shippable items included in the shipment.
        /// </summary>
        public List<IShippable> Items { get; set; }

        /// <summary>
        /// Gets or sets the address from which the shipment originates.
        /// </summary>
        public IAddress SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the address to which the shipment is sent.
        /// </summary>
        public IAddress DestinationAddress { get; set; }

        /// <summary>
        /// Creates a shipment containing cloned source and destination addresses from an interface instance.
        /// </summary>
        /// <param name="source">The shipment whose addresses are copied.</param>
        /// <returns>A new shipment with cloned addresses and an empty <see cref="Items" /> collection.</returns>
        public static Shipment CloneAddressesFromInterface(IShipment source)
        {
            var result = new Shipment
            {
                DestinationAddress = SimpleAddress.CloneAddress(source.DestinationAddress),
                SourceAddress = SimpleAddress.CloneAddress(source.SourceAddress)
            };
            return result;
        }
    }
}
