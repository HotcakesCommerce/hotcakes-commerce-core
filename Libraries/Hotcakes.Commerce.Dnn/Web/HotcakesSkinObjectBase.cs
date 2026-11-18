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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;
using Hotcakes.Commerce.Dnn.Mvc;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Web
{
    /// <summary>
    /// Provides common DNN skin-object lifecycle behavior for rendering Hotcakes MVC views.
    /// </summary>
    [Serializable]
    public class HotcakesSkinObjectBase : SkinObjectBase
    {
        /// <summary>
        /// Gets or sets the MVC rendering engine used to render this skin object.
        /// </summary>
        public MvcRenderingEngine MvcRenderingEngine { get; set; }

        /// <summary>
        /// Gets the current <see cref="HotcakesApplication"/> instance.
        /// </summary>
        protected HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        /// <summary>
        /// Initializes the skin object and updates the request content culture.
        /// </summary>
        /// <param name="e">The event data for the initialization event.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }

        /// <summary>
        /// Loads the skin object and renders its MVC view.
        /// </summary>
        /// <param name="e">The event data for the load event.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (HccApp.CurrentStore == null)
                    throw new StoreNotFoundException();

                MvcRenderingEngine = new MvcRenderingEngine(null);

                var view = RenderView();

                var panel = new Panel();
                var literal = new LiteralControl(view);

                panel.CssClass = "hcMvcView hcSkinObject";
                literal.EnableViewState = false;

                panel.Controls.Add(literal);
                Controls.Add(panel);
            }
            catch (StoreNotFoundException)
            {
                var control = LoadControl("~/DesktopModules/Hotcakes/Core/Controls/StoreNotFound.ascx");

                Controls.Clear();
                Controls.Add(control);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// Renders the MVC view for the skin object.
        /// </summary>
        /// <returns>The rendered view markup, or <see langword="null"/> when no view is supplied.</returns>
        protected virtual string RenderView()
        {
            return null;
        }
    }
}
