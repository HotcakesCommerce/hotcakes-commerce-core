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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;
using Hotcakes.Commerce.Dnn.Mvc;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Web
{
    [Serializable]
    public class HotcakesSkinObjectBase : SkinObjectBase
    {
        public MvcRenderingEngine MvcRenderingEngine { get; set; }

        protected HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }

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

        protected virtual string RenderView()
        {
            return null;
        }
    }
}