using System;
using System.Configuration;
using System.Xml.Linq;

namespace Hotcakes.Commerce.Tests.TestData
{
    public class XmlDataContext
    {
        private readonly string _path = string.Empty;

        /// <summary>
        /// Initializes a XML file path.
        /// </summary>
        /// <param name="key">The key.</param>
        public XmlDataContext(string key)
        {
            this._path = Convert.ToString(ConfigurationManager.AppSettings[key]);
        }

        /// <summary>
        /// Get XML file data
        /// </summary>
        /// <returns></returns>
        public XElement GetXml()
        {
            try
            {
                if (string.IsNullOrEmpty(_path)) return null;
                var xmldoc = XElement.Load(_path);
                return xmldoc;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
