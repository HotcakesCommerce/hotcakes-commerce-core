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

namespace Hotcakes.Modules.Core.Models
{
    public enum PinterestSize
    {
        Small = 0,
        Large = 3
    }

    public enum PinterestShape
    {
        Rectangular = 0,
        Circular = 1
    }

    public enum PinterestColor
    {
        Red = 0,
        Gray = 1,
        White = 2
    }

    public enum PinterestLanguage
    {
        English = 0,
        Japanese = 1
    }

    /// <summary>
    ///     Social media Pinterest plugin for the product
    ///     detail page. See  <b> http://pinterest.com</b>
    /// </summary>
    [Serializable]
    public class PinterestViewModel
    {
        public PinterestViewModel()
        {
            Size = PinterestSize.Small;
            Shape = PinterestShape.Rectangular;
            Color = PinterestColor.Red;
            Language = PinterestLanguage.English;
        }

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public PinterestSize Size { get; set; }

        /// <summary>
        ///     Gets or sets the shape.
        /// </summary>
        /// <value>
        ///     The shape.
        /// </value>
        public PinterestShape Shape { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        /// <remarks>
        ///     Should not be used with PinterestShape.Circular.
        /// </remarks>
        public PinterestColor Color { get; set; }

        /// <summary>
        ///     Gets or sets the language.
        /// </summary>
        /// <value>
        ///     The language.
        /// </value>
        /// <remarks>
        ///     Can only be used with PinterestShape.Rectangular.
        /// </remarks>
        public PinterestLanguage Language { get; set; }

        /// <summary>
        ///     Evaluates the model to determine if the property assignment combination is valid.
        /// </summary>
        /// <returns>If value, true is returned.</returns>
        /// <remarks>
        ///     This is necessary because certain combinations will result in invalid HTML being generated.
        /// </remarks>
        public bool ViewModelIsValid()
        {
            #region Reference

            /*
             PLEASE NOTE:
                These are the only supported combinations of this specific Pinterest button.
                Reference: https://business.pinterest.com/en/widget-builder#do_pin_it_button


                LARGE       RECTANGULAR     RED     ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-height="28" data-pin-color="red"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_red_28.png" /></a>

                LARGE       RECTANGULAR     GRAY    ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-height="28"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_gray_28.png" /></a>

                LARGE       RECTANGULAR     WHITE   ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-height="28" data-pin-color="white"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_white_28.png" /></a>

                LARGE       RECTANGULAR     RED     JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja" data-pin-color="red" data-pin-height="28"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_red_28.png" /></a>

                LARGE       RECTANGULAR     GRAY    JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja" data-pin-height="28"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_gray_28.png" /></a>

                LARGE       RECTANGULAR     WHITE   JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja" data-pin-color="white" data-pin-height="28"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_white_28.png" /></a>


                SMALL       RECTANGULAR     RED     ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-color="red"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_red_20.png" /></a>

                SMALL       RECTANGULAR     GRAY   ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_gray_20.png" /></a>

                SMALL       RECTANGULAR     WHITE   ENGLISH
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-color="white"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_rect_white_20.png" /></a>

                SMALL       RECTANGULAR     RED     JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja" data-pin-color="red"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_red_20.png" /></a>

                SMALL       RECTANGULAR     GRAY   JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_gray_20.png" /></a>

                SMALL       RECTANGULAR     WHITE   JAPANESE
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-lang="ja" data-pin-color="white"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_ja_rect_white_20.png" /></a>


                LARGE       CIRCULAR
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-shape="round" data-pin-height="32"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_round_red_32.png" /></a>

                SMALL       CIRCULAR
                <a href="//www.pinterest.com/pin/create/button/" data-pin-do="buttonBookmark" data-pin-shape="round"><img src="//assets.pinterest.com/images/pidgets/pinit_fg_en_round_red_16.png" /></a>
             */

            #endregion

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.Red &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.Gray &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.White &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.Red &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.Gray &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Rectangular && Color == PinterestColor.White &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.Red &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.Gray &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.White &&
                Language == PinterestLanguage.English)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.Red &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.Gray &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Rectangular && Color == PinterestColor.White &&
                Language == PinterestLanguage.Japanese)
            {
                return true;
            }

            if (Size == PinterestSize.Large && Shape == PinterestShape.Circular)
            {
                return true;
            }

            if (Size == PinterestSize.Small && Shape == PinterestShape.Circular)
            {
                return true;
            }

            return false;
        }
    }
}