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
using System.Web;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace Hotcakes.Modules
{
    public class AppStartHttpModule : IHttpModule
    {
        /// <summary>Initializes any data/resources on application start.</summary>
        /// <param name="application">The application context that instantiated and will be running this module.</param>
        public virtual void OnStart(HttpApplication application)
        {
            // put your application start code here
            MiniProfilerEF6.Initialize();
            MiniProfiler.Settings.IgnoredPaths = new string[] {};
        }

        /// <summary>Initializes any data/resources on HTTP module start.</summary>
        /// <param name="application">The application context that instantiated and will be running this module.</param>
        public virtual void OnInit(HttpApplication application)
        {
            // put your module initialization code here
            application.BeginRequest += application_BeginRequest;
            application.EndRequest += application_EndRequest;
        }

        private void application_BeginRequest(object sender, EventArgs e)
        {
            MiniProfiler.Start();
        }

        private void application_EndRequest(object sender, EventArgs e)
        {
            MiniProfiler.Stop();
        }

        #region Static privates

        private static volatile bool applicationStarted;
        private static readonly object applicationStartLock = new object();

        #endregion

        #region IHttpModule implementation

        /// <summary>
        ///     Disposes of the resources (other than memory) used by the module that implements
        ///     <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            // dispose any resources if needed
        }

        /// <summary>
        ///     Initializes the specified module.
        /// </summary>
        /// <param name="context">The application context that instantiated and will be running this module.</param>
        public void Init(HttpApplication context)
        {
            if (!applicationStarted)
            {
                lock (applicationStartLock)
                {
                    if (!applicationStarted)
                    {
                        // this will run only once per application start
                        OnStart(context);
                        applicationStarted = true;
                    }
                }
            }
            // this will run on every HttpApplication initialization in the application pool
            OnInit(context);
        }

        #endregion
    }
}