using HelpDeskApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace HelpDeskApp.Controllers
{
    [Authorize]
    public class HelpDeskController : Controller
    {

        private readonly HelpDeskContext _db;

        public HelpDeskController(HelpDeskContext db)
        {
            _db = db;
        }
        //Need to only give access to tickets of the logged in user.
        public IActionResult Index()
        {
            IEnumerable<Ticket> userTickets = _db.Tickets.Where(ticket => ticket.Email == User.Identity.Name && ticket.Status != "Closed");
            return View(userTickets);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket obj)
        {
            if (obj.Status == null) {
                obj.Status = "Not Started";
            }

            if (ModelState.IsValid)
            {
                _db.Tickets.Add(obj);
                _db.SaveChanges();
                if (User.IsInRole("Administrator")) return RedirectToAction("Admin"); 
                else return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Tickets.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //POST - EDIT
        public IActionResult Edit(Ticket obj)
        {
            if (ModelState.IsValid)
            {
                _db.Tickets.Update(obj);
                _db.SaveChanges();
                if (User.IsInRole("Administrator")) return RedirectToAction("Admin");
                else return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET - Edit
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Tickets.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Admin()
        {
            IEnumerable<Ticket> objList = _db.Tickets.Where(ticket => ticket.Status != "Closed");
            return View(objList);
        }

        public async Task<IActionResult> Closed(
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            if(searchString != null) { pageNumber = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;

            var items = from i in _db.Tickets select i;
            items = items.Where(item => item.Status == "Closed" && item.Email == User.Identity.Name);

            //search by attributes placed in this block
            if (!String.IsNullOrEmpty(searchString)) 
            {
                items = items.Where(ticket => ticket.Category.Contains(searchString));
            }

            int pageSize = 50;
            return View(await PaginatedList<Ticket>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ClosedAdmin(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CategorySortParm"] = String.IsNullOrEmpty(sortOrder) ? "Category" : "";
            ViewData["LocationSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Location" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
            ViewData["EmailSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Email" : "";

            if(searchString != null) {  pageNumber = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;

            var items = from i in _db.Tickets select i;
            items = items.Where(ticket => ticket.Status == "Closed");

            //search by attributes placed in this block
            if(!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(ticket => ticket.Category.Contains(searchString) || ticket.Location.Contains(searchString) ||
                ticket.Name.Contains(searchString) || ticket.Email.Contains(searchString));
            }

            //Chooses what to sort by according to parameter
            switch (sortOrder)
            {
                case "Category":
                    items = items.OrderByDescending(i => i.Category);
                    break;
                case "Location":
                    items = items.OrderBy(i => i.Location);
                    break;
                case "Name":
                    items = items.OrderBy(i => i.Name);
                    break;
                case "Email":
                    items = items.OrderBy(i => i.Email);
                    break;
                default:
                    items = items.OrderBy(i => i.Location);
                    break;
            }

            int pageSize = 50;
            return View(await PaginatedList<Ticket>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
    }
}
