using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models;
using Roulette.Models.EventViewModels;

namespace Roulette.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();
            var createdEvents = await _context.Events
                .Include(x => x.UserEvents)
                .Include(x => x.Winner)
                .Where(x => x.Creator.UserName == User.Identity.Name)
                .ToListAsync();
            var joinedEvents = await _context.Events
                .Include(x => x.UserEvents)
                .Include(x => x.Winner)
                .Include(x => x.Creator)
                .Where(x => x.Creator.UserName != User.Identity.Name)
                .Where(x => x.UserEvents.Any(y => y.User.UserName == User.Identity.Name))
                .ToListAsync();
            model.Events = createdEvents
                .Select(x =>
                    new IndexViewModel.EventViewModel
                    {
                        IsCreated = true,
                        Event = x
                    })
                .Concat(joinedEvents.Select(x =>
                    new IndexViewModel.EventViewModel
                    {
                        IsCreated = false,
                        Event = x
                    }))
                .OrderByDescending(x => x.Event.Created);
            return View(model);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(x => x.UserEvents)
                .ThenInclude(x => x.User)
                .Include(x => x.UserEvents)
                .ThenInclude(x => x.Event)
                .Include(x => x.Creator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Event @event)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.First(x => x.UserName == User.Identity.Name);
                @event.Creator = user;
                @event.Created = DateTime.Now;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Event update)
        {
            var @event = await _context.Events
                .Where(x => x.Creator.UserName == User.Identity.Name)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (@event == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(@event);

            try
            {
                @event.Name = update.Name;
                @event.Updated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (EventExists(update.Id))
                    throw;

                return NotFound();
            }

            return RedirectToAction("Index");
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Where(x => x.Creator.UserName == User.Identity.Name)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(x => x.UserEvents)
                .SingleOrDefaultAsync(m => m.Id == id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        [HttpGet, Route("api/events/{id}")]
        public IActionResult Get(int id)
        {
            var @event = _context.Events
                .Include(x => x.UserEvents)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == id);
            if (@event == null)
                return NotFound();
            return Json(@event);
        }
    }
}
