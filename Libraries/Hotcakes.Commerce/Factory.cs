#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Configuration;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce
{
    public class Factory
    {
        [ThreadStatic] private static HttpContext _httpContext;

        static Factory()
        {
            Type factoryType = null;
            var factoryTypeName = ConfigurationManager.AppSettings["HccFactory"];
            if (!string.IsNullOrEmpty(factoryTypeName))
            {
                try
                {
                    factoryType = Type.GetType(factoryTypeName);
                }
                catch
                {
                }
                ;
            }
            else
            {
                factoryType = DetectFactoryType();
            }

            if (factoryType != null)
            {
                try
                {
                    Instance = (IFactory) Activator.CreateInstance(factoryType);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Cannot create instance of type {0}", factoryType), ex);
                }
            }
            else
            {
                throw new ApplicationException(
                    "Configuration appSettings parameter 'HccFactory' is not specified and IFactory implementation was not detected.");
            }
        }

        public static IFactory Instance { get; private set; }

        public static HttpContext HttpContext
        {
            get { return HttpContext.Current ?? _httpContext; }
            set { _httpContext = value; }
        }

        private static Type DetectFactoryType()
        {
            var ifactoryType = typeof (IFactory);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var assemblyType = GetLoadableTypes(assembly)
                    .FirstOrDefault(t => ifactoryType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

                if (assemblyType != null)
                {
                    return assemblyType;
                }
            }

            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IHccUrlResolver CreateHccUrlResolver()
        {
            return Instance.CreateHccUrlResolver();
        }

        public static IHccFormRenderer CreateHccFormRenderer()
        {
            return Instance.CreateHccFormRenderer();
        }

        public static ILogger CreateEventLogger()
        {
            return Instance.CreateEventLogger();
        }

        public static ISocialService CreateSocialService(HccRequestContext context)
        {
            return Instance.CreateSocialService(context);
        }

        public static WorkflowFactory CreateWorklflowFactory(HccRequestContext context)
        {
            return Instance.CreateWorkflowFactory(context);
        }

        public static IConfigurationManager CreateConfigurationManager()
        {
            return Instance.CreateConfigurationManager();
        }

        public static IHostSettingsProvider CreateHostSettingsProvider()
        {
            return Instance.CreateHostSettingsProvider();
        }

        public static IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            return Instance.CreateStoreSettingsProvider();
        }

        public static HccDbContext CreateHccDbContext()
        {
            return Instance.CreateHccDbContext();
        }

        public static HccDbContext CreateReadOnlyHccDbContext()
        {
            return Instance.CreateReadOnlyHccDbContext();
        }

        public static T CreateRepo<T>() where T : class, IRepo
        {
            return Instance.CreateRepo<T>();
        }

        public static T CreateRepo<T>(HccRequestContext context) where T : class, IRepo
        {
            return Instance.CreateRepo<T>(context);
        }

        public static T CreateService<T>() where T : HccServiceBase
        {
            return Instance.CreateService<T>();
        }

        public static T CreateService<T>(HccRequestContext context) where T : HccServiceBase
        {
            return Instance.CreateService<T>(context);
        }
    }
}