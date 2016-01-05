using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace SeLocatorBench
{
    class Program
    {
        private List<string> _locatorTypes = new List<string>
        {
            "id",
            "cssSelector",
            "className",
            "xpath",
            "linkText",
            "name",
            "partialLinkText",
            "tagName"
        };

        private static int _testCount = 12;
        
        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine("FIREFOX START");
            p.RunTests("ff", _testCount);
            Console.WriteLine("FIREFOX END");
            
            Console.WriteLine("CHROME START");
            p.RunTests("chrome", _testCount);
            Console.WriteLine("CHROME END");
            
            Console.WriteLine("IE START");
            p.RunTests("ie", _testCount);
            Console.WriteLine("IE END");

            Console.ReadKey();
        }

        private void RunTests(string browser, int count)
        {
            IWebDriver driver;
            switch (browser)
            {
                case("ff"):
                    driver = new FirefoxDriver();
                    break;
                case("chrome"):
                    driver = new ChromeDriver();
                    break;
                case("ie"):
                    driver = new InternetExplorerDriver();
                    break;
                default:
                    driver = new ChromeDriver();
                    break;
            }
            
            driver.Navigate().GoToUrl("https://qa.builddirect.com/Laminate-Flooring/Piers-Hickory/ProductDisplay_6951_p1_10100592.aspx");

            var total = new Dictionary<string, List<long>>();

            var selectorList = new Dictionary<string, By>
            {
                {_locatorTypes[0], By.Id("_ctl9_footer")},
                {_locatorTypes[1], By.CssSelector("[data-qa-id='footerWelcome']")},
                {_locatorTypes[2], By.ClassName("footer_col")},
                {_locatorTypes[3], By.XPath("//div/div/div/div/a[contains(text(),'to BuildDirect')]")},
                {_locatorTypes[4], By.LinkText("Welcome to BuildDirect")},
                {_locatorTypes[5], By.Name("spr")},
                {_locatorTypes[6], By.PartialLinkText("Welcome to")},
                {_locatorTypes[7], By.TagName("input")}
            };

            // attempted to load each locator type before refresh but results were unusual
            foreach (var type in _locatorTypes)
            {
                for (var i = 0; i < count; i++)
                {
                    driver.Navigate().Refresh(); // selenium cached results so had to force refresh
                    var sw = new Stopwatch();
                    sw.Start();
                    driver.FindElement(selectorList[type]);
                    sw.Stop();

                    if (!total.ContainsKey(type))
                    {
                        total.Add(type, new List<long>());
                    }
                    total[type].Add(sw.ElapsedMilliseconds);

                    //Console.WriteLine(type + " " + sw.ElapsedMilliseconds + " ms");
                }
            }
            
            driver.Quit();
            foreach (var type in _locatorTypes)
            {
                Console.WriteLine(type);
                Console.WriteLine("min: " + total[type].Min() + " ms");
                Console.WriteLine("max: " + total[type].Max() + " ms");
                Console.WriteLine("avg: " + total[type].Average() + " ms");
            }
        }
    }
}
