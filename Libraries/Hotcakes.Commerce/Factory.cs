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
    /// <summary>Provides the process-wide entry point to the configured commerce factory.</summary>
    /// <remarks>Initialization honors the <c>HccFactory</c> app setting, then discovers a concrete <see cref="IFactory"/> in loaded assemblies. The selected provider is instantiated once.</remarks>
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

        /// <summary>Gets the factory provider selected during static initialization.</summary>
        public static IFactory Instance { get; private set; }

        /// <summary>Gets or sets the HTTP context used by commerce code.</summary>
        /// <remarks>The getter prefers <see cref="HttpContext.Current"/> and falls back to the thread-local value.</remarks>
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

        /// <summary>Creates the configured URL resolver.</summary>
        public static IHccUrlResolver CreateHccUrlResolver()
        {
            return Instance.CreateHccUrlResolver();
        }

        /// <summary>Creates the configured form renderer.</summary>
        public static IHccFormRenderer CreateHccFormRenderer()
        {
            return Instance.CreateHccFormRenderer();
        }

        /// <summary>Creates the configured event logger.</summary>
        public static ILogger CreateEventLogger()
        {
            return Instance.CreateEventLogger();
        }

        /// <summary>Creates a social service for a request context.</summary>
        /// <param name="context">The commerce request context.</param>
        public static ISocialService CreateSocialService(HccRequestContext context)
        {
            return Instance.CreateSocialService(context);
        }

        /// <summary>Creates a workflow factory for a request context.</summary>
        /// <param name="context">The commerce request context.</param>
        /// <remarks>The misspelled <c>CreateWorklflowFactory</c> name is preserved for compatibility.</remarks>
        public static WorkflowFactory CreateWorklflowFactory(HccRequestContext context)
        {
            return Instance.CreateWorkflowFactory(context);
        }

        /// <summary>Creates the configured configuration manager.</summary>
        public static IConfigurationManager CreateConfigurationManager()
        {
            return Instance.CreateConfigurationManager();
        }

        /// <summary>Creates the host-settings provider.</summary>
        public static IHostSettingsProvider CreateHostSettingsProvider()
        {
            return Instance.CreateHostSettingsProvider();
        }

        /// <summary>Creates the store-settings provider.</summary>
        public static IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            return Instance.CreateStoreSettingsProvider();
        }

        /// <summary>Creates a read/write commerce database context.</summary>
        public static HccDbContext CreateHccDbContext()
        {
            return Instance.CreateHccDbContext();
        }

        /// <summary>Creates a read-only commerce database context.</summary>
        public static HccDbContext CreateReadOnlyHccDbContext()
        {
            return Instance.CreateReadOnlyHccDbContext();
        }

        /// <summary>Creates a repository for the current request context.</summary>
        /// <typeparam name="T">The repository type.</typeparam>
        public static T CreateRepo<T>() where T : class, IRepo
        {
            return Instance.CreateRepo<T>();
        }

        /// <summary>Creates a repository for an explicit request context.</summary>
        /// <typeparam name="T">The repository type.</typeparam>
        /// <param name="context">The commerce request context.</param>
        public static T CreateRepo<T>(HccRequestContext context) where T : class, IRepo
        {
            return Instance.CreateRepo<T>(context);
        }

        /// <summary>Creates a service for the current request context.</summary>
        /// <typeparam name="T">The service type.</typeparam>
        public static T CreateService<T>() where T : HccServiceBase
        {
            return Instance.CreateService<T>();
        }

        /// <summary>Creates a service for an explicit request context.</summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="context">The commerce request context.</param>
        public static T CreateService<T>(HccRequestContext context) where T : HccServiceBase
        {
            return Instance.CreateService<T>(context);
        }
    }
}
