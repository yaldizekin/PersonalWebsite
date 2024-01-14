using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Portfolio.Areas.Admin.Models;
using Portfolio.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Slugify;
using System.Data;

namespace Portfolio.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("admin")]
        public IActionResult Index()
        {
            
            return View(_context.Projects.OrderByDescending(project => project.Id).ToList());
        }

        public IActionResult AddProject()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddProject(Project b)
        {

            if (CheckIfSlugExists(b.Slug))
            {
                ViewBag.Msg = "Aynı slug olduğu için ekleme yapılmadı";
                return View("Msg");
            }
            if (b.Image != null)
            {
                var extension = Path.GetExtension(b.Image.FileName);
                var newimagename = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", newimagename);
                var stream = new FileStream(location, FileMode.Create);

                //b.Image.CopyTo(stream);
                using (var image = Image.Load(b.Image.OpenReadStream()))
                {
                    var newWidth = 800;

                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new Size(newWidth /*newHeight*/),
                            Mode = ResizeMode.Max // Bu boyutlandırma modunu isteğinize göre değiştirin
                        }));

                    image.Save(stream, new JpegEncoder()); // Çıktıyı stream'e kaydet, formatı ayarlayabilirsiniz
                }
                b.ImagePath = newimagename;

            }


            _context.Add(b);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public IActionResult GenerateSlug(string title)
        {
            SlugHelper.Config config = new SlugHelper.Config();
            config.CharacterReplacements.Add("ı", "i");
            SlugHelper helper = new SlugHelper(config);

            string slug = helper.GenerateSlug(title);

            return Content(slug);
        }

        public bool CheckIfSlugExists(string slug)
        {
            if (_context.Projects.Any(p => p.Slug == slug))
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public IActionResult CheckSlugExists(string slug)
        {

            if (CheckIfSlugExists(slug))
            {
                return Json(
                   new
                   {
                       exists = true
                   }
                );
            }

            return Json(
                   new
                   {
                       exists = false
                   }
                );
        }

        [Route("/admin/deleteproject/{id}")]
        public IActionResult DeleteBlog(int id)
        {
            _context.Projects.Remove(
                    _context.Projects.Find(id)
                );
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }



    }
}
