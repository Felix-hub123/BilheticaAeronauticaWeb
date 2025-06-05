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
    public class VoosController : Controller
    {
        private readonly DataContext _context;

        public VoosController(DataContext context)
        {
            _context = context;
        }

        // GET: Voos
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Voos.Include(v => v.Destino).Include(v => v.Origem);
            return View(await dataContext.ToListAsync());
        }

        // GET: Voos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await _context.Voos
                .Include(v => v.Destino)
                .Include(v => v.Origem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        // GET: Voos/Create
        public IActionResult Create()
        {
            ViewData["DestinoId"] = new SelectList(_context.Aeroportos, "Id", "Cidade");
            ViewData["OrigemId"] = new SelectList(_context.Aeroportos, "Id", "Cidade");
            return View();
        }

        // POST: Voos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumeroVoo,DataHora,Ativo,OrigemId,DestinoId,AeronaveId")] Voo voo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DestinoId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.DestinoId);
            ViewData["OrigemId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.OrigemId);
            return View(voo);
        }

        // GET: Voos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await _context.Voos.FindAsync(id);
            if (voo == null)
            {
                return NotFound();
            }
            ViewData["DestinoId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.DestinoId);
            ViewData["OrigemId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.OrigemId);
            return View(voo);
        }

        // POST: Voos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroVoo,DataHora,Ativo,OrigemId,DestinoId,AeronaveId")] Voo voo)
        {
            if (id != voo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VooExists(voo.Id))
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
            ViewData["DestinoId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.DestinoId);
            ViewData["OrigemId"] = new SelectList(_context.Aeroportos, "Id", "Cidade", voo.OrigemId);
            return View(voo);
        }

        // GET: Voos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await _context.Voos
                .Include(v => v.Destino)
                .Include(v => v.Origem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        // POST: Voos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voo = await _context.Voos.FindAsync(id);
            _context.Voos.Remove(voo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VooExists(int id)
        {
            return _context.Voos.Any(e => e.Id == id);
        }
    }
}
