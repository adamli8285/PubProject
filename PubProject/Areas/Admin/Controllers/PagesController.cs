using PubProject.Models.Data;
using PubProject.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PubProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // Get Admin - Pages
        public ActionResult Index()
        {
            // Declaring list of PageVM
            List<PageVM> pagesList;

            using (Db db = new Db())
            {
                // define the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            // Return Pages Listview
            return View(pagesList);
        }

        // GET Admin - AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin - Addpage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Check model status
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Declaring slug
                string slug;

                // define pageDTO
                PageDTO dto = new PageDTO();

                // DTO title
                dto.Title = model.Title;

                // Check if the slug is null
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Make sure title and slug are unique
                if ( db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug) )
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                // define DTO attribues 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You've added a new page!";

            // Redirect to AddPage
            return RedirectToAction("AddPage");
        }

        // GET: Admin - Edit Page
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declaring pageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page Id
                PageDTO dto = db.Pages.Find(id);

                // Confirm if the page exists
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // define pageVM
                model = new PageVM(dto);
            }

            // Return modleview list
            return View(model);
        }

        // POST: Admin - EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Check model state
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Get page id
                int id = model.Id;

                // define slug
                string slug = "home";

                // get page by its id
                PageDTO dto = db.Pages.Find(id);

                
                dto.Title = model.Title;

                // Check the slug is not equal to home
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // Make sure title and slug are unique
                if ( db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                     db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                // define the dto attributes
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save the DTO
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You've edited the page!";

            // Redirect
            return RedirectToAction("EditPage");
        }

        // GET: Admin Page Detials
        public ActionResult PageDetails(int id)
        {
            // Declaring PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page by id 
                PageDTO dto = db.Pages.Find(id);

                // Confirm if the page exists
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                // define PageVM
                model = new PageVM(dto);
            }

            // Return view with model
            return View(model);
        }

        // GET: Admin - Delte Page
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                // Get the page by id 
                PageDTO dto = db.Pages.Find(id);

                // Remove the page from DTO
                db.Pages.Remove(dto);

                // Save
                db.SaveChanges();
            }

            // Redirect
            return RedirectToAction("Index");
        }

        // POST: Admin - Page Record
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // Set initial count
                int count = 1;

                // Declaring PageDTO
                PageDTO dto;

                // Set sorting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }

        }

        // GET: Admin - edit side bar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declaring model
            SidebarVM model;

            using (Db db = new Db())
            {
                // Get the sidebar status
                SidebarDTO dto = db.Sidebar.Find(1);

                // define model
                model = new SidebarVM(dto);
            }

            // Return view with model
            return View(model);
        }

        // POST: Admin - edit sidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                // dto body equal to model body
                dto.Body = model.Body;

                // Save
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You've edited the sidebar!";

            // Redirect
            return RedirectToAction("EditSidebar");
        }
    }
}