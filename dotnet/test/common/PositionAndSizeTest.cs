// <copyright file="PositionAndSizeTest.cs" company="Selenium Committers">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

using NUnit.Framework;
using OpenQA.Selenium.Environment;
using System;
using System.Drawing;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class PositionAndSizeTest : DriverTestFixture
    {
        [Test]
        public void ShouldBeAbleToDetermineTheLocationOfAnElement()
        {
            driver.Url = xhtmlTestPage;

            IWebElement element = driver.FindElement(By.Id("username"));
            Point location = element.Location;

            Assert.That(location.X, Is.GreaterThan(0));
            Assert.That(location.Y, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldGetCoordinatesOfAnElement()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/simple_page.html");
            Assert.AreEqual(new Point(10, 10), GetLocationInViewPort(By.Id("box")));
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesOfAnEmptyElement()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_empty_element.html");
            Assert.AreEqual(new Point(10, 10), GetLocationInViewPort(By.Id("box")));
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesOfATransparentElement()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_transparent_element.html");
            Assert.AreEqual(new Point(10, 10), GetLocationInViewPort(By.Id("box")));
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesOfAHiddenElement()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_hidden_element.html");
            Assert.AreEqual(new Point(10, 10), GetLocationInViewPort(By.Id("box")));
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesOfAnInvisibleElement()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_invisible_element.html");
            Assert.AreEqual(new Point(0, 0), GetLocationInViewPort(By.Id("box")));
            Assert.AreEqual(new Point(0, 0), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldScrollPageAndGetCoordinatesOfAnElementThatIsOutOfViewPort()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_element_out_of_view.html");
            int windowHeight = driver.Manage().Window.Size.Height;
            Point location = GetLocationInViewPort(By.Id("box"));
            Assert.That(location.X, Is.EqualTo(10));
            Assert.That(location.Y, Is.GreaterThanOrEqualTo(0));
            Assert.That(GetLocationOnPage(By.Id("box")), Is.EqualTo(new Point(10, 5010)));
            // GetLocationInViewPort only works within the context of a single frame
            // for W3C-spec compliant remote ends.
            // Assert.That(location.Y, Is.LessThanOrEqualTo(windowHeight - 100));
        }

        [Test]
        public void ShouldGetCoordinatesOfAnElementInAFrame()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/element_in_frame.html");
            driver.SwitchTo().Frame("ifr");
            IWebElement box = driver.FindElement(By.Id("box"));
            Assert.AreEqual(new Point(10, 10), box.Location);
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesInViewPortOfAnElementInAFrame()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/element_in_frame.html");
            driver.SwitchTo().Frame("ifr");
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
            // GetLocationInViewPort only works within the context of a single frame
            // for W3C-spec compliant remote ends.
            // Assert.AreEqual(new Point(25, 25), GetLocationInViewPort(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesInViewPortOfAnElementInANestedFrame()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/element_in_nested_frame.html");
            driver.SwitchTo().Frame("ifr");
            driver.SwitchTo().Frame("ifr");
            Assert.AreEqual(new Point(10, 10), GetLocationOnPage(By.Id("box")));
            // GetLocationInViewPort only works within the context of a single frame
            // for W3C-spec compliant remote ends.
            // Assert.AreEqual(new Point(40, 40), GetLocationInViewPort(By.Id("box")));
        }

        [Test]
        public void ShouldGetCoordinatesOfAnElementWithFixedPosition()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("coordinates_tests/page_with_fixed_element.html");
            Assert.That(GetLocationInViewPort(By.Id("fixed")).Y, Is.EqualTo(0));
            Assert.That(GetLocationOnPage(By.Id("fixed")).Y, Is.EqualTo(0));

            driver.FindElement(By.Id("bottom")).Click();
            Assert.That(GetLocationInViewPort(By.Id("fixed")).Y, Is.EqualTo(0));
            Assert.That(GetLocationOnPage(By.Id("fixed")).Y, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldCorrectlyIdentifyThatAnElementHasWidthAndHeight()
        {
            driver.Url = xhtmlTestPage;

            IWebElement shrinko = driver.FindElement(By.Id("linkId"));
            Size size = shrinko.Size;
            Assert.That(size.Width, Is.GreaterThan(0), "Width expected to be greater than 0");
            Assert.That(size.Height, Is.GreaterThan(0), "Height expected to be greater than 0");
        }

        [Test]
        public void ShouldHandleNonIntegerPositionAndSize()
        {
            driver.Url = rectanglesPage;

            IWebElement r2 = driver.FindElement(By.Id("r2"));
            string left = r2.GetCssValue("left");
            Assert.AreEqual(10.9, Math.Round(Convert.ToDecimal(left.Replace("px", "")), 1));
            string top = r2.GetCssValue("top");
            Assert.AreEqual(10.1, Math.Round(Convert.ToDecimal(top.Replace("px", "")), 1));
            Assert.AreEqual(new Point(11, 10), r2.Location);
            string width = r2.GetCssValue("width");
            Assert.AreEqual(48.7, Math.Round(Convert.ToDecimal(width.Replace("px", "")), 1));
            string height = r2.GetCssValue("height");
            Assert.AreEqual(49.3, Math.Round(Convert.ToDecimal(height.Replace("px", "")), 1));
            Assert.AreEqual(new Size(49, 49), r2.Size);
        }

        //------------------------------------------------------------------
        // Tests below here are not included in the Java test suite
        //------------------------------------------------------------------
        [Test]
        public void ShouldBeAbleToDetermineTheSizeOfAnElement()
        {
            driver.Url = xhtmlTestPage;

            IWebElement element = driver.FindElement(By.Id("username"));
            Size size = element.Size;

            Assert.That(size.Width, Is.GreaterThan(0));
            Assert.That(size.Height, Is.GreaterThan(0));
        }

        private Point GetLocationInViewPort(By locator)
        {
            IWebElement element = driver.FindElement(locator);
            return ((ILocatable)element).Coordinates.LocationInViewport;
        }

        private Point GetLocationOnPage(By locator)
        {
            IWebElement element = driver.FindElement(locator);
            return ((ILocatable)element).Coordinates.LocationInDom;
        }
    }
}
