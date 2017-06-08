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
    public enum FaceBookVerb
    {
        Like = 0,
        Recommend = 1
    }

    public enum FaceBookLayout
    {
        Buttons = 0,
        Standard = 1,
        Box = 2
    }

    public enum FaceBookColorScheme
    {
        Light = 0,
        Dark = 1
    }

    /// <summary>
    ///     The FaceBookLikeModel is used on the product details view, when the social sharing
    ///     features are enabled to help render the Facebook sharing correctly.
    /// </summary>
    [Serializable]
    public class FaceBookLikeModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public FaceBookLikeModel()
        {
            Verb = FaceBookVerb.Like;
            Layout = FaceBookLayout.Buttons;
            Colors = FaceBookColorScheme.Light;
            IncludeSendButton = true;
            ShowFaces = false;
            Width = 450;
            PageUrl = string.Empty;
        }

        /// <summary>
        ///     Configures whether to allow customers to Like or Recommend using this feature.
        ///     Check options at <see cref="FaceBookVerb" />
        /// </summary>
        public FaceBookVerb Verb { get; set; }

        /// <summary>
        ///     Configures how the standard Facebook sharing layout should be presented to customers.
        ///     Check options at <see cref="FaceBookLayout" />
        /// </summary>
        public FaceBookLayout Layout { get; set; }

        /// <summary>
        ///     Color for the box rendered by Facebook.
        ///     Check options at <see cref="FaceBookColorScheme" />
        /// </summary>
        public FaceBookColorScheme Colors { get; set; }

        /// <summary>
        ///     Indicates whether there should be a send button included for comments or not.
        /// </summary>
        public bool IncludeSendButton { get; set; }

        /// <summary>
        ///     Specifies if the avatars for people will be shown for those who have commented or recommended the product.
        /// </summary>
        public bool ShowFaces { get; set; }

        /// <summary>
        ///     Width of the sharing box.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     URL of the page that the customer is currently viewing.
        /// </summary>
        public string PageUrl { get; set; }
    }
}