using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjectManagerApp.Data;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // ✅ Dessa fält måste ligga INNE i klassen
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // ✅ Konstruktor måste också ligga inne i klassen
        public ProjectsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string statusFilter)
        {
            var userId = _userManager.GetUserId(User);

            // Börja med alla projekt för den inloggade användaren
            var projects = _context.Projects.Where(p => p.UserId == userId);

            // Filtrera om användaren valt status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Started")
                {
                    projects = projects.Where(p => p.Status == ProjectStatus.Started);
                }
                else if (statusFilter == "Completed")
                {
                    projects = projects.Where(p => p.Status == ProjectStatus.Completed);
                }
            }

            return View(await projects.ToListAsync());
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);

            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClientName,Description,StartDate,EndDate,Budget,Status")] Project project)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                project.UserId = userId;

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FindAsync(id);

            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();


            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClientName,Description,StartDate,EndDate,Budget,Status,UserId")] Project project)
        {
            if (id != project.Id) return NotFound();

            if (project.UserId != _userManager.GetUserId(User))
                return Forbid();


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);

            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();


            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null) _context.Projects.Remove(project);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }


    }
}
