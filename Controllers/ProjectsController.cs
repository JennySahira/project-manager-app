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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string statusFilter)
        {
            var projects = _context.Projects.AsQueryable();

            // Filtrerar projekt baserat på valt statusfilter. Skapad med hjälp av chat gpt.
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Started")
                    projects = projects.Where(p => p.Status == ProjectStatus.Started);
                else if (statusFilter == "Completed")
                    projects = projects.Where(p => p.Status == ProjectStatus.Completed);
            }

            // Lägger till antal projekt för varje kategori (används i flikräkning i vyn). Skapad med hjälp av chat gpt.
            ViewBag.AllCount = await _context.Projects.CountAsync();
            ViewBag.StartedCount = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Started);
            ViewBag.CompletedCount = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Completed);
            ViewBag.Filter = statusFilter;

            return View(await projects.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
            // Kontroll så att endast användaren som äger projektet får visa det. Skapad med hjälp av chat gpt.
            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(project);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClientName,Description,StartDate,EndDate,Budget,Status")] Project project)
        {
            project.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Samlar valideringsfel för felsökning (kan visas i vyn vid behov). Skapad med hjälp av chat gpt.
            ViewBag.Errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return View(project);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClientName,Description,StartDate,EndDate,Budget,Status,IconFileName,UserId")] Project project)
        {
            if (id != project.Id) return NotFound();

            project.UserId = _userManager.GetUserId(User); // Uppdaterar UserId för säkerhets skull. Skapad med hjälp av chat gpt.

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Hanterar fall där projektet ändrats samtidigt av någon annan. Skapad med hjälp av chat gpt.
                    if (!ProjectExists(project.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // Används för att ladda in redigeringsformuläret via AJAX i en Bootstrap-modal. Skapad med hjälp av chat gpt.
        public async Task<IActionResult> EditPartial(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return PartialView("EditPartial", project);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
                _context.Projects.Remove(project);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Används för att visa en bekräftelse-modal innan projekt tas bort. Skapad med hjälp av chat gpt.
        [HttpGet]
        public async Task<IActionResult> DeletePartial(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null || project.UserId != _userManager.GetUserId(User))
                return Forbid();

            return PartialView("DeletePartial", project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
