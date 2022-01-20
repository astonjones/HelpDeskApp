using HelpDeskApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Closed()
        {
            IEnumerable<Ticket> userTickets = _db.Tickets.Where(ticket => ticket.Email == User.Identity.Name && ticket.Status == "Closed");
            return View(userTickets);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult ClosedAdmin()
        {
            IEnumerable<Ticket> objList = _db.Tickets.Where(ticket => ticket.Status == "Closed");
            return View(objList);
        }
    }
}
