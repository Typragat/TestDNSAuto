using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitySelectionTests.PageObject
{
    class ModalCitySelectionPageObject
    {
        private IWebDriver _webdriver;
        private readonly By _searchInput = By.CssSelector("#select-city input[type='text']");
        private readonly By _openModalButton = By.CssSelector(".city-select__label");
        private readonly string _modalWindowSelector = "#v-select-city-modal";

        public ModalCitySelectionPageObject(IWebDriver webdriver)
        {
            _webdriver = webdriver;
        }
        public ModalCitySelectionPageObject OpenModal()
        {
            _webdriver.FindElement(_openModalButton).Click();

            return new ModalCitySelectionPageObject(_webdriver);
        }

        public ModalCitySelectionPageObject ClickDistrict(int district_number)
        {
            By district_selector = By.CssSelector("#select-city .districts>li:nth-child(" + district_number.ToString() + ")");
            _webdriver.FindElement(district_selector).Click();

            return new ModalCitySelectionPageObject(_webdriver);
        }
        public ModalCitySelectionPageObject ClickRegion(int region_number)
        {
            By region_selector = By.CssSelector("#select-city .regions>li:nth-child(" + region_number.ToString() + ")");
            if(region_number > 9) //scroll into view
            {
                var element = _webdriver.FindElement(region_selector);
                OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(_webdriver);
                actions.MoveToElement(element);
                actions.Perform();
            }
            _webdriver.FindElement(region_selector).Click();

            return new ModalCitySelectionPageObject(_webdriver);
        }
        public ModalCitySelectionPageObject ClickCity(int city_number)
        {
            By city_selector = By.CssSelector("#select-city .cities>li:nth-child(" + city_number.ToString() + ")");
            if (city_number > 9) //scroll into view
            {
                var element = _webdriver.FindElement(city_selector);
                OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(_webdriver);
                actions.MoveToElement(element);
                actions.Perform();
            }
            _webdriver.FindElement(city_selector).Click();

            return new ModalCitySelectionPageObject(_webdriver);
        }
        public ModalCitySelectionPageObject InputSearchFor(string value)
        {
            _webdriver.FindElement(_searchInput).SendKeys(value);

            return new ModalCitySelectionPageObject(_webdriver);
        }
        public ModalCitySelectionPageObject ClearInputSearchFor()
        {
            _webdriver.FindElement(_searchInput).Clear();

            return new ModalCitySelectionPageObject(_webdriver);
        }
        public bool CheckModalWindowIsVisible()
        {
            try
            {
                _webdriver.PageSource.Contains(_modalWindowSelector);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public int CheckRegionsCount()
        {
            var list_of_regions = _webdriver.FindElements(By.CssSelector(".regions>li"));
            return list_of_regions.Count;
        }
        public int CheckCitiesCount()
        {
            var list_of_cities = _webdriver.FindElements(By.CssSelector(".cities>li"));
            return list_of_cities.Count;
        }
        public string[] GetCitiesArray()
        {
            string[] cities_list = new string[8];
            var mainMenu = new ModalCitySelectionPageObject(_webdriver);
            _webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            Random rnd = new Random();
            mainMenu.OpenModal();
            for(int i = 0; i < 8; i++) 
            { 
                int rnd_district = rnd.Next(1, 8);
                mainMenu
                    .ClickDistrict(rnd_district)
                    .ClickRegion(rnd.Next(1, mainMenu.CheckRegionsCount()));
                int rnd_city = rnd.Next(1, mainMenu.CheckCitiesCount());
                rnd_city = rnd_city % 9;
                if(rnd_city == 0)
                {
                    rnd_city++;
                }
                cities_list[i] = _webdriver.FindElement(By.CssSelector("#select-city .cities>li:nth-child(" + rnd_city.ToString() + ") span")).Text;
            }
            return cities_list;
        }
    }
}
