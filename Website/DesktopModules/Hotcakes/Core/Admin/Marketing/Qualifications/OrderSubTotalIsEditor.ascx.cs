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

using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;
using System.Globalization;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class OrderSubTotalIsEditor : BaseQualificationControl
    {
        private OrderSubTotalIs TypedQualification
        {
            get { return Qualification as OrderSubTotalIs; }
        }

        public override void LoadQualification()
        {
            if (TypedQualification == null || OrderSubTotalIsField == null) return;
            OrderSubTotalIsField.Text = TypedQualification.Amount.ToString(CultureInfo.CurrentCulture);
        }

        public override bool SaveQualification()
        {
            if (TypedQualification == null || OrderSubTotalIsField == null) return false;

            var ototal = TypedQualification.Amount;
            var text = OrderSubTotalIsField.Text?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                decimal parsedototal;
                if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out parsedototal))
                {
                    ototal = parsedototal;
                }
                // If parsing fails, keep previous value (preserve existing behavior)
            }
            TypedQualification.Amount = ototal;

            return UpdatePromotion();
        }
    }
}