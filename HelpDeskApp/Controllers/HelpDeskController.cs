using HelpDeskApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApp.Controllers
{
    public class HelpDeskController : Controller
    {

        private readonly HelpDeskContext _db;

        public HelpDeskController(HelpDeskContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Ticket> objList = _db.Tickets;
            return View(objList);
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
            obj.Status = "Not Started";
            Console.WriteLine(ModelState.IsValid);
            Console.WriteLine(obj.Status);
            if (ModelState.IsValid)
            {
                Console.WriteLine(obj);

                _db.Tickets.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
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
    }
}
