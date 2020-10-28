using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenApp.Data;
using GreenApp.Models;
using GreenApp.Interfaces;

namespace GreenApp.Controllers
{
    public class ChallangesController : Controller
    {
        private readonly GreenAppContext _context;

        public ChallangesController(GreenAppContext context)
        {
            _context = context;
        }

        // GET: Challanges
        public async Task<IActionResult> Index()
        {
            return View(await _context.Challange.ToListAsync());
        }

        // GET: Challanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challange = await _context.Challange
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challange == null)
            {
                return NotFound();
            }

            return View(challange);
        }

        // GET: Challanges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Challanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate,Type,Reward")] Challange challange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(challange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(challange);
        }

        // GET: Challanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challange = await _context.Challange.FindAsync(id);
            if (challange == null)
            {
                return NotFound();
            }
            return View(challange);
        }

        // POST: Challanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,Type,Reward")] Challange challange)
        {
            if (id != challange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(challange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChallangeExists(challange.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(challange);
        }

        // GET: Challanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var challange = await _context.Challange
                .FirstOrDefaultAsync(m => m.Id == id);
            if (challange == null)
            {
                return NotFound();
            }

            return View(challange);
        }

        // POST: Challanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var challange = await _context.Challange.FindAsync(id);
            _context.Challange.Remove(challange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChallangeExists(int id)
        {
            return _context.Challange.Any(e => e.Id == id);
        }

       
    }
}
