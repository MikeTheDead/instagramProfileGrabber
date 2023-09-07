using instagramProfileGrabber.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using InstaSharper;
using InstaSharper.API.Builder;
using InstaSharper.API;
using InstaSharper.Classes;
using InstaSharper.Logger;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace instagramProfileGrabber.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> Index(string username)
        {
            // Configure ChromeOptions for headless mode.
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Run in headless mode.

            // Initialize ChromeDriver with the configured options.
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                // Navigate to the Instagram profile page.
                driver.Navigate().GoToUrl($"https://www.instagram.com/{username}/");

                // Wait for a few seconds to ensure the page loads (you can adjust the wait time as needed).
                System.Threading.Thread.Sleep(3000);

                // Attempt to find the profile picture element based on "username's profile picture."
                IWebElement profilePictureElement = null;
                try
                {
                    profilePictureElement = driver.FindElement(By.CssSelector($"img[alt*=\"{username}'s profile picture\"]"));
                }
                catch { }

                // If the public profile picture method fails, try the private profile method.
                if (profilePictureElement == null)
                {
                    // Find the button element containing the profile picture image.
                    IWebElement buttonElement = driver.FindElement(By.CssSelector("button._aadn"));

                    // Find the profile picture image within the button element.
                    profilePictureElement = buttonElement.FindElement(By.CssSelector("img._aadp"));
                }

                // Get the value of the src attribute (profile picture URL).
                string profilePictureUrl = profilePictureElement.GetAttribute("src");

                // Redirect to the profile picture URL.
                return Redirect(profilePictureUrl);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, e.g., log the exception message.
                Console.WriteLine("Error: " + ex.Message);

                // Handle the case where the profile picture cannot be fetched.
                return BadRequest("Failed to fetch profile picture.");
            }
            finally
            {
                // Close the browser.
                driver.Quit();
            }
        }
    }
}