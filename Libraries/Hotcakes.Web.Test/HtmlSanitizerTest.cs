using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test
{
    
    
    /// <summary>
    ///This is a test class for HtmlSanitizerTest and is intended
    ///to contain all HtmlSanitizerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HtmlSanitizerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for MakeHtmlSafe
        ///</summary>
        [TestMethod()]
        public void CanParseImageTagsNextToEachOther()
        {                        
            // image names are removed because they aren't valid URIs and are 
            // possible vectors for XSS
            XssTest("<p><img src=\"a.jpg\" /><img src=\"b.jpg\" /></p>", 
                    "<p><img src=\"  \"/><img src=\"  \"/></p>");            
        }

        [TestMethod()]
        public void CanParseImageTagsNextToEachOther2()
        {
            // image names are removed because they aren't valid URIs and are 
            // possible vectors for XSS
            XssTest("<p><img src=\"http://www.domain.com/a.jpg\" /><img src=\"http://www.domain.com/b.jpg\" /></p>",
                    "<p><img src=\"http://www.domain.com/a.jpg\"/><img src=\"http://www.domain.com/b.jpg\"/></p>");
        }

        private void XssTest(string input, string expected)
        {
            string actual;
			actual = Hotcakes.Web.HtmlSanitizer.MakeHtmlSafe(input);
            Assert.AreEqual(expected, actual);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod()]
        public void XssFilterOnClickFromATags()
        {
            XssTest("<a id=\"another\" href=\"http://www.yahoo.com\" onclick=\"javascript:alert('test');\">Click</a>",
                    "<a id=\"another\" href=\"http://www.yahoo.com/\">Click</a>");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssInputTag()
        {
            XssTest("<input type=\"button\" />",
                    "&lt;input type=&quot;button&quot; /&gt;");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssImageTag()
        {
            XssTest("<img src=\"http://mypic.com/image.jpg\" alt=\"something\" />",
                    "<img src=\"http://mypic.com/image.jpg\" alt=\"something\"/>");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssSimpleScriptTag()
        {
            XssTest("<script src=\"submit\">",
                    "&lt;script src=&quot;submit&quot;&gt;");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssUrlEncoded()
        {
            XssTest("<a href=\"%27%3B%61%6C%65%72%74%28%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%38%38%2C%38%33%2C%38%33%29%29%2F%2F%5C%27%3B%61%6C%65%72%74%28%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%38%38%2C%38%33%2C%38%33%29%29%2F%2F%22%3B%61%6C%65%72%74%28%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%38%38%2C%38%33%2C%38%33%29%29%2F%2F%5C%22%3B%61%6C%65%72%74%28%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%38%38%2C%38%33%2C%38%33%29%29%2F%2F%2D%2D%3E%3C%2F%53%43%52%49%50%54%3E%22%3E%27%3E%3C%53%43%52%49%50%54%3E%61%6C%65%72%74%28%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%38%38%2C%38%33%2C%38%33%29%29%3C%2F%53%43%52%49%50%54%3E\">Click Me</a>",
                    "<a href=\"  \">Click Me</a>");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssCssLocator()
        {
            XssTest("'';!--\"<XSS>=&{()}",
                    "&#39;&#39;;!--&quot;&lt;XSS&gt;=&amp;{()}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssEncodeScriptTags()
        {
            XssTest("<script src=http://ha.ckers.org/xss.js></script>",
                    "&lt;script src=http://ha.ckers.org/xss.js&gt;&lt;/script&gt;");
        }

        [TestMethod]
        public void XssImageTagWithJavaScriptSource()
        {
            XssTest("<img src=\"javascript:alert('XSS');\"/>",
                    "<img src=\"  \"/>");
        }

        [TestMethod]
        public void XssCaseInsensitveJavaScriptHref()
        {
            XssTest("<img src=\"JaVaScRiPt:alert('XSS')\"/>",
                    "<img src=\"  \"/>");
        }


        // 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssVBScriptSrcTagInImage()
        {
            XssTest("<img src=\"vbscript:msgbox(\'XSS\')\"/>",
                    "<img src=\"  \"/>");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssDivJavaScriptStyleTag()
        {
            XssTest("<div style=\"background-image: url(javascript:alert('XSS'))\"></div>",
                    "<div></div>");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xss"), TestMethod]
        public void XssCssExpressionAttack()
        {
            XssTest("<div style=\"width: expression(alert('XSS'));\"></div>",
                    "<div></div>");
        }
        
    }
}
