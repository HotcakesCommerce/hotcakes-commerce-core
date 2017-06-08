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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Modules.Core.Models.Validators;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The CreditCardViewModel is used primarily by the PaymentViewModel class to
    ///     update billing information when payment issues occur during checkout.
    /// </summary>
    [Serializable]
    [Bind(Exclude = "AcceptedCardTypes")]
    public class CreditCardViewModel
    {
        /// <summary>
        ///     List of enabled card types to be shown on the payment area of the
        ///     checkout view.
        /// </summary>
        public List<CardType> AcceptedCardTypes { get; set; }

        /// <summary>
        ///     The number on the front of the card.
        /// </summary>
        [Required(ErrorMessageResourceName = "CardNumber_Required",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        [CreditCardNumber(ErrorMessageResourceName = "CardNumber_Invalid",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        public string CardNumber { get; set; }

        /// <summary>
        ///     Month when the card expires.
        /// </summary>
        [Required(ErrorMessageResourceName = "ExpirationMonth_Required",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        public int ExpirationMonth { get; set; }

        /// <summary>
        ///     Year when the card expires.
        /// </summary>
        [Required(ErrorMessageResourceName = "ExpirationYear_Required",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        public int ExpirationYear { get; set; }

        /// <summary>
        ///     Security code, normally found on the back of the card.
        /// </summary>
        [Required(ErrorMessageResourceName = "SecurityCode_Required",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        public string SecurityCode { get; set; }

        /// <summary>
        ///     Name of the person authorized to use the card, as it appears on the card.
        /// </summary>
        [Required(ErrorMessageResourceName = "CardHolderName_Required",
            ErrorMessageResourceType = typeof (CreditCardViewModelLocalization))]
        public string CardHolderName { get; set; }

        /// <summary>
        ///     Gives the list of the month number shown on the view to select the
        ///     expiring month of the card
        /// </summary>
        /// <returns></returns>
        public SelectList GetMonthesList()
        {
            var ms = new List<object> {new {Value = (int?) null, Text = "--"}};

            for (var i = 1; i <= 12; i++)
            {
                ms.Add(new {Value = i, Text = i.ToString()});
            }

            return new SelectList(ms, "Value", "Text");
        }

        /// <summary>
        ///     Gives List of the years on the view to select the
        ///     expiring year of the month
        /// </summary>
        /// <returns></returns>
        public SelectList GetYearsList()
        {
            var baseYear = DateTime.Now.Year;
            var ys = new List<object> {new {Value = (int?) null, Text = "--"}};

            for (var i = 1; i <= 12; i++)
            {
                ys.Add(new {Value = baseYear + i, Text = (baseYear + i).ToString()});
            }

            return new SelectList(ys, "Value", "Text");
        }
    }

    /// <summary>
    ///     Localized messages for the model.
    /// </summary>
    public class CreditCardViewModelLocalization : ViewModelLocalization
    {
        /// <summary>
        ///     Initialize the localization file for this model.
        ///     Load all translation in memory
        /// </summary>
        static CreditCardViewModelLocalization()
        {
            Init<CreditCardViewModelLocalization>("CreditCardViewModel");
        }

        /// <summary>
        ///     Card number required validation message
        /// </summary>
        public static string CardNumber_Required { get; set; }

        /// <summary>
        ///     Card number invalid validation message
        /// </summary>
        public static string CardNumber_Invalid { get; set; }

        /// <summary>
        ///     Expiration month required validation message
        /// </summary>
        public static string ExpirationMonth_Required { get; set; }

        /// <summary>
        ///     Expiration year required validaiton message
        /// </summary>
        public static string ExpirationYear_Required { get; set; }

        /// <summary>
        ///     Security code required validation message
        /// </summary>
        public static string SecurityCode_Required { get; set; }

        /// <summary>
        ///     Card owner name required validation message
        /// </summary>
        public static string CardHolderName_Required { get; set; }
    }
}