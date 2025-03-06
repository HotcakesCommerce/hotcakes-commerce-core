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

using System;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class XmlResult : ActionResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlResult" /> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public XmlResult(object objectToSerialize)
        {
            ObjectToSerialize = objectToSerialize;
        }

        /// <summary>
        ///     Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize { get; private set; }

        /// <summary>
        ///     Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result
        ///     stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (ObjectToSerialize != null)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ContentType = "text/xml";

                var xs = new XmlSerializer(ObjectToSerialize.GetType());
                xs.Serialize(context.HttpContext.Response.Output, ObjectToSerialize);
            }
        }
    }
}