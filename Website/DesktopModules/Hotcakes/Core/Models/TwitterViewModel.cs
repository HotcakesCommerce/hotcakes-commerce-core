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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Box type information
    /// </summary>
    public enum TwitterCountBoxPosition
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }

    /// <summary>
    ///     Related account information
    /// </summary>
    [Serializable]
    public class TwitterRelatedAccount
    {
        public TwitterRelatedAccount()
        {
            TwitterHandle = string.Empty;
            Description = string.Empty;
        }

        public string TwitterHandle { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    ///     Used to build the twitter URL
    /// </summary>
    [Serializable]
    public class TwitterViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public TwitterViewModel()
        {
            LinkUrl = string.Empty;
            DisplayUrl = string.Empty;
            DefaultText = string.Empty;
            ViaTwitterName = string.Empty;
            RelatedAccounts = new List<TwitterRelatedAccount>();
            CountPosition = TwitterCountBoxPosition.Horizontal;
            Language = "en";
        }

        /// <summary>
        ///     Current page URL which user want to twit. Used to set parameter "url"
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        ///     Count information URL. Used to set parameter "counturl"
        /// </summary>
        public string DisplayUrl { get; set; }

        /// <summary>
        ///     Text needs to be passed for twit to Twitter. Used to set parameter "text"
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        ///     URL set for viatwitter name. Used to set parameter "via"
        /// </summary>
        /// <remarks>
        ///     Generally Twitter handle is passed in this. Twitter handle configured on the twitter setting available on
        ///     hotcakes admin panel
        /// </remarks>
        public string ViaTwitterName { get; set; }

        /// <summary>
        ///     Related account information passed to the twitter. More details about related account can be find at
        ///     <see cref="TwitterRelatedAccount" />.
        ///     Used to set parameter "related" with multiple key value pair separated by ":"
        /// </summary>
        public List<TwitterRelatedAccount> RelatedAccounts { get; set; }

        /// <summary>
        ///     Position of the count box on twitter widget. Used to set parameter "count"
        /// </summary>
        public TwitterCountBoxPosition CountPosition { get; set; }

        /// <summary>
        ///     Send language information to twitter. Used to set parameter "lang"
        /// </summary>
        public string Language { get; set; }
    }
}