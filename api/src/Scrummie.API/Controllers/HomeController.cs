using Microsoft.AspNetCore.Mvc;

namespace Scrummie.API.Controllers
{
    /// <summary>
    /// The home controller class.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Get the default content of home page.
        /// </summary>
        /// <returns>Default content.</returns>
        [HttpGet("/")]
        public string Get()
        {
            return "Home Page";
        }
    }
}