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

using System.Collections.ObjectModel;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    public class ShippingGroup
    {
        private Address _destinationAddress = new Address();
        private readonly Collection<LineItem> _items = new Collection<LineItem>();
        private string _shipId = string.Empty;
        private ShippingMode _ShippingMode = ShippingMode.None;
        private Address _sourceAddress = new Address();

        public ShippingGroup()
        {
            Oversize = false;
            ShipSeperately = false;
            Weight = 0;
            Height = 0;
            Width = 0;
            Length = 0;
        }

        public Collection<LineItem> Items
        {
            get { return _items; }
        }

        public Address SourceAddress
        {
            get { return _sourceAddress; }
            set { _sourceAddress = value; }
        }

        public Address DestinationAddress
        {
            get { return _destinationAddress; }
            set { _destinationAddress = value; }
        }

        public ShippingMode ShippingMode
        {
            get { return _ShippingMode; }
            set { _ShippingMode = value; }
        }

        public string ShipId
        {
            get { return _shipId; }
            set { _shipId = value; }
        }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public bool ShipSeperately { get; set; }

        public bool Oversize { get; set; }

        public static DimensionCalculator DimensionCalculator
        {
            get { return new DefaultDimensionCalculator(); }
        }

        public virtual void GenerateDimensions()
        {
            if (DimensionCalculator != null)
            {
                DimensionCalculator.GenerateDimensions(this);
            }
        }

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