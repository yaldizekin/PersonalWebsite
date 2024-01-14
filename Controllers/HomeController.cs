using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;
using System.Diagnostics;

namespace Portfolio.Controllers
{

    public class HomeController : Controller
    {
		private readonly ApplicationDbContext _context;
		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
        {
            return View(_context.Projects.OrderByDescending(project => project.Id).ToList());
        }

        [Route(template:"hakkinda")]
        public IActionResult About()
        {
            return View(_context.Projects.OrderByDescending(project => project.Id).ToList());
        }

        [Route("{slug}")]
        public IActionResult ProjectDetail(string slug)
        {
            var post = _context
                 .Projects
                 .Where(blog => blog.Slug == slug)
                 .FirstOrDefault();


            if (post != null)
            {
                return View(post);
            }

            Response.StatusCode = 404;
            return View("PageNotFound");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}