using CitySelectionTests.PageObject;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace CitySelectionTests
{

    public class Tests
    {
        string[] cities_list = new string[8];
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions().BinaryLocation = @"C:\chromedriver";
            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("https://www.dns-shop.ru/");
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void TC1_OpenModalWindow()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            mainMenu
                .OpenModal();
            Assert.IsTrue(mainMenu.CheckModalWindowIsVisible());
        }
        [Test]
        public void TC2_SelectingDistrict_WITH_TC9_AllDistrictsHaveRegions()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            mainMenu.OpenModal();
            for(int i = 1; i < 9; i++)
            {
                mainMenu.ClickDistrict(i);
                Assert.IsTrue(mainMenu.CheckRegionsCount() > 0);
            }
        }

        [Test]
        public void TC3_SelectingRegion_WITH_TC10_AllRegionsHaveCities()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            mainMenu.OpenModal();
            for (int i = 1; i < 9; i++)
            {
                mainMenu.ClickDistrict(i);
                int reg_count = mainMenu.CheckRegionsCount();
                for (int j = 1; j <= reg_count; j++)
                {
                    mainMenu.ClickRegion(j);

                    Assert.IsTrue(mainMenu.CheckCitiesCount() > 0);

                }
            }
        }
        [Test]
        public void TC4_SelectingCity()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            Random rnd = new Random();
            int rnd_district = rnd.Next(1, 8);            
            mainMenu.
                OpenModal()
                .ClickDistrict(rnd_district)
                .ClickRegion(rnd.Next(1, mainMenu.CheckRegionsCount()));
            int rnd_city = rnd.Next(1, mainMenu.CheckCitiesCount());
            string expected_city_name = driver.FindElement(By.CssSelector("#select-city .cities>li:nth-child(" + rnd_city.ToString() + ")")).Text; // saving chosen city name from modal window
            mainMenu.ClickCity(rnd_city);
            string actual_city_name = driver.FindElement(By.CssSelector(".city-select__label>p")).Text; // saving actual city name
            Assert.AreEqual(actual_city_name, expected_city_name);
        }

        [Test]
        public void TC5_SelectingCityFromBigCitiesList()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            Random rnd = new Random();
            int rnd_city = rnd.Next(1, 8);
            string city_selector = ".big-cities-bubble-list>span:nth-child(" + rnd_city.ToString() + ")>a";
            mainMenu
                .OpenModal();
            string expected_city_name = driver.FindElement(By.CssSelector(city_selector)).Text; // saving chosen city name from modal window
            driver.FindElement(By.CssSelector(city_selector)).Click();
            string actual_city_name = driver.FindElement(By.CssSelector(".city-select__label>p")).Text; // saving actual city name
            Assert.AreEqual(actual_city_name, expected_city_name);
        }

        [Test]
        public void TC6_SelectingOldCititesList()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            Random rnd = new Random();
            int rnd_district = rnd.Next(5, 8);
            mainMenu
                .OpenModal()
                .ClickDistrict(rnd_district);
            var list_of_cities_before = driver.FindElements(By.CssSelector(".cities>li"));
            rnd_district = rnd.Next(1, 4);
            mainMenu.ClickDistrict(rnd_district);
            var list_of_cities_after = driver.FindElements(By.CssSelector(".cities>li"));
            Assert.AreEqual(list_of_cities_before, list_of_cities_after);
        }

        [Test]
        public void TC7_SelectingSeveralDistricts()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            Random rnd = new Random();
            int rnd_district = rnd.Next(5, 8);
            mainMenu
                .OpenModal()
                .ClickDistrict(rnd_district);
            var list_of_regions_before = driver.FindElements(By.CssSelector(".regions>li"));
            rnd_district = rnd.Next(1, 4);
            mainMenu.ClickDistrict(rnd_district);
            var list_of_regions_after = driver.FindElements(By.CssSelector(".regions>li"));
            Assert.AreNotEqual(list_of_regions_before, list_of_regions_after);
        }

        [Test]
        public void TC8_SelectingSeveralRegions()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            Random rnd = new Random();
            int rnd_district = rnd.Next(1, 8);
            mainMenu.
                OpenModal()
                .ClickDistrict(rnd_district)
                .ClickRegion(rnd.Next(4, mainMenu.CheckRegionsCount()));
            var list_of_cities_before = driver.FindElements(By.CssSelector(".cities>li"));
            int rnd_region = rnd.Next(1, 4);
            mainMenu.ClickRegion(rnd_region);
            var list_of_cities_after = driver.FindElements(By.CssSelector(".cities>li"));
            Assert.AreNotEqual(list_of_cities_before, list_of_cities_after);
        }

        [Test]
        public void TC11_SearchingByFullCityName()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            cities_list = mainMenu.GetCitiesArray(); // Filling the array with 8 random cities from 8 random 
            for(int i = 0; i < 8; i++)               // regions from each district
            {
                mainMenu.InputSearchFor(cities_list[i]);
                string actual_city = driver.FindElement(By.CssSelector(".cities-search mark:first-child")).Text;
                Assert.AreEqual(cities_list[i], actual_city);
                mainMenu.ClearInputSearchFor();
            }
        }

        [Test]
        public void TC12_SearchingByShortCityName()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            cities_list = mainMenu.GetCitiesArray(); // Filling the array with 8 random cities from 8 random 
            for (int i = 0; i < 8; i++)              // regions from each district
            {
                string short_city_name = cities_list[i].Substring(0, 3);
                mainMenu.InputSearchFor(short_city_name);
                string actual_short_city_name = driver.FindElement(By.CssSelector(".cities-search mark:first-child")).Text;
                actual_short_city_name = actual_short_city_name.Substring(0, 3);
                Assert.AreEqual(short_city_name, actual_short_city_name);
                mainMenu.ClearInputSearchFor();
            }
        }

        [Test]
        public void TC13_SearchingByFullCityNameDiffRegistre()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            var mainMenu = new ModalCitySelectionPageObject(driver);
            cities_list = mainMenu.GetCitiesArray(); // Filling the array with 8 random cities from 8 random 
            for (int i = 0; i < 8; i++) //Uppercase  // regions from each district
            {
                mainMenu.InputSearchFor(cities_list[i].ToUpper());
                string actual_city = driver.FindElement(By.CssSelector(".cities-search mark:first-child")).Text;
                Assert.AreEqual(cities_list[i], actual_city);
                mainMenu.ClearInputSearchFor();
            }
            for (int i = 0; i < 8; i++) //Lowercase 
            {
                mainMenu.InputSearchFor(cities_list[i].ToLower());
                string actual_city = driver.FindElement(By.CssSelector(".cities-search mark:first-child")).Text;
                Assert.AreEqual(cities_list[i], actual_city);
                mainMenu.ClearInputSearchFor();
            }
        }


        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}