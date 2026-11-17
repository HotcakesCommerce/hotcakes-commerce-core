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

using System.Collections.ObjectModel;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Groups line items with their shipping origin, destination, mode, package dimensions, and charge context.
    /// </summary>
    /// <remarks>The misspelled <see cref="ShipSeperately"/> name is retained for compatibility.</remarks>
    public class ShippingGroup
    {
        private Address _destinationAddress = new Address();
        private readonly Collection<LineItem> _items = new Collection<LineItem>();
        private string _shipId = string.Empty;
        private ShippingMode _ShippingMode = ShippingMode.None;
        private Address _sourceAddress = new Address();

        /// <summary>
        /// Initializes an empty shipping group with zero dimensions and no separate-shipping or oversize flag.
        /// </summary>
        public ShippingGroup()
        {
            Oversize = false;
            ShipSeperately = false;
            Weight = 0;
            Height = 0;
            Width = 0;
            Length = 0;
        }

        /// <summary>
        /// Gets the mutable collection of order line items assigned to this group.
        /// </summary>
        public Collection<LineItem> Items
        {
            get { return _items; }
        }

        /// <summary>Gets or sets the address from which this group ships.</summary>
        public Address SourceAddress
        {
            get { return _sourceAddress; }
            set { _sourceAddress = value; }
        }

        /// <summary>Gets or sets the destination address for this group.</summary>
        public Address DestinationAddress
        {
            get { return _destinationAddress; }
            set { _destinationAddress = value; }
        }

        /// <summary>Gets or sets the shipping source mode for this group.</summary>
        public ShippingMode ShippingMode
        {
            get { return _ShippingMode; }
            set { _ShippingMode = value; }
        }

        /// <summary>Gets or sets the group identifier; cloning clears this value.</summary>
        public string ShipId
        {
            get { return _shipId; }
            set { _shipId = value; }
        }

        /// <summary>Gets or sets the package length.</summary>
        public decimal Length { get; set; }

        /// <summary>Gets or sets the package width.</summary>
        public decimal Width { get; set; }

        /// <summary>Gets or sets the package height.</summary>
        public decimal Height { get; set; }

        /// <summary>Gets or sets the total package weight.</summary>
        public decimal Weight { get; set; }

        /// <summary>Gets or sets whether the group must ship separately. The compatibility-sensitive property name is misspelled.</summary>
        public bool ShipSeperately { get; set; }

        /// <summary>Gets or sets whether the group is oversized.</summary>
        public bool Oversize { get; set; }

        /// <summary>Gets the default dimension calculator, creating a <see cref="DefaultDimensionCalculator"/> on each access.</summary>
        public static DimensionCalculator DimensionCalculator
        {
            get { return new DefaultDimensionCalculator(); }
        }

        /// <summary>Generates package dimensions using <see cref="DimensionCalculator"/>.</summary>
        public virtual void GenerateDimensions()
        {
            if (DimensionCalculator != null)
            {
                DimensionCalculator.GenerateDimensions(this);
            }
        }

        /// <summary>Creates a copy with cloned line items, copied addresses, a cleared identifier, and recalculated dimensions.</summary>
        /// <param name="CopyBvins">Whether cloned line items should retain their BVIN values.</param>
        /// <returns>A new shipping group.</returns>
        public ShippingGroup Clone(bool CopyBvins)
        {
            var result = new ShippingGroup();

            foreach (var li in _items)
            {
                result.Items.Add(li.Clone(CopyBvins));
            }
            _sourceAddress.CopyTo(result.SourceAddress);
            _destinationAddress.CopyTo(result.DestinationAddress);
            result.ShippingMode = _ShippingMode;
            result.ShipId = string.Empty;
            result.Length = Length;
            result.Width = Width;
            result.Height = Height;
            result.Weight = Weight;
            result.ShipSeperately = ShipSeperately;
            result.GenerateDimensions();

            return result;
        }

        /// <summary>Converts this group to the shipping-provider package contract.</summary>
        /// <returns>An <see cref="IShippable"/> containing dimensions, item quantity, and line value.</returns>
        public IShippable AsIShippable()
        {
            GenerateDimensions();
            var result = new Shippable();

            result.BoxHeight = Height;
            result.BoxLength = Length;
            result.BoxWeight = Weight;
            result.BoxWidth = Width;

            foreach (var li in Items)
            {
                result.QuantityOfItemsInBox += li.Quantity;
                result.BoxValue += li.LineTotal;
            }

            return result;
        }
    }
}
