using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AvioesController : Controller
    {
        private readonly DataContext _context;

        public AvioesController(DataContext context)
        {
            _context = context;
        }

        // GET: Avioes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Avioes.ToListAsync());
        }

        // GET: Avioes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _context.Avioes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aviao == null)
            {
                return NotFound();
            }

            return View(aviao);
        }

        // GET: Avioes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Avioes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Marca,Modelo,LugaresEconomica,LugaresExecutiva,ImageId,Disponivel")] Aviao aviao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aviao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aviao);
        }

        // GET: Avioes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _context.Avioes.FindAsync(id);
            if (aviao == null)
            {
                return NotFound();
            }
            return View(aviao);
        }

        // POST: Avioes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,LugaresEconomica,LugaresExecutiva,ImageId,Disponivel")] Aviao aviao)
        {
            if (id != aviao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aviao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AviaoExists(aviao.Id))
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
            return View(aviao);
        }

        // GET: Avioes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aviao = await _context.Avioes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aviao == null)
            {
                return NotFound();
            }

            return View(aviao);
        }

        // POST: Avioes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aviao = await _context.Avioes.FindAsync(id);
            _context.Avioes.Remove(aviao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AviaoExists(int id)
        {
            return _context.Avioes.Any(e => e.Id == id);
        }
    }
}
