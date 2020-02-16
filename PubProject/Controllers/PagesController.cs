using PubProject.Models.Data;
using PubProject.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PubProject.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Get/set page slug
            if (page == "")
                page = "home";

            // Declaring model and DTO
            PageVM model;
            PageDTO dto;

            // Check if page exists
            using (Db db = new Db())
            {
                if (! db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Get page DTO
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Set page title
            ViewBag.PageTitle = dto.Title;

            // Check for sidebar status
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // define model
            model = new PageVM(dto);

            // Return view with model
            return View(model);
        }
        
        public ActionResult PagesMenuPartial()
        {
            // Declaring a list of PageVM
            List<PageVM> pageVMList;

            // Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            // Return partial view with list
            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            // Declaring model
            SidebarVM model;

            // define model
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                model = new SidebarVM(dto);
            }

            // Return partial view with model
            return PartialView(model);
        }
    }
}